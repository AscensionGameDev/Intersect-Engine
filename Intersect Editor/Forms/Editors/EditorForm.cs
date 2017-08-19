using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.Logging;

namespace Intersect.Editor.Forms.Editors
{
    public class EditorForm : Form
    {
        private bool closing = false;
        protected EditorForm()
        {
            ApplyHooks();
        }

        protected void ApplyHooks()
        {
            PacketHandler.GameObjectUpdatedDelegate = type =>
            {
                if (IsDisposed || closing || Disposing) return;
                var action = (Action<GameObjectType>) FireGameObjectUpdatedDelegate;
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
            this.Closing += EditorForm_Closing;
        }

        private void EditorForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closing = true;
        }

        private void FireGameObjectUpdatedDelegate(GameObjectType type)
        {
            if (IsDisposed || closing || Disposing) return;
            GameObjectUpdatedDelegate(type);
        }

        protected virtual void GameObjectUpdatedDelegate(GameObjectType type)
        {
        }
    }
}