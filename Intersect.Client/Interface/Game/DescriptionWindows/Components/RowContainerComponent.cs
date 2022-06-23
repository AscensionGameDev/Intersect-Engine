using System;

using Newtonsoft.Json.Linq;

using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Interface.Game.DescriptionWindows.Components
{
    public partial class RowContainerComponent : ComponentBase
    {
        protected KeyValueRowComponent mKeyValueRow;

        protected JObject KeyValueRowLayout;

        private int mComponentY = 0;

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

            mKeyValueRow = new KeyValueRowComponent(mContainer);
        }

        private void GetLayoutTemplates()
        {
            KeyValueRowLayout = mKeyValueRow.GetJson();
        }

        private void DestroyLayoutComponents()
        {
            mKeyValueRow.Dispose();
        }

        private void PositionComponent(ComponentBase component)
        {
            component.SetPosition(component.X, mComponentY);
            mComponentY += component.Height;
        }

        /// <inheritdoc/>
        public override void CorrectWidth()
        {
            base.CorrectWidth();
            var margins = mContainer.Margin;

            mContainer.SetSize(mContainer.Width, mContainer.Height + margins.Bottom);
        }

        /// <summary>
        /// Add a new <see cref="KeyValueRowComponent"/> row to the container.
        /// </summary>
        /// <param name="key">The key to display.</param>
        /// <param name="value">The value to display.</param>
        /// <returns>Returns a new instance of <see cref="KeyValueRowComponent"/> with the provided settings.</returns>
        public KeyValueRowComponent AddKeyValueRow(string key, string value)
        {
            var row = new KeyValueRowComponent(mContainer, key, value);

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

    public partial class KeyValueRowComponent : ComponentBase
    {
        protected Label mKeyLabel;

        protected Label mValueLabel;

        public KeyValueRowComponent(Base parent) : this(parent, string.Empty, string.Empty)
        {
        }

        public KeyValueRowComponent(Base parent, string key, string value) : base(parent, "KeyValueRow")
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

        /// <summary>
        /// Set the <see cref="Color"/> of the key text.
        /// </summary>
        /// <param name="color">The <see cref="Color"/> to draw the key text in.</param>
        public void SetKeyTextColor(Color color) => mKeyLabel.SetTextColor(color, Label.ControlState.Normal);

        /// <summary>
        /// Set the <see cref="Color"/> of the value text.
        /// </summary>
        /// <param name="color">The <see cref="Color"/> to draw the value text in.</param>
        public void SetValueTextColor(Color color) => mValueLabel.SetTextColor(color, Label.ControlState.Normal);

        /// <summary>
        /// Get the Json layout of the current component.
        /// </summary>
        /// <returns>Returns a <see cref="JObject"/> containing the layout of the current component.</returns>
        public JObject GetJson() => mContainer.GetJson();

        /// <summary>
        /// Set the Json layout of the current component.
        /// </summary>
        /// <param name="json">The new layout to apply to this component in <see cref="JObject"/> format.</param>
        public void LoadJson(JObject json) => mContainer.LoadJson(json);
    }
}
