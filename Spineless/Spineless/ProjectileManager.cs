using System;
using System.Collections.Generic;
using Spineless.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gnomic.Entities;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

namespace Spineless
{
    class ProjectileManager
    {
        static Vector2 PROJECTILE_START_POS = new Vector2(100, 10);

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
            ses.Physics.Width = 0.5f;
            ses.Physics.Height = 0.5f;
            ses.Physics.Density = 1;
            ses.Physics.Offset = new Vector2(0, -(ses.Physics.Height / 2));
            ses.Position = PROJECTILE_START_POS;
            
            Projectile p    = (Projectile)ses.CreateEntity();
            p.Initialize(lvl);
            p.Deactivated   += new Action<Entity>(OnProjectileDeactivated);
            p.Physics.Bodies[0].FixtureList[0].CollidesWith = (Category)(SpinelessCollisionCategories.Terrain 
                | SpinelessCollisionCategories.Border
                | SpinelessCollisionCategories.Enemy);
            p.Physics.Bodies[0].FixtureList[0].CollisionCategories = (Category)SpinelessCollisionCategories.SplashProjectile;
            p.Physics.Bodies[0].FixtureList[0].OnCollision = OnSplashProjectileCollision;
            p.Physics.Bodies[0].FixtureList[0].UserData = p;

            return p;
        }

        public void Launch(Vector2 startPos, Vector2 impulse)
        {
            foreach(Projectile p in this.projectiles)
            {
                if (!p.IsActive)
                {
                    p.Physics.Position = startPos;
                    p.IsActive = true;
                    p.Physics.Enabled = true;
                    p.Physics.Bodies[0].ApplyLinearImpulse(impulse);
                    p.Activate();
                    break;
                }
            }
        }

        private bool OnSplashProjectileCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            Projectile p = (Projectile)fixtureA.UserData;
            p.ClipInstance.Play("death", false);
            p.Deactivate(p.ClipInstance.CurrentAnim.DurationInSeconds);
            p.Physics.Enabled = false;
            p.Physics.Bodies[0].ResetDynamics();

            return true;
        }

        private void OnProjectileDeactivated(Gnomic.Entities.Entity obj)
        {
            Projectile p = (Projectile)obj;
            p.IsActive = false;
            p.Physics.Position = PROJECTILE_START_POS;
            p.ClipInstance.Play("bounce", true);
            // p.Physics is already deactivated
        }
    }
}
