using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Server.WebApi.Authentication
{
    public class JwtToken
    {
        public string Data;
        public long Expiration;

        public override bool Equals(object obj)
        {
            return Equals(obj as JwtToken);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                return ((Data != null ? Data.GetHashCode() : 0) * 397) ^ Expiration.GetHashCode();
                // ReSharper enable NonReadonlyMemberInGetHashCode
            }
        }

        public bool Equals(JwtToken token)
        {
            return token?.Expiration == Expiration && string.Equals(token.Data, Data);
        }
    }
}
