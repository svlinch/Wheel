using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Factories
{
    public class MainFactory : IObjectFactory
    {
        private class Template
        {
            public GameObject Prefab;
        }

        #region Injection
        private DiContainer _container;

        public MainFactory(DiContainer container)
        {
            _container = container;
        }
        #endregion

        private readonly Dictionary<string, Template> _loadedTemplates = new Dictionary<string, Template>();

        public GameObject Create(FactoryDescription description)
        {
            var template = GetTemplate(description);
            if (template == null)
            {
                return null;
            }

            var result = InstantiateFromTemplate(template, description);
            return result;
        }

        private Template GetTemplate(FactoryDescription description)
        {
            Template template;
            if (!_loadedTemplates.TryGetValue(description.PrefabName, out template))
            {
                template = LoadTemplate(description.PrefabName);
                if (template == null)
                {
                    Debug.LogWarning(string.Format("Prefab {0} not found", description.PrefabName));
                    return null;
                }
                _loadedTemplates[description.PrefabName] = template;
            }
            return template;
        }

        private Template LoadTemplate(string prefabName)
        {
            var template = new Template();
            template.Prefab = Resources.Load<GameObject>(prefabName);
            if (template.Prefab == null)
            {
                return null;
            }
            return template;
        }

        private GameObject InstantiateFromTemplate(Template template, FactoryDescription description)
        {
            GameObject result = null;
            if (description.Parent != null)
            {
                result = Object.Instantiate(template.Prefab, description.Parent);
            }
            else
            {
                result = Object.Instantiate(template.Prefab);
            }
            if (description.Position.HasValue)
            {
                switch (description.PositionType)
                {
                    case EPositionType.Local: result.transform.localPosition = description.Position.Value; break;
                    case EPositionType.World: result.transform.position = description.Position.Value; break;
                    default: result.transform.localPosition = description.Position.Value; break;
                }
            }

            _container.InjectGameObject(result);
            return result;
        }
    }
}