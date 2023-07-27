using Assets.Scripts.GameData;
using Assets.Scripts.Units;

namespace Assets.Scripts.Events
{
    public class SimpleDamageEvent
    {
        public UnitTemplateHolder TrapTemplate;
        public WeaponTemplate Weapon;
        public Enemy Enemy;

        public SimpleDamageEvent(UnitTemplateHolder template, WeaponTemplate weapon, Enemy enemy)
        {
            TrapTemplate = template;
            Weapon = weapon;
            Enemy = enemy;
        }
    }
}