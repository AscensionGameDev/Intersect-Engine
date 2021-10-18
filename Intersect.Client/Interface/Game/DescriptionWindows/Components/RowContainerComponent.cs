using Intersect.Client.Framework.Gwen.Control;

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Client.Interface.Game.DescriptionWindows.Components
{
    public class RowContainerComponent : ComponentBase
    {
        protected KeyvalueRowComponent mKeyValueRow;

        protected JObject KeyValueRowLayout;

        protected int mComponentY = 0;

        public RowContainerComponent(Base parent, string name) : base(parent, name)
        {
            GenerateComponents();
            LoadLayout();
            GetLayoutTemplates();
            DestroyLayoutComponents();
        }

        protected override void GenerateComponents()
        {
            base.GenerateComponents();

            mKeyValueRow = new KeyvalueRowComponent(mContainer);
        }

        protected void GetLayoutTemplates()
        {
            KeyValueRowLayout = mKeyValueRow.GetJson();
        }

        protected void DestroyLayoutComponents()
        {
            mKeyValueRow.Dispose();
        }

        protected void PositionComponent(ComponentBase component)
        {
            component.SetPosition(component.X, mComponentY);
            mComponentY += component.Height;
        }

        public override void CorrectWidth()
        {
            base.CorrectWidth();
            var margins = mContainer.Margin;

            mContainer.SetSize(mContainer.Width, mContainer.Height + margins.Bottom);
        }

        public KeyvalueRowComponent AddKeyValueRow(string key, string value)
        {
            var row = new KeyvalueRowComponent(mContainer, key, value);

            // Since we're pulling some trickery here, catch any errors doing this ourselves and log them.
            try
            {
                row.LoadJson(KeyValueRowLayout);
            }
            catch (Exception ex)
            {
                Logging.Log.Error(ex, $"An error occured while loading KeyvalueRowComponent Json for {mName}");
            }

            row.SizeToChildren(true, false);
            PositionComponent(row);
            return row;
        }
    }

    public class KeyvalueRowComponent : ComponentBase
    {
        protected Label mKeyLabel;

        protected Label mValueLabel;

        public KeyvalueRowComponent(Base parent) : this(parent, string.Empty, string.Empty)
        {
        }


        public KeyvalueRowComponent(Base parent, string key, string value) : base(parent, "KeyValueRow")
        {
            GenerateComponents();
            mKeyLabel.SetText(key);
            mValueLabel.SetText(value);
        }

        protected override void GenerateComponents()
        {
            base.GenerateComponents();

            mKeyLabel = new Label(mContainer, "Key");
            mValueLabel = new Label(mContainer, "Value");
        }

        public JObject GetJson() => mContainer.GetJson();

        public void LoadJson(JObject json) => mContainer.LoadJson(json);
    }
}
