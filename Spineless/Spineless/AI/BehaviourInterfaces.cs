using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Spineless.AI
{
    // TODO: Add interfaces here!

    public interface IMoveable
    {
        Vector2 Heading { get; set; }
        float Speed { get; set; }

        void MoveTowards();
        // void Stop();
    }
}
