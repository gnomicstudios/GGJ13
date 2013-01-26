using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Spineless;

namespace Spineless.AI
{
    // TODO: Add interfaces here!

    public interface IMoveable
    {
        Vector2 Position { get; set; }

#if false
        Vector2 Heading { get; set; }
        float Speed { get; set; }

        void MoveTowards();
        void Stop();
#endif
    }

#if false
    public class SetHeadingAction<IMoveable> : Behaviour<IMoveable>
    {
        Vector2 _heading;

        public SetHeadingAction(Vector2 heading)
        {
            _heading = heading;
        }

        public override bool Evaluate(IMoveable entity)
        {
            entity.Heading = _heading;
        }
    }
#endif
}
