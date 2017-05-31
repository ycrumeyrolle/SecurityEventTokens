using System;

namespace AspNetCore.SecurityEventTokens
{
    public class EventStreamStatePaused : EventStreamState
    {
        public override string Value => "paused";

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
            throw new NotImplementedException();
        }

        public override EventStreamState Resume()
        {
            return EventStreamState.OnState;
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
            return EventStreamState.OffState;
        }
    }
}