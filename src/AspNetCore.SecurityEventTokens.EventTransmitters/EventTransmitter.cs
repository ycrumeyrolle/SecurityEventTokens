using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace AspNetCore.SecurityEventTokens
{
    public class EventTransmitter
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly ILogger<EventTransmitter> _logger;
        private readonly ConcurrentQueue<SecurityEventToken> _queue = new ConcurrentQueue<SecurityEventToken>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IJwtSerializer _jwtSerializer;
        private readonly IEventStreamMetadataService _eventStreamMetadataProvider;

        public EventTransmitter(ILoggerFactory loggerFactory, IJwtSerializer jwtSerializer, IEventStreamMetadataService eventStreamMetadataProvider)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (jwtSerializer == null)
            {
                throw new ArgumentNullException(nameof(jwtSerializer));
            }
            
            if (eventStreamMetadataProvider == null)
            {
                throw new ArgumentNullException(nameof(eventStreamMetadataProvider));
            }

            _logger = loggerFactory.CreateLogger<EventTransmitter>();
            _jwtSerializer = jwtSerializer;
            _eventStreamMetadataProvider = eventStreamMetadataProvider;
        }

        public async Task<EventTransmitterResult> TransmitAsync(SecurityEventToken set, CancellationToken cancellationToken = default(CancellationToken))
        {
            var eventStream = await _eventStreamMetadataProvider.GetEventStreamMetadataByIssuerAsync(set.Issuer);
            if (eventStream == null)
            {
                throw new InvalidOperationException($"No event stream for this issuer '{set.Issuer}'.");
            }

            if (eventStream.SubStatus.IsOffState)
            {
                _logger.LogWarning($"The event transmitter is in state '{EventStreamState.OffState.Value}. The current SET will not be delivred. You must enable the event stream before attempting to send a new SET.");
                return EventTransmitterResult.Off();
            }

            if (eventStream.SubStatus.IsFailState)
            {
                _logger.LogWarning($"The event transmitter is in state '{EventStreamState.FailState.Value}. The current SET will not be delivred. You must restart the event stream before attempting to send a new SET.");
                return EventTransmitterResult.Fail();
            }

            if (eventStream.SubStatus.IsPausedState)
            {
                _logger.LogWarning($"The event transmitter is in state '{EventStreamState.PausedState.Value}. The current SET will be delivred when the event stream will be resumed.");
                _queue.Enqueue(set);
                // TODO : So what ?
                return EventTransmitterResult.Paused();
            }

            if (eventStream.SubStatus.IsVerifyState)
            {
                try
                {
                    await _semaphore.WaitAsync(cancellationToken);

                    // double check verify state
                    if (eventStream.SubStatus.IsVerifyState)
                    {
                        await ConfirmStateStream(eventStream, cancellationToken);
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            if (eventStream.SubStatus.IsOnState)
            {
                string transmissionError = null;
                string transmissionErrorDescription = null;
                string token = await CreateJwtStringAsync(set);
                var sendJwtFailed = false;
                for (int i = 0; i < eventStream.MaxRetries; i++)
                {
                    sendJwtFailed = false;
                    HttpResponseMessage response = null;
                    try
                    {
                        response = await SendJwt(token, eventStream, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        sendJwtFailed = true;
                        transmissionError = "connection";
                        transmissionErrorDescription = "Timeout.";
                    }
                    catch (HttpRequestException)
                    {
                        sendJwtFailed = true;
                        transmissionError = "receiver";
                        transmissionErrorDescription = "An error occured while sending the SET.";
                    }

                    if (response != null)
                    {
                        // TODO : 202 only ?
                        if (response.StatusCode != HttpStatusCode.Accepted)
                        {
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            if (string.IsNullOrEmpty(errorMessage))
                            {
                                return EventTransmitterResult.Error();
                            }

                            var error = JObject.Parse(errorMessage);
                            return EventTransmitterResult.Error(error.Value<string>("description"), error.Value<string>("err"));
                        }
                        else
                        {
                            break;
                        }
                    }

                    _logger.LogWarning($"SET transmission failed for the {i + 1} times. Retrying in {eventStream.MinDeliveryInterval} seconds.");
                    await Task.Delay(eventStream.MinDeliveryInterval);
                }

                if (sendJwtFailed)
                {
                    eventStream.SubStatus.Timeout(transmissionError, transmissionErrorDescription);
                    return EventTransmitterResult.Error(transmissionError, transmissionErrorDescription);
                }

                _logger.LogInformation($"SET '{set.JwtId}' has been sent.");
                return EventTransmitterResult.Success();
            }

            throw new InvalidOperationException();
        }

        private async Task ConfirmStateStream(EventStreamMetadata eventStream, CancellationToken cancellationToken)
        {
            string confirmChallenge;
            string transmissionError = null;
            string transmissionErrorDescription = null;
            var verify = CreateVerifySecurityEventToken(eventStream, out confirmChallenge);
            string verifyToken = await CreateJwtStringAsync(verify);
            bool verifyFailed = false;

            for (int i = 0; i < eventStream.MaxRetries; i++)
            {
                verifyFailed = false;
                HttpResponseMessage verifyResponse = null;
                try
                {
                    verifyResponse = await SendJwt(verifyToken, eventStream, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    verifyFailed = true;
                    transmissionError = "connection";
                    transmissionErrorDescription = "Timeout.";
                }
                catch (HttpRequestException)
                {
                    verifyFailed = true;
                    transmissionError = "receiver";
                    transmissionErrorDescription = "An error occured while sending the SET.";
                }

                if (verifyResponse != null)
                {
                    if (verifyResponse.StatusCode != HttpStatusCode.OK || verifyResponse.Content.Headers?.ContentType?.MediaType != "application/json")
                    {
                        verifyFailed = true;
                    }
                    else
                    {
                        var verifyMessage = await verifyResponse.Content.ReadAsStringAsync();
                        if (string.IsNullOrEmpty(verifyMessage))
                        {
                            verifyFailed = true;
                        }
                        else
                        {
                            var verifyJson = JObject.Parse(verifyMessage);
                            var challengeResponse = verifyJson.Value<string>(Constants.VerifyChallengeResponse);
                            if (string.Equals(challengeResponse, confirmChallenge, StringComparison.Ordinal))
                            {
                                break;
                            }
                        }
                    }
                }

                _logger.LogWarning($"Verification failed for the {i+1} times. Retrying in {eventStream.MinDeliveryInterval} seconds.");
                await Task.Delay(eventStream.MinDeliveryInterval);
            }

            if (verifyFailed)
            {
                _logger.LogError($"An error occurred on the verify the event stream. {transmissionError} : {transmissionErrorDescription}");
                eventStream.SubStatus.ConfirmFail(transmissionError, transmissionErrorDescription);
                throw new SecurityEventTokenException(transmissionError, transmissionErrorDescription);
            }

            _logger.LogInformation($"Verification succeeded.");
            eventStream.SubStatus.Confirm();
            while (_queue.TryDequeue(out var remaining))
            {
                await TransmitAsync(remaining);
            }
        }

        private async Task<HttpResponseMessage> SendJwt(string jwtString, EventStreamMetadata eventStream, CancellationToken cancellationToken)
        {
            var content = new StringContent(jwtString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/jwt");
            if(eventStream.MaxDeliveryTime == 0)
            {
                    return await _httpClient.PostAsync(eventStream.DeliveryUri, content, cancellationToken);
            }
            else
            {
                using (var cts = new CancellationTokenSource(eventStream.MaxDeliveryTime * 1000))
                {
                    using (var mergedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token))
                    {
                        return await _httpClient.PostAsync(eventStream.DeliveryUri, content, mergedCts.Token);
                    }
                }
            }
        }

        public async Task<string> CreateJwtStringAsync(SecurityEventToken token)
        {           
            var jwtString = await _jwtSerializer.SerializeAsync(token);
            return jwtString;
        }

        /// <summary>
        /// Create a verify SET.
        /// <code>
        /// {
        ///   "jti": "4d3559ec67504aaba65d40b0363faad8",
        ///   "events":["[[this RFC URL]]#verify"],
        ///   "iat": 1458496404,
        ///   "iss": "https://scim.example.com",
        ///   "exp": 1458497000,
        ///   "aud":[
        ///    "https://scim.example.com/Feeds/98d52461fa5bbc879593b7754",
        ///    "https://scim.example.com/Feeds/5d7604516b1d08641d7676ee7"
        ///   ],
        ///   "[[this RFC URL]]#verify":{
        ///     "confirmChallenge":"ca2179f4-8936-479a-a76d-5486e2baacd7"
        ///   }
        /// }
        /// </code>
        /// </summary>
        /// <returns></returns>
        public static SecurityEventToken CreateVerifySecurityEventToken(EventStreamMetadata eventStream, out string challengeConfirm)
        {
            string jti = Guid.NewGuid().ToString();
            challengeConfirm = Guid.NewGuid().ToString();

            return new SecurityEventTokenBuilder()
                .JwtId(jti)
                .Issuer(eventStream.FeedUri)
                .Expires(DateTime.UtcNow.AddSeconds(eventStream.MaxDeliveryTime))
                .Audiences(eventStream.Audiences)
                .VerifyEvent(challengeConfirm)
                .Build();
        }
    }
}
