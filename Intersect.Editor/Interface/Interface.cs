using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Gwen.Skin;

using Base = Intersect.Client.Framework.Gwen.Renderer.Base;

namespace Intersect.Editor.Interface
{

    public static class Interface
    {

        public static readonly List<KeyValuePair<string, string>> MsgboxErrors =
            new List<KeyValuePair<string, string>>();

        //GWEN GUI
        public static bool GwenInitialized;

        public static InputBase GwenInput;

        public static Base GwenRenderer;

        public static bool HideUi;

        private static Canvas sGameCanvas;

        private static Canvas sMenuCanvas;

        public static bool SetupHandlers { get; set; }

        public static TexturedBase Skin { get; set; }

        //Input Handling
        public static List<Client.Framework.Gwen.Control.Base> FocusElements { get; set; }

        public static List<Client.Framework.Gwen.Control.Base> InputBlockingElements { get; set; }

        #region "Gwen Setup and Input"

        //Gwen Low Level Functions
        public static void InitGwen()
        {
        }

        public static void DestroyGwen(bool exiting = false)
        {
        }

        public static bool HasInputFocus()
        {
            if (FocusElements == null || InputBlockingElements == null)
            {
                return false;
            }

            return FocusElements.Any(t => t.MouseInputEnabled && (t?.HasFocus ?? false)) || InputBlockingElements.Any(t => t?.IsHidden == false);
        }

        #endregion

        #region "GUI Functions"

        //Actual Drawing Function
        public static void DrawGui()
        {
        }

        public static void ToggleInput(bool val)
        {
        }

        public static bool MouseHitGui()
        {
            return false;
        }

        public static bool MouseHitBase(Client.Framework.Gwen.Control.Base obj)
        {
            return false;
        }

        public static string[] WrapText(string input, int width, GameFont font)
        {
            return Array.Empty<string>();
        }

        #endregion

        public static Client.Framework.Gwen.Control.Base FindControlAtCursor()
        {
            return default;
        }
    }
}
