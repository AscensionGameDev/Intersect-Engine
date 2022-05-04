using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Intersect.Client.Framework.Graphics;

public abstract class ViewportManager
{
    protected ViewportManager()
    {

    }

    public Viewport ActiveViewport { get; set; }

    internal abstract void OnViewportResize(Viewport viewport);
}
