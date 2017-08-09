using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.Logging;

namespace Intersect.Editor.Forms.Editors
{
    public class EditorForm : Form
    {

        protected EditorForm()
        {
            ApplyHooks();
        }

        protected void ApplyHooks()
        {
            PacketHandler.GameObjectUpdatedDelegate = type =>
            {
                if (IsDisposed) return;
                var action = (Action<GameObjectType>)FireGameObjectUpdatedDelegate;
                try
                {
                    if (InvokeRequired) Invoke(action, type);
                    else action(type);
                }
                catch (Exception e)
                {
                    Log.Debug(e);
                }
            };
        }

        private void FireGameObjectUpdatedDelegate(GameObjectType type)
        {
            if (IsDisposed) return;
            GameObjectUpdatedDelegate(type);
        }

        protected virtual void GameObjectUpdatedDelegate(GameObjectType type)
        {

        }
    }
}