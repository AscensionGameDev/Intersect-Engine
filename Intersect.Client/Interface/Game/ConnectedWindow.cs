using System;

using Intersect.Client.Core;
using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;

namespace Intersect.Client.Interface.Game
{
	class ConnectedWindow
	{
		private WindowControl mConnectedWindow;
		private ListBox mConnected;

		private bool isKeyDown;

		public ConnectedWindow(Canvas gameCanvas)
		{
			mConnectedWindow = new WindowControl(gameCanvas, Strings.Connected.title, false, "ConnectedWindow");
			mConnectedWindow.DisableResizing();

			mConnected = new ListBox(mConnectedWindow, "ConnectedList");

			UpdateList();

			mConnectedWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
		}

		public void Update()
		{
			if (mConnectedWindow.IsHidden)
			{
				return;
			}
		}

		public void Show()
		{
			mConnectedWindow.Show();
		}

		public bool IsVisible()
		{
			return !mConnectedWindow.IsHidden;
		}

		public void Hide()
		{
			mConnectedWindow.Hide();
		}

		public void UpdateList()
		{
			//Clear previous instances if already existing
			if (mConnected != null)
			{
				mConnected.Clear();
			}
			foreach (var f in Globals.Me.OnlinePlayer)
			{
				var row = mConnected.AddRow(f);
				row.UserData = f;
				row.SetTextColor(Color.Green);
			}
			
		}
	}
}
