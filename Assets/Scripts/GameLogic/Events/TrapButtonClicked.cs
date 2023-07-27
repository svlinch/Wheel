using Assets.Scripts.GameData;

namespace Assets.Scripts.Events
{
    public class TrapButtonClicked
    {
        public UnitTemplateHolder Template;

        public TrapButtonClicked(UnitTemplateHolder template)
        {
            Template = template;
        }
    }
}