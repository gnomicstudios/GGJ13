using System;
using System.Collections.Generic;
using Spineless.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spineless
{
    class ProjectileManager
    {
        LevelScreen lvl;
        List<Projectile> projectiles;

        public ProjectileManager(LevelScreen lvl)
        {
            this.lvl = lvl;
            this.projectiles = new List<Projectile>();

            // cache bunch of projectiles
            for (int i = 0; i < 10; i++)
            {
                this.projectiles.Add(Create()); 
            }
        }

        private Projectile Create()
        {
            SpinelessEntitySettings ses = new SpinelessEntitySettings();
            ses.ActivateByDefault = false;
            ses.ClipFile = "egg.clipxml";
            ses.EntityClass = "Spineless.Entities.Projectile, Spineless";
            ses.DefaultAnimName = "bounce";
            ses.Physics = new SpinelessPhysicsSettings();
            ses.Physics.Width = 1;
            ses.Physics.Height = 1;
            ses.Position = new Vector2(100, 10);
            
            Projectile p = (Projectile)ses.CreateEntity();
            p.Initialize(lvl);

            return p;
        }

        public void Launch(Vector2 impulse)
        {
            foreach(Projectile p in this.projectiles)
            {
                if (!p.IsActive)
                {
                    p.Physics.Enabled = true;
                    p.Physics.Bodies[0].ApplyLinearImpulse(impulse);
                    p.IsActive = true;
                    lvl.AddEntity(p);
                    break;
                }
            }
        }
    }
}
