using System.Collections.Generic;
using System;

namespace Assets.Scripts.GameData
{
    [Serializable]
    public class TemplateStruct<T> where T : struct
    {
        public T[] Templates;
    }

    [Serializable]
    public class TemplateClass<T> where T : class
    {
        public List<T> Templates;
    }
}