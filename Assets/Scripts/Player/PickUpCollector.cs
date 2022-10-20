using System.Diagnostics;

namespace Player
{
    public class PickUpCollector
    {
        private int pickUpCollection;
        public int maxPickUpsHeld;

        public PickUpCollector(int maxPickUpsHeld)
        {
            this.maxPickUpsHeld = maxPickUpsHeld;
        }

        public int PickUpCollection
        {
            get => pickUpCollection;
            set => pickUpCollection = value;
        }

        public void Reset()
        {
            PickUpCollection = 0;
        }
    }
}