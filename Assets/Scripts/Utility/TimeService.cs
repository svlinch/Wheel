using UnityEngine;

namespace Assets.Scripts.Utility
{
    public class TimeService: ITimeService
    {
        public TimeService()
        {
            SetPauseState(true);
        }

        public bool Pause { get; private set; }

        public void SetPauseState(bool newState)
        {
            Pause = newState;
        }

        public float GetDeltaTime(bool regardlessToPause)
        {
            if (regardlessToPause && Pause)
            {
                return 0f;
            }
            else
            {
                return Time.deltaTime;
            }
        }
    }
}
