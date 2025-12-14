using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Framework.SkillTree;
using Intersect.GameObjects;
using Intersect.Network.Packets.Client;
using Intersect.Client.Networking;

namespace Intersect.Client.Interface.Game.SkillTree
{
    public class SkillNodeItem : Button
    {
        private SkillNode _node;
        private SpellDescriptor _spell;

        public SkillNodeItem(Base parent, SkillNode node) : base(parent)
        {
            _node = node;
            SetSize(48, 48);
            _spell = SpellDescriptor.Get(node.SpellId);

            if (_spell != null)
            {
                SetToolTipText($"{_spell.Name}\nCost: {node.Cost} Points");
                
                var icon = Globals.ContentManager.GetTexture(Intersect.Client.Framework.Content.GameContentManager.TextureType.Spell, _spell.Icon);
                if (icon != null)
                {
                    SetImage(icon, true); 
                }
            }
            else
            {
                SetText("?");
            }

            OnClicked += SkillNodeItem_OnClicked;
        }

        private void SkillNodeItem_OnClicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.Me == null) return;
            
            PacketSender.SendLearnSkill(_node.SpellId);
        }
    }
}
