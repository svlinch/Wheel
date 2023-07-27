using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Factories
{
    public class PoolFactory
    {
        private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();
        private IObjectFactory _factory;
        private Transform _holder;

        public PoolFactory(IObjectFactory factory, string type)
        {
            _factory = factory;
            _holder = new GameObject().transform;
            _holder.name = string.Format("pool:{0}", type);
        }

        public GameObject Create(FactoryDescription description)
        {
            var pool = GetPool(description.Kind);
            var obj = pool.Get();
            if (obj == null)
            {
                obj = _factory.Create(description);
                pool.Add(obj);
            }
            obj.SetActive(true);
            CheckoutPosition(obj.transform, description);
            return obj;
        }

        public void ReturnToPool(GameObject obj, string kind)
        {
            var pool = GetPool(kind);
            pool.Return(obj);
        }

        private Pool GetPool(string kind)
        {
            Pool pool;
            if (!_pools.TryGetValue(kind, out pool))
            {
                pool = new Pool(_holder);
                _pools.Add(kind, pool);
            }
            return pool;
        }

        private void CheckoutPosition(Transform tr, FactoryDescription description)
        {
            tr.SetParent(description.Parent);
            if (description.Position.HasValue)
            {
                switch (description.PositionType)
                {
                    case EPositionType.Local: tr.localPosition = description.Position.Value; break;
                    case EPositionType.World: tr.position = description.Position.Value; break;
                    default: tr.localPosition = description.Position.Value; break;
                }
            }
        }
    }
}
