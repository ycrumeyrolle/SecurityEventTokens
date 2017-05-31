using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCore.SecurityEventTokens
{
    public class EventStreamMetadata
    {
        public EventStreamMetadata()
        {
            SubStatus = new EventTransmitterStateWrapper(this);
        }

        /// <summary>
        /// An OPTIONAL JSON String value containing the URI for a feed
        /// supported by the feed provider.  It describes the content of the
        /// feed and MAY also be a resolvable URI where the feed meta data may
        /// be returned as a JSON object.  REQUIRED.
        /// </summary>
        /// <remarks>REQUIRED or OPTIONNAL ???</remarks>
        public string FeedUri { get; set; }

        /// <summary>
        ///  A REQUIRED JSON String value which is a URI with a prefix of
        /// "urn:ietf:params:set:method".  This specification defines HTTP
        /// POST delivery method:
        /// "urn:ietf:params:set:method:HTTP:webCallback"
        /// in which the Feed Provider delivers events using HTTP POST to a
        /// specified callback URI.
        /// </summary>
        public string MethodUri { get; set; } = Constants.WebCallbackMethodUri;

        /// <summary>
        /// A JSON String value containing a URI that describes the location
        /// where SETs are received(e.g.via HTTP POST).  Its format and
        /// usage requirements are defined by the associated "methodUri".
        /// </summary>
        public string DeliveryUri { get; set; }

        /// <summary>
        /// An OPTIONAL JSON Array of JSON String values which are URIs
        /// representing the audience(s) of the Event Stream.The value SHALL
        /// be the value of SET "aud" claim sent to the Event Receiver
        /// </summary>
        public ICollection<string> Audiences { get; set; }

        /// <summary>
        /// An OPTIONAL public JSON Web Key (see [RFC7517]) from the Event
        /// Transmitter that will be used by the Event Receiver to verify the
        /// authenticity of issued SETs.
        /// </summary>
        public JsonWebKey FeedJwk { get; set; }

        /// <summary>
        /// An OPTIONAL public JSON Web Key (see [RFC7517]) for the Event
        /// Receiver that MAY be used by the Feed Provider to encrypt SET
        /// tokens for the specified Event Receiver.
        /// </summary>
        public JsonWebKey ConfidentialJwk { get; set; }

        /// <summary>
        /// An OPTIONAL JSON number indicating the maximum number of attempts
        /// to deliver a SET.A value of '0' indicates there is no maximum.
        /// Upon reaching the maximum, the Event Stream "subStatus" attribute
        /// is set to "failed".
        /// </summary>
        public int MaxRetries { get; set; }

        /// <summary>
        /// An OPTIONAL number indicating the maximum amount of time in
        /// seconds a SET MAY take for successful delivery per request or
        /// cumulatively across multiple retries.Upon reaching the maximum,
        /// the Event Stream "subStatus" is set to "failed".  If undefined,
        /// there is no maximum time.
        /// </summary>
        public int MaxDeliveryTime { get; set; }

        /// <summary>
        /// An OPTIONAL JSON integer that represents the minimum interval in
        /// seconds between deliveries.A value of '0' indicates delivery
        /// should happen immediately.When delivery is a polling method
        /// (e.g.HTTP GET), it is the expected time between Event Receiver
        /// attempts.When in push mode(e.g.HTTP POST), it is the interval
        /// the server will wait before sending a new event or events.
        /// </summary>
        public int MinDeliveryInterval { get; set; }

        /// <summary>
        /// An OPTIONAL JSON String keyword value.  When the Event Stream has
        /// "subState" set to "fail", one of the following error keywords is
        /// set:
        ///
        /// "connection" indicates an error occurred attempting to open a
        /// TCP connection with the assigned endpoint.
        ///
        /// "tls" indicates an error occurred establishing a TLS connection
        /// with the assigned endpoint.
        ///
        /// "dnsname" indicates an error occurred establishing a TLS
        /// connection where the dnsname was not validated.
        ///
        /// "receiver" indicates an error occurred whereby the Event
        /// Receiver has indicated an error for which the Event Transmitter
        /// is unable to correct.
        /// </summary>
        public string TransactionError { get; set; }

        /// <summary>
        /// An OPTIONAL String value that is usually human readable that
        /// provides further diagnostic detail by the indicated "txErr" error
        /// code.
        /// </summary>
        public string TransactionErrorDescription { get; set; }

        /// <summary>
        /// An OPTIONAL JSON String keyword that indicates the current state
        /// of an Event Stream.More information on the Event Stream state
        /// can be found in Section 2.2.  Valid keywords are:
        ///
        ///  "on" - indicates the Event Stream has been verified and that
        ///  the Feed Provider MAY pass SETs to the Event Receiver.
        ///
        ///  "verify" - indicates the Event Stream is pending verification.
        ///  While in "verify", SETs, except for the verify SET (see
        ///  Section 3.4) are not delivered to the Event Receiver.Once
        ///  verified, the status returns to "on".
        ///
        ///  "paused" - indicates the Event Stream is temporarily suspended.
        ///  While "paused", SETs SHOULD be retained and delivered when
        ///  state returns to "on".  If delivery is paused for an extended
        ///  period defined by the Event Transmitter, the Event Transmitter
        ///  MAY change the state to "off" indicating SETs are no longer
        ///  retained.
        ///
        ///  "off" - indicates that the Event Stream is no longer passing
        ///  SETs.  While in off mode, the Event Stream metadata is
        ///  maintained, but new events are ignored, not delivered or
        ///  retained.  Before returning to "on", a verification MUST be
        ///  performed.
        ///
        ///  "fail" - indicates that the Event Stream was unable to deliver
        ///  SETs to the Event Receiver due an unrecoverable error or for an
        ///  extended period of time.Unlike paused status, a failed Event
        ///  Stream does not retain existing or new SETs that are issued.
        ///  Before returning to "on", a verification MUST be performed.
        /// </summary>
        public EventStreamState SubStatus { get; }

        private class EventTransmitterStateWrapper : EventStreamState
        {
            private readonly EventStreamMetadata _metadata;
            private EventStreamState _inner;

            public EventTransmitterStateWrapper(EventStreamMetadata metadata)
            {
                _metadata = metadata;
                _inner = Create();
            }

            public override string Value => _inner.Value;

            public override EventStreamState Confirm()
            {
                ClearError();
                return _inner = _inner.Confirm();
            }

            public override EventStreamState ConfirmFail(string error, string errorDescription)
            {
                SetError(error, errorDescription);
                return _inner = _inner.ConfirmFail(error, errorDescription);
            }

            public override EventStreamState Disable()
            {
                ClearError();
                return _inner = _inner.Disable();
            }

            public override EventStreamState Enable()
            {
                ClearError();
                return _inner = _inner.Enable();
            }

            public override EventStreamState Limited()
            {
                ClearError();
                return _inner = _inner.Limited();
            }

            public override EventStreamState Restart()
            {
                ClearError();
                return _inner = _inner.Restart();
            }

            public override EventStreamState Resume()
            {
                ClearError();
                return _inner = _inner.Resume();
            }

            public override EventStreamState Suspend()
            {
                ClearError();
                return _inner = _inner.Suspend();
            }

            public override EventStreamState Timeout(string error, string errorDescription)
            {
                SetError(error, errorDescription);
                return _inner = _inner.Timeout(error, errorDescription);
            }

            public override bool Equals(object obj)
            {
                return _inner.Equals(obj);
            }

            public override int GetHashCode()
            {
                return _inner.GetHashCode();
            }

            private void SetError(string error, string errorDescription)
            {
                _metadata.TransactionError = error;
                _metadata.TransactionErrorDescription = errorDescription;
            }

            private void ClearError()
            {
                _metadata.TransactionError = null;
                _metadata.TransactionErrorDescription = null;
            }
        }
    }
}
