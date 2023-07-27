using Assets.Scripts.Units;

namespace Assets.Scripts.Events
{
    public class DamageEvent
    {
        public Trap Trap;
        public Enemy Enemy;

        public DamageEvent(Trap trap, Enemy enemy)
        {
            Trap = trap;
            Enemy = enemy;
        }
    }
}