using Intersect.Client.General;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Input;

namespace Intersect.Client.Core.Controls
{
    public partial class ControlValue
    {
        public static ControlValue Default => new ControlValue(Keys.None, Keys.None);

        public Keys Modifier { get; set; }

        public Keys Key { get; set; }

        public bool IsMouseKey => Key == Keys.LButton || Key == Keys.RButton || Key == Keys.MButton;

        public ControlValue(Keys modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
        }

        public bool IsDown()
        {
            // Assume our modifier is clicked.
            var modifier = true;

            // Check to see if it actually is, if not disable it!
            if (Modifier != Keys.None && !KeyDown(Modifier))
            {
                modifier = false;
            }

            // Check to see if our modifier and real key are pressed!
            if (modifier)
            {
                if (IsMouseKey)
                {
                    switch (Key)
                    {
                        case Keys.LButton:
                            if (Globals.InputManager.MouseButtonDown(MouseButtons.Left))
                            {
                                return true;
                            }

                            break;
                        case Keys.RButton:
                            if (Globals.InputManager.MouseButtonDown(MouseButtons.Right))
                            {
                                return true;
                            }

                            break;
                        case Keys.MButton:
                            if (Globals.InputManager.MouseButtonDown(MouseButtons.Middle))
                            {
                                return true;
                            }

                            break;
                    }
                }
                else
                {
                    if (KeyDown(Key))
                    {
                        return true;
                    }
                }
            }
           
            // If we get this far, clearly our buttons aren't pressed.
            return false;
        }

        private bool KeyDown(Keys key) => Globals.InputManager.KeyDown(key);
    }
}
