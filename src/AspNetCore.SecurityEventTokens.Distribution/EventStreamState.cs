using System;

namespace AspNetCore.SecurityEventTokens
{
    public abstract class EventStreamState
    {
        public static readonly EventStreamState VerifyState = new EventStreamStateVerify();
        public static readonly EventStreamState OnState = new EventStreamStateOn();
        public static readonly EventStreamState FailState = new EventStreamStateFail();
        public static readonly EventStreamState OffState = new EventStreamStateOff();
        public static readonly EventStreamState PausedState = new EventStreamStatePaused();

        public bool IsVerifyState => Equals(VerifyState);
        public bool IsOnState => Equals(OnState);
        public bool IsOffState => Equals(OffState);
        public bool IsFailState => Equals(FailState);
        public bool IsPausedState => Equals(PausedState);

        public abstract string Value { get; }
        public abstract EventStreamState Limited();
        public abstract EventStreamState Enable();
        public abstract EventStreamState Disable();
        public abstract EventStreamState Suspend();
        public abstract EventStreamState Resume();
        public abstract EventStreamState Timeout(string error, string errorDescription);
        public abstract EventStreamState Confirm();
        public abstract EventStreamState ConfirmFail(string error, string errorDescription);
        public abstract EventStreamState Restart();

        public static EventStreamState Create()
        {
            return VerifyState;
        }
    }
}
