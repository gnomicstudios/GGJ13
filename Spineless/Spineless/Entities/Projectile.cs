using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;

namespace Spineless.Entities
{
    [Flags]
    enum SpinelessCollisionCategories 
    {
        All                 = Category.All,
        Princess            = Category.Cat1,
        DirectHitProjectile = Category.Cat2,
        SplashProjectile    = Category.Cat3,
        Terrain             = Category.Cat4,
        Siege               = Category.Cat5,
        Knight              = Category.Cat6,
        Enemy               = Category.Cat7,
        AllProjectiles      = DirectHitProjectile | SplashProjectile,
    }

    class Projectile : SpinelessEntity
    {
        public bool IsActive;
        
        public Projectile()
        {
            IsActive = false;
        }

        public void Explode()
        {
            this.ClipInstance.Play("death", false);
        }
    }
}
