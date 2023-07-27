using System.Collections.Generic;
using Assets.Scripts.GameData;
using Assets.Scripts.Models;

namespace Assets.Scripts.Units
{
    public class ImmortalTrap : SimpleTrap
    {
        public override void SetModel(UnitModel model)
        {
            _model = model;
        }

        public override void HandleUpdate(float dTime)
        {
            UpdateWeapons(dTime);
        }

        public override void SetBattleState(bool state)
        {
            _statusCanvas.ChangeState(false);
            if (state)
            {
                CheckoutWeapons();
            }
        }

        public override void HandleDamageEvent(Dictionary<string, float> numeric, List<ImpactTemplate> impacts)
        {
        }

        public override bool RemoveCheck()
        {
            return false;
        }
    }
}