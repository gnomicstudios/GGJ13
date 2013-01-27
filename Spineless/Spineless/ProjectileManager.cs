using System;
using System.Collections.Generic;
using Spineless.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gnomic.Entities;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Gnomic.Physics;
using FarseerPhysics.Dynamics.Joints;

namespace Spineless
{
    class ProjectileManager
    {
        static Vector2 PROJECTILE_START_POS = new Vector2(0, -100);

        LevelScreen lvl;
        List<Projectile> splashProjectiles;
        List<Projectile> directProjectiles;

        public ProjectileManager(LevelScreen lvl)
        {
            this.lvl = lvl;
            this.splashProjectiles = new List<Projectile>();
            this.directProjectiles = new List<Projectile>();

            // cache bunch of projectiles

            for (int i = 0; i < 10; i++)
            {
                Projectile p = Create("ball", 32, 32, 1);
                p.Type = ProjectileType.Bomb;
                p.DefaultAnimName = "ball";
                this.splashProjectiles.Add(p);
            }

            for (int i = 0; i < 50; i++)
            {
                Projectile p = Create("arrow", 15, 15, 3);
                p.Type = ProjectileType.Arrow;
                p.DefaultAnimName = "arrow";
                this.directProjectiles.Add(p);
            }
        }

        private Projectile Create(string animName, int width, int height, float density)
        {
            SpinelessEntitySettings ses = new SpinelessEntitySettings();
            ses.ActivateByDefault       = false;
            ses.ClipFile                = "projectiles.clipxml";
            ses.EntityClass             = "Spineless.Entities.Projectile, Spineless";
            ses.DefaultAnimName         = animName;
            ses.Physics                 = new SpinelessPhysicsSettings();
            ses.Physics.Width           = ConvertUnits.ToSimUnits(width);
            ses.Physics.Height          = ConvertUnits.ToSimUnits(height);
            ses.Physics.Density         = density;
            ses.Physics.Offset          = new Vector2(0, -(ses.Physics.Height / 2));
            ses.Position                = PROJECTILE_START_POS;
            ses.Physics.Category        = SpinelessCollisionCategories.AllProjectiles;
            ses.Physics.CollidesWith    = SpinelessCollisionCategories.Terrain | SpinelessCollisionCategories.Enemy;

            Projectile p    = (Projectile)ses.CreateEntity();
            p.Initialize(lvl);

            p.Deactivated   += new Action<Entity>(OnProjectileDeactivated);
            p.Physics.Bodies[0].FixtureList[0].CollidesWith = (Category)(SpinelessCollisionCategories.Terrain | SpinelessCollisionCategories.Enemy);
            p.Physics.Bodies[0].FixtureList[0].CollisionCategories = (Category)SpinelessCollisionCategories.SplashProjectile;
            p.Physics.Bodies[0].FixtureList[0].OnCollision = OnProjectileCollision;
            p.Physics.Bodies[0].FixtureList[0].UserData = p;

            return p;
        }

        public void Launch(Vector2 startPos, Vector2 impulse, float angle, ProjectileType type)
        {
            List<Projectile> projectiles = splashProjectiles;
            if(type == ProjectileType.Arrow)
                projectiles = directProjectiles;

            foreach(Projectile p in projectiles)
            {
                if (!p.IsActive)
                {
                    p.Physics.Position = startPos;
                    p.IsActive = true;
                    p.Physics.Enabled = true;
                    p.Physics.Bodies[0].Rotation = angle;
                    p.Physics.Bodies[0].ApplyLinearImpulse(impulse);
                    p.Activate();
                    break;
                }
            }
        }

        private bool OnProjectileCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            Projectile p = (Projectile)fixtureA.UserData;

            if (p.Type == ProjectileType.Bomb)
            {
                // KABOOM!
                p.ClipInstance.Play("explode", false);
                lvl.Splash(p.Physics.Position, 100, 120);
                p.Deactivate(p.ClipInstance.CurrentAnim.DurationInSeconds);
                p.Physics.Enabled = false;
            }
            else
            {
                if(fixtureB.UserData is Unit)
                {
                    Unit u = (Unit)fixtureB.UserData;
                    p.HitJoint = new RevoluteJoint(p.Physics.Bodies[0], u.Physics.Bodies[0], Vector2.Zero, u.Physics.Bodies[0].GetLocalPoint(p.Physics.Bodies[0].Position));
                    lvl.Physics.World.AddJoint(p.HitJoint);
                }
                p.ClipInstance.Play("arrowHit");
                p.Deactivate(10);
            }


            p.Physics.Bodies[0].ResetDynamics();

            return true;
        }

        private void OnProjectileDeactivated(Gnomic.Entities.Entity obj)
        {
            Projectile p = (Projectile)obj;
            p.IsActive = false;
            p.HitJoint = null;
            p.Physics.Position = PROJECTILE_START_POS;
            p.ClipInstance.Play(p.DefaultAnimName, true);
            p.Physics.Enabled = false;
        }
    }
}
