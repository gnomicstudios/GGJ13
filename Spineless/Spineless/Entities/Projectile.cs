using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;

namespace Spineless.Entities
{
    public enum ProjectileType 
    {
        Splash,
        DirectHit
    }

    class Projectile : SpinelessEntity
    {
        public bool IsActive;
        public ProjectileType Type;
        public string DefaultAnimName;

        public Projectile()
        {
            IsActive = false;
        }
    }
}
