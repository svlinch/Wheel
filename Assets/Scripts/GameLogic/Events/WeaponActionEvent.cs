using UnityEngine;
using Assets.Scripts.GameData;
using Assets.Scripts.Units;

namespace Assets.Scripts.Events
{
    public class WeaponActionEvent
    {
        public Transform Start;
        public Transform Direction;
        public WeaponTemplate Template;
        public Trap Trap;

        public WeaponActionEvent(Transform start, Transform direction, Trap trap, WeaponTemplate template)
        {
            Start = start;
            Direction = direction;
            Trap = trap;
            Template = template;
        }
    }
}
