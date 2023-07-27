namespace Assets.Scripts.Utility
{
    public interface ITimeService
    {
        public bool Pause { get; }
        public void SetPauseState(bool newState);
        public float GetDeltaTime(bool regardlessToPause);
    }
}