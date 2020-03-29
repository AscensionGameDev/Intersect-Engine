using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;

namespace Intersect.Client.Core.Controls
{

    public class ControlMap
    {

        public Keys Key1;

        public Keys Key2;

        public ControlMap(Control control, Keys key1, Keys key2)
        {
            this.Key1 = key1;
            this.Key2 = key2;
        }

        public bool KeyDown()
        {
            if (Key1 != Keys.None && Globals.InputManager.KeyDown(Key1))
            {
                return true;
            }

            if (Key2 != Keys.None && Globals.InputManager.KeyDown(Key2))
            {
                return true;
            }

            if (Interface.Interface.MouseHitGui())
            {
                return false;
            }

            switch (Key1)
            {
                case Keys.LButton:
                    if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
                    {
                        return true;
                    }

                    break;
                case Keys.RButton:
                    if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Right))
                    {
                        return true;
                    }

                    break;
                case Keys.MButton:
                    if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Middle))
                    {
                        return true;
                    }

                    break;
            }

            switch (Key2)
            {
                case Keys.LButton:
                    if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
                    {
                        return true;
                    }

                    break;
                case Keys.RButton:
                    if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Right))
                    {
                        return true;
                    }

                    break;
                case Keys.MButton:
                    if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Middle))
                    {
                        return true;
                    }

                    break;
            }

            return false;
        }

    }

}
