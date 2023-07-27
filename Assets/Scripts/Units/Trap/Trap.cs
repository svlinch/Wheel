using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Assets.Scripts.GameData;
using Assets.Scripts.UI;
using Assets.Scripts.Models;

namespace Assets.Scripts.Units
{
    public abstract class Trap : MonoBehaviour
    {
        #region Injection
        protected EventService _eventService;

        [Inject]
        public void Inject(EventService eventService)
        {
            _eventService = eventService;
        }
        #endregion

        [SerializeField]
        protected GameObject _visualPart;
        [SerializeField]
        protected TrapStatusCanvas _statusCanvas;
        [SerializeField]
        protected Transform _spawnPoint;
        [SerializeField]
        protected Transform _directionHelp;

        protected int _indexInList;
        protected UnitModel _model;
        protected List<ImpactModel> _impacts = new List<ImpactModel>();

        public UnitModel GetModel()
        {
            return _model;
        }

        public abstract void Setup(int index);
        public abstract void SetModel(UnitModel model);
        public abstract void HandleUpdate(float dTime);
        public abstract void SetBattleState(bool state);
        public abstract void HandleDamageEvent(Dictionary<string, float> numeric, List<ImpactTemplate> impacts);
        public abstract bool RemoveCheck();
        public abstract void Remove();
        public abstract void HandleClick();
        protected abstract void ApplyChanges(Dictionary<string, float> numeric);
        protected abstract void ApplyDamage(float change);
        protected abstract void ApplyImpacts(List<ImpactTemplate> impacts);
        protected abstract void UpdateImpacts(float dTime);
        protected virtual void HandleContact(Transform enemy) { }
        protected void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("enemy"))
            {
                HandleContact(other.transform);
            }
        }
    }
}