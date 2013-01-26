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
        float Speed { get; set; }

        void MoveTowards(Vector2 position);
    }
}
