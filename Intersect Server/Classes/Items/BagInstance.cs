namespace Intersect_Server.Classes.Items
{
    public class BagInstance
    {
        public int Slots = 0;
        public ItemInstance[] Items = null;

        public BagInstance(int slots)
        {
            Slots = slots;
            Items = new ItemInstance[slots];
            for (int i = 0; i < slots; i++)
            {
                Items[i] = new ItemInstance(-1,-1, -1);
            }
        }


        public BagInstance Clone()
        {
            var bag = new BagInstance(Slots);
            for (int i = 0; i < Slots; i++)
            {
                if (Items[i] != null)
                {
                    bag.Items[i] = Items[i].Clone();
                }
            }
            return bag;
        }
    }
}
