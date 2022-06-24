using System;

namespace Intersect.Security
{

    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Interface |
        AttributeTargets.Struct |
        AttributeTargets.Field |
        AttributeTargets.Property
    )]
    public partial class ApiVisibilityAttribute : Attribute
    {

        public ApiVisibilityAttribute(ApiVisibility visibility)
        {
            Visibility = visibility;
        }

        public ApiVisibility Visibility { get; }

    }

}
