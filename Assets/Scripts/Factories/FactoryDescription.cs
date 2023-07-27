using UnityEngine;

namespace Assets.Scripts.Factories
{
    public class FactoryDescription
    {
        public string PrefabName;
        public string Kind;
        public EPositionType PositionType;
        public Vector3? Position;
        public Transform Parent;
    }

    public enum EObjectType
    {
        Enemy,
        Base,
        Bullet
    }

    public enum EPositionType
    {
        World,
        Local
    }

    public class FactoryDescriptionBuilder
    {
        private string _prefabName;
        private Vector3? _position;
        private EObjectType? _objectType;
        private EPositionType _positionType;
        private Transform _parent;
        private string _kind;

        public static FactoryDescriptionBuilder Object()
        {
            return new FactoryDescriptionBuilder();
        }

        public FactoryDescriptionBuilder Parent(Transform parent)
        {
            _parent = parent;
            return this;
        }

        public FactoryDescriptionBuilder PrefabName(string prefabName)
        {
            _prefabName = prefabName;
            return this;
        }

        public FactoryDescriptionBuilder Position(Vector3 position)
        {
            _position = position;
            return this;
        }

        public FactoryDescriptionBuilder PositionType(EPositionType type)
        {
            _positionType = type;
            return this;
        }

        public FactoryDescriptionBuilder Kind(string kind)
        {
            _kind = kind;
            return this;
        }
        
        public FactoryDescriptionBuilder Type(EObjectType type)
        {
            _objectType = type;
            return this;
        }

        public FactoryDescription Build()
        {
            var result = new FactoryDescription();

            if (string.IsNullOrEmpty(_prefabName))
            {
                _prefabName = string.Format("{0}{1}", GetPath(), _kind);
            }

            result.PositionType = _positionType;
            result.Parent = _parent;
            result.PrefabName = _prefabName;
            result.Position = _position;
            result.Kind = _kind;

            return result;
        }

        private string GetPath()
        {
            if (_objectType == null || string.IsNullOrEmpty(_kind))
            {
                Debug.LogWarning(string.Format("Cant get prefab with parameters: {0},{1} ",_objectType, _kind));
                return null;
            }
            switch (_objectType.Value)
            {
                case EObjectType.Base: return StaticPathTranslator.TRAP_PATH;
                case EObjectType.Enemy: return StaticPathTranslator.ENEMY_PATH;
                case EObjectType.Bullet: return StaticPathTranslator.BATTLE_PATH;
                default: return null;
            }
        }
    }
}
