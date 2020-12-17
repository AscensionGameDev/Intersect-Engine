using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Menu
{

    public class CreateCharacterWindow
    {

        private Button mBackButton;

        private ImagePanel mCharacterContainer;

        private ImagePanel mCharacterNameBackground;

        private ImagePanel mCharacterPortrait;

        //Image
        private string mCharacterPortraitImg = "";

        private Label mCharCreationHeader;

        //Controls
        private ImagePanel mCharCreationPanel;

        private Label mCharnameLabel;

        private TextBox mCharnameTextbox;

        private ImagePanel mClassBackground;

        private ComboBox mClassCombobox;

        private Label mClassLabel;

        private Button mCreateButton;

        private int mDisplaySpriteIndex = -1;

        private LabeledCheckBox mFemaleChk;

        private List<KeyValuePair<int, ClassSprite>> mFemaleSprites = new List<KeyValuePair<int, ClassSprite>>();

        private ImagePanel mGenderBackground;

        private Label mGenderLabel;

        private Label mHint2Label;

        private Label mHintLabel;

        //Parent
        private MainMenu mMainMenu;

        private LabeledCheckBox mMaleChk;

        //Class Info
        private List<KeyValuePair<int, ClassSprite>> mMaleSprites = new List<KeyValuePair<int, ClassSprite>>();

        private Button mNextSpriteButton;

        private Button mPrevSpriteButton;

        private SelectCharacterWindow mSelectCharacterWindow;

        //Init
        public CreateCharacterWindow(
            Canvas parent,
            MainMenu mainMenu,
            ImagePanel parentPanel,
            SelectCharacterWindow selectCharacterWindow
        )
        {
            //Assign References
            mMainMenu = mainMenu;
            mSelectCharacterWindow = selectCharacterWindow;

            //Main Menu Window
            mCharCreationPanel = new ImagePanel(parent, "CharacterCreationWindow");
            mCharCreationPanel.IsHidden = true;

            //Menu Header
            mCharCreationHeader = new Label(mCharCreationPanel, "CharacterCreationHeader");
            mCharCreationHeader.SetText(Strings.CharacterCreation.title);

            //Character Name Background
            mCharacterNameBackground = new ImagePanel(mCharCreationPanel, "CharacterNamePanel");

            //Character name Label
            mCharnameLabel = new Label(mCharacterNameBackground, "CharacterNameLabel");
            mCharnameLabel.SetText(Strings.CharacterCreation.name);

            //Character name Textbox
            mCharnameTextbox = new TextBox(mCharacterNameBackground, "CharacterNameField");
            mCharnameTextbox.SubmitPressed += CharnameTextbox_SubmitPressed;

            //Class Background
            mClassBackground = new ImagePanel(mCharCreationPanel, "ClassPanel");

            //Class Label
            mClassLabel = new Label(mClassBackground, "ClassLabel");
            mClassLabel.SetText(Strings.CharacterCreation.Class);

            //Class Combobox
            mClassCombobox = new ComboBox(mClassBackground, "ClassCombobox");
            mClassCombobox.ItemSelected += classCombobox_ItemSelected;

            //Hint Label
            mHintLabel = new Label(mCharCreationPanel, "HintLabel");
            mHintLabel.SetText(Strings.CharacterCreation.hint);
            mHintLabel.IsHidden = true;

            //Hint2 Label
            mHint2Label = new Label(mCharCreationPanel, "Hint2Label");
            mHint2Label.SetText(Strings.CharacterCreation.hint2);
            mHint2Label.IsHidden = true;

            //Character Container
            mCharacterContainer = new ImagePanel(mCharCreationPanel, "CharacterContainer");

            //Character sprite
            mCharacterPortrait = new ImagePanel(mCharacterContainer, "CharacterPortait");
            mCharacterPortrait.SetSize(48, 48);

            //Next Sprite Button
            mNextSpriteButton = new Button(mCharacterContainer, "NextSpriteButton");
            mNextSpriteButton.Clicked += _nextSpriteButton_Clicked;

            //Prev Sprite Button
            mPrevSpriteButton = new Button(mCharacterContainer, "PreviousSpriteButton");
            mPrevSpriteButton.Clicked += _prevSpriteButton_Clicked;

            //Class Background
            mGenderBackground = new ImagePanel(mCharCreationPanel, "GenderPanel");

            //Gender Label
            mGenderLabel = new Label(mGenderBackground, "GenderLabel");
            mGenderLabel.SetText(Strings.CharacterCreation.gender);

            //Male Checkbox
            mMaleChk = new LabeledCheckBox(mGenderBackground, "MaleCheckbox")
            {
                Text = Strings.CharacterCreation.male
            };

            mMaleChk.IsChecked = true;
            mMaleChk.Checked += maleChk_Checked;
            mMaleChk.UnChecked += femaleChk_Checked; // If you notice this, feel free to hate us ;)

            //Female Checkbox
            mFemaleChk = new LabeledCheckBox(mGenderBackground, "FemaleCheckbox")
            {
                Text = Strings.CharacterCreation.female
            };

            mFemaleChk.Checked += femaleChk_Checked;
            mFemaleChk.UnChecked += maleChk_Checked;

            //Register - Send Registration Button
            mCreateButton = new Button(mCharCreationPanel, "CreateButton");
            mCreateButton.SetText(Strings.CharacterCreation.create);
            mCreateButton.Clicked += CreateButton_Clicked;

            mBackButton = new Button(mCharCreationPanel, "BackButton");
            mBackButton.IsHidden = true;
            mBackButton.SetText(Strings.CharacterCreation.back);
            mBackButton.Clicked += BackButton_Clicked;

            mCharCreationPanel.LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer.GetResolutionString());
        }

        public bool IsHidden => mCharCreationPanel.IsHidden;

        public void Init()
        {
            mClassCombobox.DeleteAll();
            var classCount = 0;
            foreach (ClassBase cls in ClassBase.Lookup.Values)
            {
                if (!cls.Locked)
                {
                    mClassCombobox.AddItem(cls.Name);
                    classCount++;
                }
            }

            LoadClass();
            UpdateDisplay();
        }

        public void Update()
        {
            if (!Networking.Network.Connected)
            {
                Hide();
                mMainMenu.Show();
                Interface.MsgboxErrors.Add(new KeyValuePair<string, string>("", Strings.Errors.lostconnection));

                return;
            }

            // Re-Enable our buttons if we're not waiting for the server anymore with it disabled.
            if (!Globals.WaitingOnServer && mCreateButton.IsDisabled)
            {
                mCreateButton.Enable();
            }
        }

        //Methods
        private void UpdateDisplay()
        {
            var isFace = true;
            if (GetClass() != null && mDisplaySpriteIndex != -1)
            {
                mCharacterPortrait.IsHidden = false;
                if (GetClass().Sprites.Count > 0)
                {
                    if (mMaleChk.IsChecked)
                    {
                        mCharacterPortrait.Texture = Globals.ContentManager.GetTexture(
                            GameContentManager.TextureType.Face, mMaleSprites[mDisplaySpriteIndex].Value.Face
                        );

                        if (mCharacterPortrait.Texture == null)
                        {
                            mCharacterPortrait.Texture = Globals.ContentManager.GetTexture(
                                GameContentManager.TextureType.Entity, mMaleSprites[mDisplaySpriteIndex].Value.Sprite
                            );

                            isFace = false;
                        }
                    }
                    else
                    {
                        mCharacterPortrait.Texture = Globals.ContentManager.GetTexture(
                            GameContentManager.TextureType.Face, mFemaleSprites[mDisplaySpriteIndex].Value.Face
                        );

                        if (mCharacterPortrait.Texture == null)
                        {
                            mCharacterPortrait.Texture = Globals.ContentManager.GetTexture(
                                GameContentManager.TextureType.Entity, mFemaleSprites[mDisplaySpriteIndex].Value.Sprite
                            );

                            isFace = false;
                        }
                    }

                    if (mCharacterPortrait.Texture != null)
                    {
                        if (isFace)
                        {
                            mCharacterPortrait.SetTextureRect(
                                0, 0, mCharacterPortrait.Texture.GetWidth(), mCharacterPortrait.Texture.GetHeight()
                            );

                            var scale = Math.Min(
                                mCharacterContainer.InnerWidth / (double) mCharacterPortrait.Texture.GetWidth(),
                                mCharacterContainer.InnerHeight / (double) mCharacterPortrait.Texture.GetHeight()
                            );

                            mCharacterPortrait.SetSize(
                                (int) (mCharacterPortrait.Texture.GetWidth() * scale),
                                (int) (mCharacterPortrait.Texture.GetHeight() * scale)
                            );

                            mCharacterPortrait.SetPosition(
                                mCharacterContainer.Width / 2 - mCharacterPortrait.Width / 2,
                                mCharacterContainer.Height / 2 - mCharacterPortrait.Height / 2
                            );
                        }
                        else
                        {
                            mCharacterPortrait.SetTextureRect(
                                0, 0, mCharacterPortrait.Texture.GetWidth() / Options.Instance.Sprites.NormalFrames,
                                mCharacterPortrait.Texture.GetHeight() / Options.Instance.Sprites.Directions
                            );

                            mCharacterPortrait.SetSize(
                                mCharacterPortrait.Texture.GetWidth() / Options.Instance.Sprites.NormalFrames, mCharacterPortrait.Texture.GetHeight() / Options.Instance.Sprites.Directions
                            );

                            mCharacterPortrait.SetPosition(
                                mCharacterContainer.Width / 2 - mCharacterPortrait.Width / 2,
                                mCharacterContainer.Height / 2 - mCharacterPortrait.Height / 2
                            );
                        }
                    }
                }
            }
            else
            {
                mCharacterPortrait.IsHidden = true;
            }
        }

        public void Show()
        {
            mCharCreationPanel.Show();
        }

        public void Hide()
        {
            mCharCreationPanel.Hide();
        }

        private ClassBase GetClass()
        {
            if (mClassCombobox.SelectedItem == null)
            {
                return null;
            }

            foreach (var cls in ClassBase.Lookup)
            {
                if (mClassCombobox.SelectedItem.Text == cls.Value.Name && !((ClassBase) cls.Value).Locked)
                {
                    return (ClassBase) cls.Value;
                }
            }

            return null;
        }

        private void LoadClass()
        {
            var cls = GetClass();
            mMaleSprites.Clear();
            mFemaleSprites.Clear();
            mDisplaySpriteIndex = -1;
            if (cls != null)
            {
                for (var i = 0; i < cls.Sprites.Count; i++)
                {
                    if (cls.Sprites[i].Gender == 0)
                    {
                        mMaleSprites.Add(new KeyValuePair<int, ClassSprite>(i, cls.Sprites[i]));
                    }
                    else
                    {
                        mFemaleSprites.Add(new KeyValuePair<int, ClassSprite>(i, cls.Sprites[i]));
                    }
                }
            }

            ResetSprite();
        }

        private void ResetSprite()
        {
            mNextSpriteButton.IsHidden = true;
            mPrevSpriteButton.IsHidden = true;
            if (mMaleChk.IsChecked)
            {
                if (mMaleSprites.Count > 0)
                {
                    mDisplaySpriteIndex = 0;
                    if (mMaleSprites.Count > 1)
                    {
                        mNextSpriteButton.IsHidden = false;
                        mPrevSpriteButton.IsHidden = false;
                    }
                }
                else
                {
                    mDisplaySpriteIndex = -1;
                }
            }
            else
            {
                if (mFemaleSprites.Count > 0)
                {
                    mDisplaySpriteIndex = 0;
                    if (mFemaleSprites.Count > 1)
                    {
                        mNextSpriteButton.IsHidden = false;
                        mPrevSpriteButton.IsHidden = false;
                    }
                }
                else
                {
                    mDisplaySpriteIndex = -1;
                }
            }
        }

        private void _prevSpriteButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mDisplaySpriteIndex--;
            if (mMaleChk.IsChecked)
            {
                if (mMaleSprites.Count > 0)
                {
                    if (mDisplaySpriteIndex == -1)
                    {
                        mDisplaySpriteIndex = mMaleSprites.Count - 1;
                    }
                }
                else
                {
                    mDisplaySpriteIndex = -1;
                }
            }
            else
            {
                if (mFemaleSprites.Count > 0)
                {
                    if (mDisplaySpriteIndex == -1)
                    {
                        mDisplaySpriteIndex = mFemaleSprites.Count - 1;
                    }
                }
                else
                {
                    mDisplaySpriteIndex = -1;
                }
            }

            UpdateDisplay();
        }

        private void _nextSpriteButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mDisplaySpriteIndex++;
            if (mMaleChk.IsChecked)
            {
                if (mMaleSprites.Count > 0)
                {
                    if (mDisplaySpriteIndex >= mMaleSprites.Count)
                    {
                        mDisplaySpriteIndex = 0;
                    }
                }
                else
                {
                    mDisplaySpriteIndex = -1;
                }
            }
            else
            {
                if (mFemaleSprites.Count > 0)
                {
                    if (mDisplaySpriteIndex >= mFemaleSprites.Count)
                    {
                        mDisplaySpriteIndex = 0;
                    }
                }
                else
                {
                    mDisplaySpriteIndex = -1;
                }
            }

            UpdateDisplay();
        }

        void TryCreateCharacter(int gender)
        {
            if (Globals.WaitingOnServer || mDisplaySpriteIndex == -1)
            {
                return;
            }

            if (FieldChecking.IsValidUsername(mCharnameTextbox.Text, Strings.Regex.username))
            {
                if (mMaleChk.IsChecked)
                {
                    PacketSender.SendCreateCharacter(
                        mCharnameTextbox.Text, GetClass().Id, mMaleSprites[mDisplaySpriteIndex].Key
                    );
                }
                else
                {
                    PacketSender.SendCreateCharacter(
                        mCharnameTextbox.Text, GetClass().Id, mFemaleSprites[mDisplaySpriteIndex].Key
                    );
                }

                Globals.WaitingOnServer = true;
                mCreateButton.Disable();
                ChatboxMsg.ClearMessages();
            }
            else
            {
                Interface.MsgboxErrors.Add(new KeyValuePair<string, string>("", Strings.CharacterCreation.invalidname));
            }
        }

        //Input Handlers
        void CharnameTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            if (mMaleChk.IsChecked == true)
            {
                TryCreateCharacter(0);
            }
            else
            {
                TryCreateCharacter(1);
            }
        }

        void classCombobox_ItemSelected(Base control, ItemSelectedEventArgs args)
        {
            LoadClass();
            UpdateDisplay();
        }

        void maleChk_Checked(Base sender, EventArgs arguments)
        {
            mMaleChk.IsChecked = true;
            mFemaleChk.IsChecked = false;
            ResetSprite();
            UpdateDisplay();
        }

        void femaleChk_Checked(Base sender, EventArgs arguments)
        {
            mFemaleChk.IsChecked = true;
            mMaleChk.IsChecked = false;
            ResetSprite();
            UpdateDisplay();
        }

        void CreateButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.WaitingOnServer)
            {
                return;
            }

            if (mMaleChk.IsChecked == true)
            {
                TryCreateCharacter(0);
            }
            else
            {
                TryCreateCharacter(1);
            }
        }

        private void BackButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            if (Options.Player.MaxCharacters <= 1)
            {
                //Logout
                mMainMenu.Show();
            }
            else
            {
                //Character Selection Screen
                mSelectCharacterWindow.Show();
            }
        }

    }

}
