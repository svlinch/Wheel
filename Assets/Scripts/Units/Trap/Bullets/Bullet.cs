using UnityEngine;
using Zenject;
using Assets.Scripts.Factories;
using Assets.Scripts.GameData;
using Assets.Scripts.Events;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Units
{
    public class Bullet : MonoBehaviour
    {
        #region Injection
        private EventService _eventService;
        private ITimeService _timeService;
        private SubscriptionHolder _subscriptions;

        [Inject]
        private void Inject(EventService eventService, ITimeService timeService)
        {
            _eventService = eventService;
            _timeService = timeService;
            _subscriptions = new SubscriptionHolder(_eventService);
            _subscriptions.Subscribe<EndLevelEvent>(HandleEndLevel);
        }
        #endregion

        private PoolFactory _factory;
        private WeaponTemplate _weaponTemplate;
        private Vector3 _direction;
        private Transform _transform;
        private UnitTemplateHolder _unitTemplate;

        public void Initialize(PoolFactory factory, WeaponTemplate template, UnitTemplateHolder trapTemplate, Vector3 direction)
        {
            _factory = factory;
            _weaponTemplate = template;
            _unitTemplate = trapTemplate;
            _direction = direction;
            _transform = transform;
        }

        private void Update()
        {
            _transform.Translate(_direction * _timeService.GetDeltaTime(false));
        }

        private bool HandleEndLevel(EndLevelEvent e)
        {
            _factory.ReturnToPool(gameObject, _weaponTemplate.Spawn);
            return true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("enemy"))
            {
                var enemyScript = other.GetComponentInParent<Enemy>();
                if (enemyScript != null)
                {
                    _eventService.SendMessage(new SimpleDamageEvent(_unitTemplate, _weaponTemplate, enemyScript));
                }
                _factory.ReturnToPool(gameObject, _weaponTemplate.Spawn);
            }
        }
    }
}