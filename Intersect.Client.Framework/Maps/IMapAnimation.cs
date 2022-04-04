using Intersect.Client.Framework.Entities;
using System;

namespace Intersect.Client.Framework.Maps
{
    public interface IMapAnimation : IAnimation
    {
        Guid Id { get; }
    }
}