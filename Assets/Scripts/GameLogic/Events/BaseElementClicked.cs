using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Events
{
    public class BaseElementClicked
    {
        public int Index;
        public UnitModel Model;
        public Vector3 Position;

        public BaseElementClicked(int index, UnitModel model, Vector3 position)
        {
            Index = index;
            Model = model;
            Position = position;
        }
    }
}