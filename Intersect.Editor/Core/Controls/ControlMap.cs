using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Input;
using Intersect.Editor.General;

namespace Intersect.Editor.Core.Controls
{

    public class ControlMap
    {
        public ControlValue Key1;

        public ControlValue Key2;

        public ControlMap(Control control, ControlValue key1, ControlValue key2)
        {
            this.Key1 = key1;
            this.Key2 = key2;
        }

        public bool KeyDown()
        {
            if (Key1.IsMouseKey || Key2.IsMouseKey)
            {
                if (Interface.Interface.MouseHitGui())
                {
                    return false;
                }
            }

            if (Key1.IsDown())
            {
                return true;
            }

            if (Key2.IsDown())
            {
                return true;
            }

            return false;
        }

    }

}
