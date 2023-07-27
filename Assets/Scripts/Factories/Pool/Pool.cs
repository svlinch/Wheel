using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.Factories
{
    public class Pool
    {
        private List<GameObject> _pool = new List<GameObject>();
        private Transform _holder;

        public Pool(Transform holder)
        {
            _holder = holder;
        }

        public GameObject Get()
        {
            if (_pool.Count > 0)
            {
                return _pool.FirstOrDefault(x => !x.activeSelf);
            }
            else
            {
                return null;
            }
        }

        public void Add(GameObject obj)
        {
            _pool.Add(obj);
        }

        public void Return(GameObject obj)
        {
            var inPool = _pool.FirstOrDefault(x => x == obj);
            if (inPool != null)
            {
                inPool.SetActive(false);
                inPool.transform.SetParent(_holder);
            }
        }
    }
}