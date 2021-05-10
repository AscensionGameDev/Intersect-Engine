using System;
using System.Windows.Forms;

using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.Logging;

namespace Intersect.Editor.Forms.Editors
{

    public class EditorForm : Form
    {

        private bool mClosing = false;

        protected EditorForm()
        {
            ApplyHooks();

            this.Icon = Properties.Resources.Icon;
        }

        protected void ApplyHooks()
        {
            PacketHandler.GameObjectUpdatedDelegate = type =>
            {
                if (IsDisposed || mClosing || Disposing)
                {
                    return;
                }

                var action = (Action<GameObjectType>) FireGameObjectUpdatedDelegate;
                try
                {
                    if (!this.Disposing && !this.IsDisposed)
                    {
                        if (InvokeRequired)
                        {
                            Invoke(action, type);
                        }
                        else
                        {
                            action(type);
                        }
                    }
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
            mClosing = true;
        }

        private void FireGameObjectUpdatedDelegate(GameObjectType type)
        {
            if (IsDisposed || mClosing || Disposing)
            {
                return;
            }

            GameObjectUpdatedDelegate(type);
        }

        protected virtual void GameObjectUpdatedDelegate(GameObjectType type)
        {
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // EditorForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "EditorForm";
            this.ResumeLayout(false);

        }

    }

}
