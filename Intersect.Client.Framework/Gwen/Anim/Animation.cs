using System;
using System.Collections.Generic;

using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.Anim
{

    public class Animation
    {

        //private static List<Animation> g_AnimationsListed = new List<Animation>(); // unused
        private static readonly Dictionary<Base, List<Animation>> Animations = new Dictionary<Base, List<Animation>>();

        protected Base mControl;

        public virtual bool Finished => throw new InvalidOperationException("Pure virtual function call");

        protected virtual void Think()
        {
        }

        public static void Add(Base control, Animation animation)
        {
            animation.mControl = control;
            if (!Animations.ContainsKey(control))
            {
                Animations[control] = new List<Animation>();
            }

            Animations[control].Add(animation);
        }

        public static void Cancel(Base control)
        {
            if (Animations.ContainsKey(control))
            {
                Animations[control].Clear();
                Animations.Remove(control);
            }
        }

        internal static void GlobalThink()
        {
            foreach (var pair in Animations)
            {
                var valCopy = pair.Value.FindAll(x => true); // list copy so foreach won't break when we remove elements
                foreach (var animation in valCopy)
                {
                    animation.Think();
                    if (animation.Finished)
                    {
                        pair.Value.Remove(animation);
                    }
                }
            }
        }

    }

}
