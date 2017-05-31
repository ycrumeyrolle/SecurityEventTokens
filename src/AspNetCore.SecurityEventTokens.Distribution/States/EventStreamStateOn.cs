using System;

namespace AspNetCore.SecurityEventTokens
{
    public class EventStreamStateOn : EventStreamState
    {
        public override string Value => "on";

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
            return EventStreamState.OffState;
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
            throw new NotImplementedException();
        }

        public override EventStreamState Suspend()
        {
            return EventStreamState.PausedState;
        }

        public override EventStreamState Timeout(string error, string errorDescription)
        {
            return EventStreamState.FailState;
        }

        public override EventStreamState Limited()
        {
            throw new NotImplementedException();
        }
    }
}