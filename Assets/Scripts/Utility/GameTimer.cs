namespace Assets.Scripts.Utility
{
    public class GameTimer
    {
        private ITimeService _timeService;
        private float _remainsTime;
        private bool _regardlessToPause;

        public GameTimer(ITimeService timeService, float duration, bool regardlessToPause = true)
        {
            _timeService = timeService;
            _remainsTime = duration;
            _regardlessToPause = regardlessToPause;
        }

        public GameTimer(float duration)
        {
            _remainsTime = duration;
        }

        public void CheckoutTimer(float duration)
        {
            _remainsTime = duration;
        }

        public bool HandleUpdate()
        {
            if (_remainsTime > 0f)
            {
                _remainsTime -= _timeService.GetDeltaTime(_regardlessToPause);
                return false;
            }
            return true;
            
        }

        public bool HandleUpdate(float dTime)
        {
            if (_remainsTime > 0f)
            {
                _remainsTime -= dTime;
                return false;
            }
            return true;
        }
    }
}
