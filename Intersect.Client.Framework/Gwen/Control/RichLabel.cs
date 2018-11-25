using System;
using System.Collections.Generic;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;

namespace Intersect.Client.Framework.Gwen.Control
{
    /// <summary>
    ///     Multiline label with text chunks having different color/font.
    /// </summary>
    public class RichLabel : Base
    {
        private readonly List<TextBlock> mTextBlocks;
        private readonly string[] mNewline;

        private bool mNeedsRebuild;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RichLabel" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public RichLabel(Base parent, string name = "")
            : base(parent, name)
        {
            mNewline = new string[] {Environment.NewLine};
            mTextBlocks = new List<TextBlock>();
        }

        /// <summary>
        ///     Adds a line break to the control.
        /// </summary>
        public void AddLineBreak()
        {
            TextBlock block = new TextBlock {Type = BlockType.NewLine};
            mTextBlocks.Add(block);
        }

        /// <summary>
        ///     Adds text to the control.
        /// </summary>
        /// <param name="text">Text to add.</param>
        /// <param name="color">Text color.</param>
        /// <param name="font">Font to use.</param>
        public void AddText(string text, Color color, Alignments alignment = Alignments.Left, GameFont font = null)
        {
            if (String.IsNullOrEmpty(text))
                return;

            var lines = text.Split(mNewline, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                    AddLineBreak();

                TextBlock block = new TextBlock
                {
                    Type = BlockType.Text,
                    Text = lines[i],
                    Color = color,
                    Font = font,
                    Alignment = alignment
                };

                mTextBlocks.Add(block);
                mNeedsRebuild = true;
                Invalidate();
            }
        }

        /// <summary>
        ///     Resizes the control to fit its children.
        /// </summary>
        /// <param name="width">Determines whether to change control's width.</param>
        /// <param name="height">Determines whether to change control's height.</param>
        /// <returns>
        ///     True if bounds changed.
        /// </returns>
        public override bool SizeToChildren(bool width = true, bool height = true)
        {
            Rebuild();
            return base.SizeToChildren(width, height);
        }

        protected void SplitLabel(string text, GameFont font, TextBlock block, ref int x, ref int y, ref int lineHeight)
        {
            var spaced = Util.SplitAndKeep(text, " ");
            if (spaced.Length == 0)
                return;

            int spaceLeft = Width - x;
            string leftOver;

            // Does the whole word fit in?
            Point stringSize = Skin.Renderer.MeasureText(font, text);
            if (spaceLeft > stringSize.X)
            {
                CreateLabel(text, block, ref x, ref y, ref lineHeight, true);
                return;
            }

            // If the first word is bigger than the line, just give up.
            Point wordSize = Skin.Renderer.MeasureText(font, spaced[0]);
            if (wordSize.X >= spaceLeft)
            {
                CreateLabel(spaced[0], block, ref x, ref y, ref lineHeight, true);
                if (spaced[0].Length >= text.Length)
                    return;

                leftOver = text.Substring(spaced[0].Length + 1);
                SplitLabel(leftOver, font, block, ref x, ref y, ref lineHeight);
                return;
            }

            string newString = String.Empty;
            for (int i = 0; i < spaced.Length; i++)
            {
                wordSize = Skin.Renderer.MeasureText(font, newString + spaced[i]);
                if (wordSize.X > spaceLeft)
                {
                    CreateLabel(newString, block, ref x, ref y, ref lineHeight, true);
                    x = 0;
                    y += lineHeight;
                    break;
                }

                newString += spaced[i];
            }

            int newstrLen = newString.Length;
            if (newstrLen < text.Length)
            {
                leftOver = text.Substring(newstrLen + 1);
                SplitLabel(leftOver, font, block, ref x, ref y, ref lineHeight);
            }
        }

        protected void CreateLabel(string text, TextBlock block, ref int x, ref int y, ref int lineHeight, bool noSplit)
        {
            // Use default font or is one set?
            GameFont font = Skin.DefaultFont;
            if (block.Font != null)
                font = block.Font;

            // This string is too long for us, split it up.
            Point p = Skin.Renderer.MeasureText(font, text);

            if (lineHeight == -1)
            {
                lineHeight = p.Y;
            }

            if (!noSplit)
            {
                if (x + p.X > Width)
                {
                    SplitLabel(text, font, block, ref x, ref y, ref lineHeight);
                    return;
                }
            }

            // Wrap
            if (x + p.X > Width)
            {
                CreateNewline(ref x, ref y, lineHeight);
            }

            Label label = new Label(this);
            label.SetText(x == 0 ? text.TrimStart(' ') : text);
            label.TextColorOverride = block.Color;
            label.Font = font;
            label.RestrictToParent = false;
            label.SizeToContents();
            label.Alignment = Pos.None;
            label.SetPosition(x, y);
            label.RemoveAlignments();
            label.AddAlignment(block.Alignment);
            label.ProcessAlignments();

            //lineheight = (lineheight + pLabel.Height()) / 2;

            x += label.Width;

            if (x > Width)
            {
                CreateNewline(ref x, ref y, lineHeight);
            }
        }

        protected void CreateNewline(ref int x, ref int y, int lineHeight)
        {
            x = 0;
            y += lineHeight;
        }

        public void ClearText()
        {
            mTextBlocks.Clear();
            mNeedsRebuild = true;
        }

        protected void Rebuild()
        {
            DeleteAllChildren();

            int x = 0;
            int y = 0;
            int lineHeight = -1;

            foreach (var block in mTextBlocks)
            {
                if (block.Type == BlockType.NewLine)
                {
                    CreateNewline(ref x, ref y, lineHeight);
                    continue;
                }

                if (block.Type == BlockType.Text)
                {
                    CreateLabel(block.Text, block, ref x, ref y, ref lineHeight, false);
                    continue;
                }
            }

            mNeedsRebuild = false;
        }

        /// <summary>
        ///     Handler invoked when control's bounds change.
        /// </summary>
        /// <param name="oldBounds">Old bounds.</param>
        protected override void OnBoundsChanged(Rectangle oldBounds)
        {
            base.OnBoundsChanged(oldBounds);
            Rebuild();
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            base.Layout(skin);
            if (mNeedsRebuild)
                Rebuild();

            // align bottoms. this is still not ideal, need to take font metrics into account.
            Base prev = null;
            foreach (Base child in Children)
            {
                if (prev != null && child.Y == prev.Y)
                {
                    Align.PlaceRightBottom(child, prev);
                }
                prev = child;
            }
        }

        protected struct TextBlock
        {
            public BlockType Type;
            public string Text;
            public Color Color;
            public GameFont Font;
            public Alignments Alignment;
        }

        protected enum BlockType
        {
            Text,
            NewLine
        }
    }
}