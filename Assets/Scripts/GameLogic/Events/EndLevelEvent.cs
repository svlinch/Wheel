namespace Assets.Scripts.Events
{
    public class EndLevelEvent
    {
        public bool Win;
        public int Index;
        public int Reward;

        public EndLevelEvent(bool win, int index = 0, int reward = 0)
        {
            Win = win;
            Index = index;
            Reward = reward;
        }
    }
}