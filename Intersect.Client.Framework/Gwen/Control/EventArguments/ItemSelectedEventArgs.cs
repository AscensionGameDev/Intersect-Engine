using System;

namespace Intersect.Client.Framework.Gwen.Control.EventArguments
{

    public class ItemSelectedEventArgs : EventArgs
    {

        internal ItemSelectedEventArgs(Base selecteditem, bool automated = false)
        {
            this.SelectedItem = selecteditem;
            this.Automated = automated;
        }

        public Base SelectedItem { get; private set; }

        public bool Automated { get; private set; }

    }

}
