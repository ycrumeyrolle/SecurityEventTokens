using System;

namespace AspNetCore.SecurityEventTokens
{
    public class EventStreamStateOff : EventStreamState
    {
        public override string Value => "off";

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
            return EventStreamState.OnState;
        }

        public override EventStreamState Limited()
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
            throw new NotImplementedException();
        }

        public override EventStreamState Timeout(string error, string errorDescription)
        {
            throw new NotImplementedException();
        }
    }
}