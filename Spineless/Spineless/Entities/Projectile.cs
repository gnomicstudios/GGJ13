using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;

namespace Spineless.Entities
{
    class Projectile : SpinelessEntity
    {
        public bool IsActive;
        
        public Projectile()
        {
            IsActive = false;
        }
    }
}
