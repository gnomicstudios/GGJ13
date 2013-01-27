using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Joints;

namespace Spineless.Entities
{
    public enum ProjectileType 
    {
        Bomb,
        Arrow
    }

    class Projectile : SpinelessEntity
    {
        public bool IsActive;
        public ProjectileType Type;
        public string DefaultAnimName;
        public RevoluteJoint HitJoint;

        public Projectile()
        {
            IsActive = false;
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (this.Type == ProjectileType.Arrow)
            {
                Vector2 v = this.Physics.Bodies[0].LinearVelocity;
                if (v.Length() > 0.01f)
                    ClipInstance.Rotation = (float)Math.Atan2(v.Y, v.X);
            }
        }

    }
}
