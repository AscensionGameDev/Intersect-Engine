using System;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Text box with masked text.
    /// </summary>
    /// <remarks>
    ///     This class doesn't prevent programatic access to the text in any way.
    /// </remarks>
    public class TextBoxPassword : TextBox
    {

        private string mMask;

        private char mMaskCharacter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextBoxPassword" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TextBoxPassword(Base parent, string name = "") : base(parent, name)
        {
            mMaskCharacter = '*';
        }

        /// <summary>
        ///     Character used in place of actual characters for display.
        /// </summary>
        public char MaskCharacter
        {
            get => mMaskCharacter;
            set => mMaskCharacter = value;
        }

        /// <summary>
        ///     Handler for text changed event.
        /// </summary>
        protected override void OnTextChanged()
        {
            mMask = new String(MaskCharacter, Text.Length);
            TextOverride = mMask;
            base.OnTextChanged();

            //Really hacky way to make sure the size for the mask is calculated.
            base.Children[0].SizeToChildren();
        }

    }

}
