using Assets.Scripts.Events;

namespace Assets.Scripts.Units
{
    public class Foundament : SimpleTrap
    {
        public override void HandleClick()
        {
            _eventService.SendMessage(new BaseElementClicked(_indexInList, null));
        }
    }
}