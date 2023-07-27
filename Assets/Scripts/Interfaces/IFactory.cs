using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Factories
{
    public interface IObjectFactory
    {
        public GameObject Create(FactoryDescription description);
    }
}
