using System.Collections;
using System;

namespace Assets.Scripts
{
    public interface IInitialize
    {
        public IEnumerator Initialize();
        public Type Type();
    }
}
