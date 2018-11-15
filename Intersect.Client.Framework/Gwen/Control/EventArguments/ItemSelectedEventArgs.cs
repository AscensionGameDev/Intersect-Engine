using System;

namespace Intersect.Client.Framework.Gwen.Control.EventArguments
{
    public class ItemSelectedEventArgs : EventArgs
    {
        internal ItemSelectedEventArgs(Base selecteditem)
        {
            this.SelectedItem = selecteditem;
        }

        public Base SelectedItem { get; private set; }
    }
}