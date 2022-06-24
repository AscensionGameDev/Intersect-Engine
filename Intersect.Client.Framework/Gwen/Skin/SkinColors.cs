namespace Intersect.Client.Framework.Gwen.Skin
{

    /// <summary>
    ///     UI colors used by skins.
    /// </summary>
    public partial struct SkinColors
    {

        public partial struct _Window
        {

            public Color TitleActive;

            public Color TitleInactive;

        }

        public partial struct _Button
        {

            public Color Normal;

            public Color Hover;

            public Color Down;

            public Color Disabled;

        }

        public partial struct _Tab
        {

            public partial struct _Inactive
            {

                public Color Normal;

                public Color Hover;

                public Color Down;

                public Color Disabled;

            }

            public partial struct _Active
            {

                public Color Normal;

                public Color Hover;

                public Color Down;

                public Color Disabled;

            }

            public _Inactive Inactive;

            public _Active Active;

        }

        public partial struct _Label
        {

            public Color Default;

            public Color Bright;

            public Color Dark;

            public Color Highlight;

        }

        public partial struct _Tree
        {

            public Color Lines;

            public Color Normal;

            public Color Hover;

            public Color Selected;

        }

        public partial struct _Properties
        {

            public Color LineNormal;

            public Color LineSelected;

            public Color LineHover;

            public Color ColumnNormal;

            public Color ColumnSelected;

            public Color ColumnHover;

            public Color LabelNormal;

            public Color LabelSelected;

            public Color LabelHover;

            public Color Border;

            public Color Title;

        }

        public partial struct _Category
        {

            public Color Header;

            public Color HeaderClosed;

            public partial struct _Line
            {

                public Color Text;

                public Color TextHover;

                public Color TextSelected;

                public Color Button;

                public Color ButtonHover;

                public Color ButtonSelected;

            }

            public partial struct _LineAlt
            {

                public Color Text;

                public Color TextHover;

                public Color TextSelected;

                public Color Button;

                public Color ButtonHover;

                public Color ButtonSelected;

            }

            public _Line Line;

            public _LineAlt LineAlt;

        }

        public Color ModalBackground;

        public Color TooltipText;

        public _Window Window;

        public _Button Button;

        public _Tab Tab;

        public _Label Label;

        public _Tree Tree;

        public _Properties Properties;

        public _Category Category;

    }

}
