
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Intersect_Editor.Classes.Core;
using Intersect_Editor.Classes.Maps;
using Intersect_Editor.Forms;
using Intersect_Library.Localization;

namespace Intersect_Editor.Classes
{
    public static class EditorLoop
    {
        private static int _fps = 0;
        private static int _fpsCount = 0;
        private static long _fpsTime = 0;
        private static frmMain myForm;
        private static long animationTimer = Globals.System.GetTimeMs();
        private static long waterfallTimer = Globals.System.GetTimeMs();
        private static Thread mapThread;
        private static frmProgress progressForm;
        public static void StartLoop()
        {
            Globals.MainForm.Visible = true;
            Globals.MainForm.EnterMap(Globals.CurrentMap == null ? 0 : Globals.CurrentMap.GetId());
            myForm = Globals.MainForm;

            if (mapThread == null)

            {
                mapThread = new Thread(UpdateMaps);
                mapThread.Start();
                // drawing loop
                while (myForm.Visible) // loop while the window is open
                {
                    RunFrame();
                }
            }
        }

        public static void RunFrame()
        {
            //Shooting for 30fps
            var startTime = Globals.System.GetTimeMs();
            myForm.Update();

            if (waterfallTimer < Globals.System.GetTimeMs())
            {
                Globals.WaterfallFrame++;
                if (Globals.WaterfallFrame == 3) { Globals.WaterfallFrame = 0; }
                waterfallTimer = Globals.System.GetTimeMs() + 500;
            }
            if (animationTimer < Globals.System.GetTimeMs())
            {
                Globals.AutotileFrame++;
                if (Globals.AutotileFrame == 3) { Globals.AutotileFrame = 0; }
                animationTimer = Globals.System.GetTimeMs() + 600;
            }
            //Check Editors
            if (Globals.ResourceEditor != null && Globals.ResourceEditor.IsDisposed == false)
            {
                Globals.ResourceEditor.Render();
            }
            EditorGraphics.Render();
            GameContentManager.Update();
            Network.Update();
            Application.DoEvents(); // handle form events

            _fpsCount++;
            if (_fpsTime < Globals.System.GetTimeMs())
            {
                _fps = _fpsCount;
                myForm.toolStripLabelFPS.Text = Strings.Get("mainform","fps",_fps);
                _fpsCount = 0;
                _fpsTime = Globals.System.GetTimeMs() + 1000;
            }
            Thread.Sleep(Math.Max(1, (int)(1000 / 60f - (Globals.System.GetTimeMs() - startTime))));
        }

        private static void UpdateMaps()
        {
            while (!Globals.ClosingEditor)
            {
                if (Globals.MapsToScreenshot.Count > 0 && Globals.FetchingMapPreviews == false && myForm.InvokeRequired)
                {
                    if (progressForm == null || progressForm.IsDisposed ||
                        progressForm.Visible == false)
                    {
                        progressForm = new frmProgress();
                        progressForm.SetTitle("Saving Map Cache");
                        new Task((() => progressForm.ShowDialog())).Start();
                        while (Globals.MapsToScreenshot.Count > 0)
                        {
                            var maps = MapInstance.GetObjects();
                            foreach (var map in maps)
                            {
                                if (!myForm.Disposing && progressForm.IsHandleCreated) progressForm.BeginInvoke((Action)(() =>progressForm.SetProgress(Globals.MapsToScreenshot.Count + " maps remaining.", -1,false)));
                                if (map.Value != null)
                                {
                                    map.Value.Update();
                                }
                                Network.Update();
                                Application.DoEvents();
                            }
                            Thread.Sleep(50);
                        }
                        Globals.MapGrid.ResetForm();
                        progressForm.BeginInvoke(new Action(() => progressForm.Close()));
                    }
                }
                Thread.Sleep(100);
            }
        }
    }
}
