using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

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
