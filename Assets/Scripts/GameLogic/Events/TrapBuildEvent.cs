using Assets.Scripts.Units;

namespace Assets.Scripts.Events
{
    public class TrapBuildEvent
    {
        public bool Reset;
        public Trap NewTrap;

        public TrapBuildEvent(Trap newTrap, bool reset = false)
        {
            Reset = reset;
            NewTrap = newTrap;
        }
    }
}