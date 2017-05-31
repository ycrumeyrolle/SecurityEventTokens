using System;

namespace AspNetCore.SecurityEventTokens
{
    public class EventStreamStateFail : EventStreamState
    {
        public override string Value => "fail";

        public override EventStreamState Confirm()
        {
            throw new NotImplementedException();
        }

        public override EventStreamState ConfirmFail(string error, string errorDescription)
        {
            throw new NotImplementedException();
        }

        public override EventStreamState Disable()
        {
            throw new NotImplementedException();
        }

        public override EventStreamState Enable()
        {
            throw new NotImplementedException();
        }

        public override EventStreamState Restart()
        {
            return EventStreamState.VerifyState;
        }

        public override EventStreamState Resume()
        {
            throw new NotImplementedException();
        }

        public override EventStreamState Suspend()
        {
            throw new NotImplementedException();
        }

        public override EventStreamState Timeout(string error, string errorDescription)
        {
            throw new NotImplementedException();
        }

        public override EventStreamState Limited()
        {
            throw new NotImplementedException();
        }
    }
}