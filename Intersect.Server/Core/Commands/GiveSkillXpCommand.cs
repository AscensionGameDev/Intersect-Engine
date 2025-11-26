using Intersect.Localization;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Networking;
using Intersect.Framework.Core.GameObjects.Skills;

namespace Intersect.Server.Core.Commands
{
    internal sealed partial class GiveSkillXpCommand : TargetClientCommand
    {
        public GiveSkillXpCommand() : base(
            new LocaleCommand("giveskillxp", "Gives skill experience to a player"),
            new LocaleArgument("target", char.MinValue, "Target player name"),
            new VariableArgument<string>(new LocaleArgument("skill", char.MinValue, "Skill name or ID"), RequiredIfNotHelp, true),
            new VariableArgument<string>(new LocaleArgument("amount", char.MinValue, "Experience amount"), RequiredIfNotHelp, true)
        )
        {
        }

        private VariableArgument<string> SkillArgument => FindArgument<VariableArgument<string>>(1);
        private VariableArgument<string> AmountArgument => FindArgument<VariableArgument<string>>(2);

        protected override void HandleTarget(ServerContext context, ParserResult result, Client target)
        {
            if (target?.Entity == null)
            {
                Console.WriteLine("    Player is offline");
                return;
            }

            var skillNameOrId = result.Find(SkillArgument);
            var amountStr = result.Find(AmountArgument);

            if (string.IsNullOrEmpty(skillNameOrId))
            {
                Console.WriteLine("    No skill specified");
                return;
            }

            if (string.IsNullOrEmpty(amountStr) || !long.TryParse(amountStr, out var amount))
            {
                Console.WriteLine("    Invalid amount specified");
                return;
            }

            SkillDescriptor skill;
            if (Guid.TryParse(skillNameOrId, out Guid skillId))
            {
                skill = SkillDescriptor.Get(skillId);
            }
            else
            {
                skill = SkillDescriptor.Lookup.Values
                    .OfType<SkillDescriptor>()
                    .FirstOrDefault(s => 
                        s.Name.Equals(skillNameOrId, StringComparison.OrdinalIgnoreCase));
            }

            if (skill == null)
            {
                Console.WriteLine($"    Skill '{skillNameOrId}' not found");
                return;
            }

            lock (target.Entity)
            {
                target.Entity.GiveSkillExperience(skill.Id, amount);
                var level = target.Entity.GetSkillLevel(skill.Id);
                var exp = target.Entity.GetSkillExperience(skill.Id);
                Console.WriteLine($"    Gave {amount} XP to {target.Entity.Name}'s {skill.Name} skill (Level {level}, {exp} XP)");
            }
        }
    }
}

