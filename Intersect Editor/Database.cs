using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Editor
{
    public static class Database
    {
        public static void InitDatabase()
        {
            InitItems();
        }
        private static void InitItems()
        {
            Globals.Items = new Item[Constants.MAX_ITEMS];
        }
    }
}
