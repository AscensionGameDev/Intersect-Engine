using System;
using System.Collections.Generic;

using Intersect.Client;
using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Mail
{
    public class MailBoxWindow
    {
		private WindowControl mMailBoxWindow;

		private Label mMail;
		private ListBox mMailListBox;


		private Label mSender;
		private Label mTitle;
		private RichLabel mMessage;
		private Label mItem;

		private Button mCloseButton;

		private Button mTakeButton;

		public MailBoxWindow(Canvas gameCanvas)
		{
			mMailBoxWindow = new WindowControl(gameCanvas, Strings.MailBox.title, false, "MailBoxWindow");
			mMailBoxWindow.DisableResizing();
			Interface.InputBlockingElements.Add(mMailBoxWindow);

			mMail = new Label(mMailBoxWindow, "Mail")
			{
				Text = Strings.MailBox.mails
			};

			mMailListBox = new ListBox(mMailBoxWindow, "MailListBox");
			mMailListBox.EnableScroll(false, true);
			mMailListBox.RowSelected += Selected_MailListBox;
			mMailListBox.AllowMultiSelect = false;
			mSender = new Label(mMailBoxWindow, "Sender");
			mSender.Hide();
			mTitle = new Label(mMailBoxWindow, "Title");
			mTitle.Hide();
			mMessage = new RichLabel(mMailBoxWindow, "Message");
			mMessage.Hide();

			mItem = new Label(mMailBoxWindow, "Item");
			mItem.Hide();


			mTakeButton = new Button(mMailBoxWindow, "TakeButton");
			mTakeButton.SetText(Strings.MailBox.take);
			mTakeButton.Clicked += Take_Clicked;
			mTakeButton.Hide();

			mCloseButton = new Button(mMailBoxWindow, "CloseButton");
			mCloseButton.SetText(Strings.MailBox.close);
			mCloseButton.Clicked += CloseButton_Clicked;

			mMailBoxWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
		}

		private void Take_Clicked(Base sender, ClickedEventArgs e)
		{
			var selected = mMailListBox.SelectedRow;
			Client.Mail mail = selected.UserData as Client.Mail;
			PacketSender.SendTakeMail(mail.MailID);
		}

		private void Selected_MailListBox(Base sender, ItemSelectedEventArgs e)
		{
			if (Globals.Mails.Count > 0)
			{
				var selected = mMailListBox.SelectedRow;
				Client.Mail mail = selected.UserData as Client.Mail;
				mSender.Text = $"{Strings.MailBox.sender}: {mail.SenderName}";
				mTitle.Text = $"{Strings.MailBox.mailtitle}: {mail.Name}";
				mMessage.ClearText();
				mMessage.AddText($"{mail.Message}", Color.White);
				mSender.Show();
				mTitle.Show();
				mMessage.Show();
				if (mail.Item != Guid.Empty)
				{
					var item = ItemBase.Get(mail.Item);
					if (item.IsStackable)
					{
						mItem.Text = $"{Strings.MailBox.mailitem}: [{mail.Quantity}] {item.Name}";
					}
					else
					{
						mItem.Text = $"{Strings.MailBox.mailitem}: {item.Name}";
					}
					mItem.Show();
				}
				else
				{
					mItem.Hide();
				}
				mTakeButton.Show();
			}
			else
			{
				mSender.Hide();
				mTitle.Hide();
				mMessage.Hide();
				mItem.Hide();
				mTakeButton.Hide();
			}
		}

		void CloseButton_Clicked(Base sender, ClickedEventArgs e)
		{
			PacketSender.SendCloseMail();
		}

		public void UpdateMail()
		{
			mMailListBox.RemoveAllRows();
			mMailListBox.ScrollToTop();
			foreach (Client.Mail mail in Globals.Mails)
			{
				var row = mMailListBox.AddRow(mail.Name.Trim(), "", mail);
				row.SetTextColor(Color.White);
			}
			if (Globals.Mails.Count > 0)
			{
				mMailListBox.SelectByUserData(Globals.Mails[0]);
			}
			else
			{
				mSender.Hide();
				mTitle.Hide();
				mMessage.Hide();
				mItem.Hide();
				mTakeButton.Hide();
			}
		}

		public void Close()
		{
			mMailBoxWindow.Close();
		}

		public bool IsVisible()
		{
			return !mMailBoxWindow.IsHidden;
		}

		public void Hide()
		{
			mMailBoxWindow.IsHidden = true;
		}
	}
}
