using Intersect.Client.Framework.Input;
using Intersect.Client.General;

namespace Intersect.Client.Core.Controls
{

    public partial class ControlMap
    {
        public ControlValue Key1;

        public ControlValue Key2;

        public ControlMap(ControlValue key1, ControlValue key2)
        {
            this.Key1 = key1;
            this.Key2 = key2;
        }

        public bool KeyDown()
        {
            if (Interface.Interface.MouseHitGui() && (Globals.InputManager.MouseButtonDown(MouseButtons.Right) ||
                                                      Globals.InputManager.MouseButtonDown(MouseButtons.Left)))
            {
                return false;
            }

            return Key1.IsDown() || Key2.IsDown();
        }

    }

}
