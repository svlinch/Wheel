using Assets.Scripts.Utility;
using Assets.Scripts.GameData;

namespace Assets.Scripts.Models
{
    public class ImpactModel
    {
        private ImpactTemplate _template;
        private GameTimer _durationTimer;
        private GameTimer _tickTimer;

        public ImpactModel(ImpactTemplate template)
        {
            _template = template;
            _durationTimer = new GameTimer(_template.Duration);
            _tickTimer = new GameTimer(_template.Tick);
        }

        public int GetValue()
        {
            return _template.Value;
        }

        public (bool, bool) Checkout(float dTime)
        {
            var durationResult = _durationTimer.HandleUpdate(dTime);
            var tickResult = _tickTimer.HandleUpdate(dTime);
            if (tickResult)
            {
                _tickTimer.CheckoutTimer(_template.Tick);
            }
            return (durationResult, tickResult);
        }
    }
}