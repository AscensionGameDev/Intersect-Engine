using System.Collections.Generic;
using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.SkillTree;
using Intersect.GameObjects;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Game.SkillTree
{
    public class SkillTreeWindow : Window
    {
        private ScrollControl _container;
        private List<SkillNodeItem> _nodes = new List<SkillNodeItem>();

        public SkillTreeWindow(Canvas gameCanvas) : base(gameCanvas, "Skill Tree", false, "SkillTreeWindow")
        {
            DisableResizing();
            IsClosable = true;
            SetSize(400, 400);
            Alignment = [Alignments.Center];

            _container = new ScrollControl(this, "SkillTreeContainer");
            _container.Dock = Pos.Fill;
            _container.EnableScroll(true, true);
        }

        public void LoadTree()
        {
            _container.DeleteAllChildren();
            _nodes.Clear();

            // Just showing the first tree for now or all combined
            foreach (var tree in Globals.SkillTrees)
            {
                // Create a label for tree name?
                 var lbl = new Label(_container);
                 lbl.Text = tree.Name;
                 lbl.SetPosition(10, 10); // Simple positioning

                foreach (var node in tree.Nodes)
                {
                    var item = new SkillNodeItem(_container, node);
                    // Initial Layout: 50x50 grid + offset
                    int offsetX = 50;
                    int offsetY = 40;
                    item.SetPosition(node.X * 64 + offsetX, node.Y * 64 + offsetY);
                    _nodes.Add(item);
                }
            }
        }

        public override void Show()
        {
            base.Show();
            LoadTree(); // Reload on show to update states
        }

        protected override void EnsureInitialized()
        {
            LoadTree();
        }
    }
}
