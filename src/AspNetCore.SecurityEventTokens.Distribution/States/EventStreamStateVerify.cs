using System;

namespace AspNetCore.SecurityEventTokens
{
    public class EventStreamStateVerify : EventStreamState
    {
        public override string Value => "verify";

        public override EventStreamState Confirm()
        {
            return OnState;
        }

        public override EventStreamState ConfirmFail(string error, string errorDescription)
        {
            return FailState;
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
