using System;
using JetBrains.Annotations;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Localization
{
    public struct LocalizedString
    {
        [NotNull]
        private readonly string mValue;

        public LocalizedString([NotNull] string value)
        {
            mValue = value;
        }

        public static implicit operator LocalizedString([NotNull] string value)
        {
            return new LocalizedString(value);
        }

        public static implicit operator string(LocalizedString str)
        {
            return str.mValue;
        }

        public override string ToString()
        {
            return mValue;
        }

        public string ToString(params object[] args)
        {
            try
            {
                return args?.Length == 0 ? mValue : string.Format(mValue, args ?? new object[] { });
            }
            catch (FormatException)
            {
                return "Format Exception!";
            }
        }
    }
}
