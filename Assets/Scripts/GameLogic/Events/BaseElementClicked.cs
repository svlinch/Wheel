using Assets.Scripts.Models;

namespace Assets.Scripts.Events
{
    public class BaseElementClicked
    {
        public int Index;
        public UnitModel Model;
        public BaseElementClicked(int index, UnitModel model)
        {
            Index = index;
            Model = model;
        }
    }
}