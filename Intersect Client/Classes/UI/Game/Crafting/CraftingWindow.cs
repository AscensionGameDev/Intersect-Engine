using System;
using System.Collections.Generic;
using System.IO;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.ControlInternal;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI.Game.Chat;
using Color = IntersectClientExtras.GenericClasses.Color;
using Intersect.Client.Classes.UI.Game.Crafting;

namespace Intersect_Client.Classes.UI.Game
{
    public class CraftingWindow
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;
        private ImagePanel _bar;

        private ImagePanel _barContainer;

        private RecipeItem _CombinedItem;
        private Button _craft;

        //Controls
        private WindowControl _craftWindow;
        private ScrollControl _itemContainer;
        private List<RecipeItem> _items = new List<RecipeItem>();
        private Label _lblIngredients;
        private Label _lblProduct;
        private Label _lblRecepies;

        //Objects
        private ListBox _Recepies;
        private List<Label> _values = new List<Label>();

        private long BarTimer;
        private int craftIndex;
        public bool crafting;

        private bool _initialized = false;
        private ImagePanel _itemTemplate;
        private ImagePanel _craftedItemTemplate;

        //Location
        public int X { get { return _craftWindow.X; } }
        public int Y { get { return _craftWindow.Y; } }

        public CraftingWindow(Canvas _gameCanvas)
        {
            _craftWindow = new WindowControl(_gameCanvas, Strings.Get("craftingbench", "title"),false,"CraftingWindow");
            _craftWindow.DisableResizing();

            _itemContainer = new ScrollControl(_craftWindow,"IngredientsContainer");

            //Labels
            _lblRecepies = new Label(_craftWindow,"RecipesTitle");
            _lblRecepies.Text = Strings.Get("craftingbench", "recepies");

            _lblIngredients = new Label(_craftWindow,"IngredientsTitle");
            _lblIngredients.Text = Strings.Get("craftingbench", "ingredients");

            _lblProduct = new Label(_craftWindow,"ProductLabel");
            _lblProduct.Text = Strings.Get("craftingbench", "product");

            //Recepie list
            _Recepies = new ListBox(_craftWindow,"RecipesList");

            //Progress Bar
            _barContainer = new ImagePanel(_craftWindow,"ProgressBarContainer");
            _bar = new ImagePanel(_barContainer,"ProgressBar");

            //Load the craft button
            _craft = new Button(_craftWindow,"CraftButton");
            _craft.SetText(Strings.Get("craftingbench", "craft"));
            _craft.Clicked += craft_Clicked;
            
            Gui.LoadRootUIData(_craftWindow, "InGame.xml");

            Gui.InputBlockingElements.Add(_craftWindow);
        }

        private void LoadCraftItems(int index)
        {
            //Combined item
            craftIndex = index;
            if (_CombinedItem != null)
            {
                _craftWindow.Children.Remove(_CombinedItem.container);
            }
            //Clear the old item description box
            if (_CombinedItem != null && _CombinedItem._descWindow != null)
            {
                _CombinedItem._descWindow.Dispose();
            }
            var craft = Globals.GameBench.Crafts[index];
            if (craft == null) return;
            
            _CombinedItem = new RecipeItem(this, new CraftIngredient(craft.Item, 0))
            {
                container = new ImagePanel(_craftWindow,"CraftedItemContainer")
            };
            _CombinedItem.Setup("CraftedItemIcon");

            //TODO Made this more efficient.
            Gui.LoadRootUIData(_CombinedItem.container, "InGame.xml");

            for (int i = 0; i < _items.Count; i++)
            {
                //Clear the old item description box
                if (_items[i]._descWindow != null)
                {
                    _items[craftIndex]._descWindow.Dispose();
                }
                _itemContainer.RemoveChild(_items[i].container, true);
            }
            _items.Clear();
            _values.Clear();

            for (int i = 0; i < craft.Ingredients.Count; i++)
            {
                _items.Add(new RecipeItem(this, craft.Ingredients[i]));
                _items[i].container = new ImagePanel(_itemContainer, "IngredientItemContainer");
                _items[i].Setup("IngredientItemIcon");

                Label _lblTemp = new Label(_items[i].container,"IngredientItemValue");

                int n = Globals.Me.FindItem(craft.Ingredients[i].Item);
                int x = 0;
                if (n > -1)
                {
                    x = Globals.Me.Inventory[n].ItemVal;
                }
                _lblTemp.Text = x + "/" + craft.Ingredients[i].Quantity;
                _values.Add(_lblTemp);

                //TODO Made this more efficient.
                Gui.LoadRootUIData(_items[i].container, "InGame.xml");

                var xPadding = _items[i].container.Padding.Left + _items[i].container.Padding.Right;
                var yPadding = _items[i].container.Padding.Top + _items[i].container.Padding.Bottom;
                _items[i].container.SetPosition(
                    (i % ((_itemContainer.Width - _itemContainer.GetVerticalScrollBar().Width) / (_items[i].container.Width + xPadding))) * (_items[i].container.Width + xPadding) + xPadding,
                    (i / ((_itemContainer.Width - _itemContainer.GetVerticalScrollBar().Width) / (_items[i].container.Width + xPadding))) * (_items[i].container.Height + yPadding) + yPadding);
            }
        }

        public void Close()
        {
            if (crafting == false)
            {
                _craftWindow.Close();
            }
        }

        public bool IsVisible()
        {
            return !_craftWindow.IsHidden;
        }

        public void Hide()
        {
            if (crafting == false)
            {
                _craftWindow.IsHidden = true;
            }
        }

        //Load new recepie
        void tmpNode_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            if (crafting == false)
            {
                LoadCraftItems(Convert.ToInt32(((ListBoxRow) sender).UserData));
            }
        }

        //Craft the item
        void craft_Clicked(Base sender, ClickedEventArgs arguments)
        {
            foreach (CraftIngredient c in Globals.GameBench.Crafts[craftIndex].Ingredients)
            {
                int n = Globals.Me.FindItem(c.Item);
                int x = 0;
                if (n > -1)
                {
                    x = Globals.Me.Inventory[n].ItemVal;
                    if (x == 0)
                    {
                        x = 1;
                    }
                }
                if (x < c.Quantity)
                {
                    ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Get("craftingbench", "incorrectresources"), Color.Red));
                    return;
                }
            }

            crafting = true;
            BarTimer = Globals.System.GetTimeMS();
            PacketSender.SendCraftItem(craftIndex);
            _craftWindow.IsClosable = false;
        }

        //Update the crafting bar
        public void Update()
        {
            if (!_initialized)
            {
                ListBoxRow tmpRow;
                for (int i = 0; i < Globals.GameBench.Crafts.Count; i++)
                {
                    tmpRow = _Recepies.AddRow((i + 1) + ") " + ItemBase.GetName(Globals.GameBench.Crafts[i].Item));
                    tmpRow.UserData = i;
                    tmpRow.DoubleClicked += tmpNode_DoubleClicked;
                    tmpRow.Clicked += tmpNode_DoubleClicked;
                    tmpRow.SetTextColor(Color.White);
                }

                //Load the craft data
                LoadCraftItems(0);
                _initialized = true;
            }
            if (crafting == true)
            {
                long i = Globals.System.GetTimeMS() - BarTimer;
                if (i > Globals.GameBench.Crafts[craftIndex].Time)
                {
                    i = Globals.GameBench.Crafts[craftIndex].Time;
                    crafting = false;
                    _craftWindow.IsClosable = true;
                    LoadCraftItems(craftIndex);
                }
                decimal width = Convert.ToDecimal(i) / Convert.ToDecimal(Globals.GameBench.Crafts[craftIndex].Time) *
                                _barContainer.Width;
                _bar.Width = Convert.ToInt32(width);
            }
        }
    }
}