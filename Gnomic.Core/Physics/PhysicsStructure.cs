using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using FarseerPhysics.Controllers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Gnomic.Physics;

namespace Gnomic.Physics
{
    public class PhysicsStructure
    {
        public List<Body> Bodies { get; set; }

        //also list of joints & springs
        public List<Joint> Joints { get; set; }

        private VelocityLimitController velocityLimiter;

        internal Vector2 position;
        public Vector2 Position 
        {
            get
            {
                return ConvertUnits.ToDisplayUnits(Bodies[0].Position);
            }
            set 
            {
                Vector2 simVal = ConvertUnits.ToSimUnits(value);

                if (position != simVal)
                {
                    position = simVal;

                    if (Bodies.Count == 0)
                        return;

                    // treat body 0 as "root" body
                    Body body = Bodies[0];

                    // move other bodies relative to body 0
                    Vector2 basePos = body.Position;
                    body.Position = position;

                    for (int i = 1; i < Bodies.Count; i++)
                    {
                        body = Bodies[i];
                        body.Position = new Vector2(position.X + body.Position.X - basePos.X, position.Y + body.Position.Y - basePos.Y);
                    }
                }
            }
        }
        
        object userData;
        public object UserData
        {
            get { return userData; }
            set
            {
                if (userData != value)
                {
                    userData = value;
                    foreach (Body b in Bodies)
                    {
                        b.UserData = userData;
                        foreach (Fixture f in b.FixtureList)
                        {
                            f.UserData = userData;
                        }
                    }
                }
            }
        }

        public PhysicsStructure()
        {
            Bodies = new List<Body>();
            Joints = new List<Joint>();
        }
        public PhysicsStructure(Vector2 pos)
            : this()
        {
            position = pos;
        }


        public void ResetDynamics()
        {
            foreach (Body b in Bodies)
            {
                b.ResetDynamics();
            }
        }

        public void SetJointsEnabled(bool enabled)
        {
            foreach (Joint j in Joints)
                j.Enabled = enabled;
        }

        //public void SetVelocityLimit(World world, float maxLinearVelocity, float maxAngularVelocity)
        //{
        //    if (velocityLimiter == null)
        //    {
        //        velocityLimiter = new VelocityLimitController(maxLinearVelocity, maxAngularVelocity);
        //        world.AddController(velocityLimiter);
        //        foreach (Body b in Bodies)
        //        {
        //            velocityLimiter.AddBody(b);
        //        }
        //    }
        //    else
        //    {
        //        velocityLimiter.MaxLinearVelocity = maxLinearVelocity;
        //        velocityLimiter.MaxAngularVelocity = maxAngularVelocity;
        //    }
        //}

        private bool enabled = true;
        public bool Enabled 
        { 
            get
            {
                return enabled;
            } 
            set
            {
                enabled = value;
                foreach (Body b in Bodies)
                {
                    b.Enabled = value;
                }
                foreach (Joint j in Joints)
                {
                    j.Enabled = value;
                }
            }
        }

        public void ApplyForceToBody(
            int index, float magnitude, Vector2 direction)
        {
            Bodies[index].ApplyForce(ConvertUnits.ToSimUnits(magnitude) *
                                     ConvertUnits.ToSimUnits(direction));
        }
    }
}
