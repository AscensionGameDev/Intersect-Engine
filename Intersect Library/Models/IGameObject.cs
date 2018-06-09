using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Models
{
    public interface IGameObject
    {
        Guid Id { get; }
    }
}