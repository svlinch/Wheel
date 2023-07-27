namespace Assets.Scripts.Events
{
    public class BuildResourcesChanged
    {
        public int Number;

        public BuildResourcesChanged(int newNumber)
        {
            Number = newNumber;
        }
    }
}