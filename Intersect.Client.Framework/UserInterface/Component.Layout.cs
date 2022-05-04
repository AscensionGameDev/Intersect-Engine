using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Intersect.Client.Framework.UserInterface;

public partial class Component
{
    private bool _dirty;

    public virtual void Invalidate(bool invalidateChildren = false)
    {
        _dirty = true;

        if (invalidateChildren)
        {
            foreach (var child in Children)
            {
                child.Invalidate(invalidateChildren);
            }
        }

        _parent?.Invalidate();
    }

    protected virtual void Layout()
    {

    }

    protected virtual void LayoutBegin()
    {

    }

    protected virtual void LayoutEnd()
    {

    }
}
