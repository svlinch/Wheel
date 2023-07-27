using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Zenject;
using Assets.Scripts.Factories;
using Assets.Scripts.GameData;
using Assets.Scripts.Events;
using Assets.Scripts.Units;

namespace Assets.Scripts.GameLogic
{
    public class DamageManager
    {
        #region Injection
        private SubscriptionHolder _subscriptions;
        private BalanceService _balanceService;
        private PoolFactory _factory;

        [Inject]
        public void Inject(EventService eventService, BalanceService balanceService, MainFactory factory)
        {
            _balanceService = balanceService;
            _factory = new PoolFactory(factory, "bullets");

            _subscriptions = new SubscriptionHolder(eventService);
            _subscriptions.Subscribe<DamageEvent>(HandleDamageEvent);
            _subscriptions.Subscribe<SimpleDamageEvent>(HandleSimpleDamageEvent);
            _subscriptions.Subscribe<WeaponActionEvent>(HandleWeaponActionEvent);

        }
        #endregion

        private bool HandleDamageEvent(DamageEvent e)
        {
            var trap = e.Trap;
            var enemy = e.Enemy;
            //var resultT = new DamageEventResult(trap.GetModel().ReadParameter(StaticParameterTranslator.HEALTH), enemy.GetModel().ReadParameter(StaticParameterTranslator.HEALTH));
            var resultT = new DamageEventResult(trap.GetModel().ReadParameter(StaticParameterTranslator.HEALTH));
            var resultE = new DamageEventResult(enemy.GetModel().ReadParameter(StaticParameterTranslator.HEALTH));

            var trapWeapons = trap.GetModel().Template.GetWeapons().Where(x => x.Type == EWeaponType.OnTouch).OrderByDescending(x => x.Priority).ToArray();
            var enemyWeapons = enemy.GetModel().Template.GetWeapons().Where(x => x.Type == EWeaponType.OnTouch).OrderByDescending(x => x.Priority).ToArray();

            var maxTrapPriority = 0;
            var maxEnemyPriority = 0;
            if (trapWeapons.Length > 0)
            {
                maxTrapPriority = trapWeapons[0].Priority;
            }
            if (enemyWeapons.Length > 0)
            {
                maxEnemyPriority = enemyWeapons[0].Priority;
            }

            var maxPriotity = Mathf.Max(maxTrapPriority, maxEnemyPriority);

            var formulaResult = 0f;

            for (int currentPriority = maxPriotity; currentPriority >= 0; currentPriority--)
            {
                if (resultT.NumericParameters[StaticParameterTranslator.CURRENT_HEALTH] - resultT.NumericParameters[StaticParameterTranslator.HEALTH] > 0f)
                {
                    for (int j = 0; j < trapWeapons.Length; j++)
                    {
                        if (trapWeapons[j].Priority == currentPriority)
                        {
                            formulaResult = _balanceService.GetFormula(trapWeapons[j].Formula).GetResult(
                                trap.GetModel().Template.GetNumericParameters(),
                                enemy.GetModel().Template.GetNumericParameters(),
                                resultT.NumericParameters,
                                resultE.NumericParameters,
                                trapWeapons[j].NumericParameters);
                            switch (trapWeapons[j].TargetType)
                            {
                                case ETargetType.Self:
                                    resultT.ChangeParameter(trapWeapons[j].ChangeParameter, formulaResult);
                                    if (trapWeapons[j].Impact != null)
                                    {
                                        resultT.Impacts.Add(trapWeapons[j].Impact);
                                    }
                                    break;
                                case ETargetType.Common:
                                    resultE.ChangeParameter(trapWeapons[j].ChangeParameter, formulaResult);
                                    if (trapWeapons[j].Impact != null)
                                    {
                                        resultE.Impacts.Add(trapWeapons[j].Impact);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (resultE.NumericParameters[StaticParameterTranslator.CURRENT_HEALTH] - resultE.NumericParameters[StaticParameterTranslator.HEALTH] > 0f)
                {
                    for (int j = 0; j < enemyWeapons.Length; j++)
                    {
                        if (enemyWeapons[j].Priority == currentPriority)
                        {
                            formulaResult = _balanceService.GetFormula(enemyWeapons[j].Formula).GetResult(
                                trap.GetModel().Template.GetNumericParameters(),
                                enemy.GetModel().Template.GetNumericParameters(),
                                resultT.NumericParameters,
                                resultE.NumericParameters,
                                enemyWeapons[j].NumericParameters);
                            switch (enemyWeapons[j].TargetType)
                            {
                                case ETargetType.Self:
                                    resultE.ChangeParameter(enemyWeapons[j].ChangeParameter, formulaResult);
                                    if (enemyWeapons[j].Impact != null)
                                    {
                                        resultE.Impacts.Add(enemyWeapons[j].Impact);
                                    }
                                    break;
                                case ETargetType.Common:
                                    resultT.ChangeParameter(enemyWeapons[j].ChangeParameter, formulaResult);
                                    if (enemyWeapons[j].Impact != null)
                                    {
                                        resultT.Impacts.Add(enemyWeapons[j].Impact);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            resultT.Finish();
            resultE.Finish();
            trap.HandleDamageEvent(resultT.NumericParameters, resultT.Impacts);
            enemy.HandleDamageEvent(resultE.NumericParameters, resultE.Impacts);
            return true;
        }

        private bool HandleSimpleDamageEvent(SimpleDamageEvent e)
        {
            var enemy = e.Enemy;
            var result = new DamageEventResult(enemy.GetModel().ReadParameter(StaticParameterTranslator.HEALTH));

            result.NumericParameters[StaticParameterTranslator.HEALTH] += _balanceService.GetFormula(e.Weapon.Formula).GetResult(
                    e.TrapTemplate.GetNumericParameters(),
                    enemy.GetModel().Template.GetNumericParameters(),
                    null,
                    result.NumericParameters,
                    e.Weapon.NumericParameters);

            if (e.Weapon.Impact != null)
            {
                result.Impacts.Add(e.Weapon.Impact);
            }

            result.Finish();
            enemy.HandleDamageEvent(result.NumericParameters, result.Impacts);
            return true;
        }

        private bool HandleWeaponActionEvent(WeaponActionEvent e)
        {
            var weapon = e.Template;
            if (string.IsNullOrEmpty(weapon.Spawn) && weapon.TargetType == ETargetType.Self)
            {
                var result = new DamageEventResult(e.Trap.GetModel().ReadParameter(StaticParameterTranslator.HEALTH));
                if (weapon.Impact != null)
                {
                    result.Impacts.Add(weapon.Impact);
                }
                result.NumericParameters[StaticParameterTranslator.HEALTH] += _balanceService.GetFormula(weapon.Formula).GetResult(
                    e.Trap.GetModel().Template.GetNumericParameters(),
                    null,
                    result.NumericParameters,
                    null,
                    null);
                result.Finish();
                e.Trap.HandleDamageEvent(result.NumericParameters, result.Impacts);
            }
            else
            {
                var description = FactoryDescriptionBuilder.Object()
                                .Type(EObjectType.Bullet)
                                .Kind(weapon.Spawn)
                                .Position(e.Start.position)
                                .PositionType(EPositionType.World)
                                .Build();
                var bullet = _factory.Create(description).GetComponent<Bullet>();
                bullet.Initialize(_factory, weapon, e.Trap.GetModel().Template, e.Direction.position - e.Start.position);
            }

            return true;
        }

        private class DamageEventResult
        {
            public Dictionary<string, float> NumericParameters = new Dictionary<string, float>();
            public List<ImpactTemplate> Impacts = new List<ImpactTemplate>();

            public DamageEventResult(float trapHealth)
            {
                NumericParameters.Add(StaticParameterTranslator.CURRENT_HEALTH, trapHealth);
                NumericParameters.Add(StaticParameterTranslator.HEALTH, 0);
            }

            public void ChangeParameter(string key, float value)
            {
                if (!NumericParameters.ContainsKey(key))
                {
                    NumericParameters.Add(key, 0f);
                }
                NumericParameters[key] += value;
            }

            public void Finish()
            {
                NumericParameters.Remove(StaticParameterTranslator.CURRENT_HEALTH);
            }
        }
    }
}