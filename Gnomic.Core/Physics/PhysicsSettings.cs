using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;

namespace Gnomic.Physics
{
    public class BodySettings
    {
        [ContentSerializer(Optional = true)]
        public float Inertia { get; set; }
        [ContentSerializer(Optional = true)]
        public float Friction { get; set; }
        [ContentSerializer(Optional = true)]
        public bool IsStatic { get; set; }
        [ContentSerializer(Optional = true)]
        public bool IgnoreGravity { get; set; }

        [ContentSerializer(Optional = true)]
        public Vector2 Offset { get; set; }

        [ContentSerializer(Optional = true)]
        public List<FixtureSettings> Fixtures { get; set; }

        public BodySettings() 
        {
            Fixtures = new List<FixtureSettings>();

            Inertia = 2f;
            Friction = 2f;
            IsStatic = false;
            IgnoreGravity = false;
        }

        public Body CreateBody(World w)
        {
            return CreateBody(w, Vector2.Zero);
        }
        public Body CreateBody(World w, Vector2 position)
        {
            Body b = new Body(w);
            b.IsStatic = IsStatic;
            b.IgnoreGravity = IgnoreGravity;

            foreach (FixtureSettings fs in Fixtures)
            {
                List<Fixture> fixtures = fs.CreateFixture(b);
                foreach (Fixture f in fixtures)
                {
                    b.FixtureList.Add(f);
                }
            }

            b.Inertia = Inertia;
            b.Friction = Friction;
            b.Position = position + Offset;

            return b;
        }
    }

    public class FixtureSettings
    {
        public Vector2[] Verts { get; set; }
        [ContentSerializer(Optional = true)]
        public float Density { get; set; }
        [ContentSerializer(Optional = true)]
        public Vector2 Offset { get; set; }
        [ContentSerializer(Optional = true)]
        public float RotationOffset { get; set; }
        [ContentSerializer(Optional = true)]
        public bool IsSensor { get; set; }
        [ContentSerializer(Optional = true)]
        public float Restitution { get; set; }
        [ContentSerializer(Optional = true)]
        public short CollisionGroup { get; set; }
        [ContentSerializer(Optional = true)]
        public int CollisionCategories { get; set; }
        [ContentSerializer(Optional = true)]
        public int CollidesWith { get; set; }

        public FixtureSettings() 
        {
            Density = 1.0f;
            Restitution = 0.3f;
        }
                
        public List<Fixture> CreateFixture(Body body)
        {
            var vertices = new Vertices(Verts);
            List<Vertices> vertsList = BayazitDecomposer.ConvexPartition(vertices);
            
            List<Fixture> fixtureList = new List<Fixture>();

            Vector2 mainCentroid = vertices.GetCentroid();

            foreach (Vertices v in vertsList)
            {
                PolygonShape shape = new PolygonShape(v, Density);
                Fixture fixture = new Fixture(body, shape, RotationOffset);
                fixture.IsSensor = IsSensor;
                fixture.CollisionGroup = CollisionGroup;
                fixture.CollisionCategories = (Category)CollisionCategories;
                fixture.CollidesWith = (Category)CollidesWith;
                fixtureList.Add(fixture);
            }
            return fixtureList;
        }

    }
    public class PhysicsStructureSettings
    {
        public List<BodySettings> Bodies { get; set; }

        //List of joints
        public List<JointSettings> Joints { get; set; }

        public PhysicsStructureSettings()
        {
            Bodies = new List<BodySettings>();
        }

        public PhysicsStructure CreateStructure(World w, Vector2 position)
        {
            PhysicsStructure po = new PhysicsStructure(position);

            // Create elements
            foreach (BodySettings bs in Bodies)
                po.Bodies.Add(bs.CreateBody(w, position));

            // Create element links
            if (Joints != null)
            {
                foreach (JointSettings js in Joints)
                    js.AddToPhysicsObject(w, po);
            }

            return po;
        }
    }

    public class JointSettings
    {
        public JointType JointType { get; set; }

        public int BodyId1 { get; set; }

        [ContentSerializer(Optional = true)]
        public int BodyId2 { get; set; }

        [ContentSerializer(Optional = true)]
        public float SpringConstant { get; set; }

        [ContentSerializer(Optional = true)]
        public float DampingConstant { get; set; }

        [ContentSerializer(Optional = true)]
        public float BreakPoint { get; set; }

        [ContentSerializer(Optional = true)]
        public Vector2 Anchor1 { get; set; }

        [ContentSerializer(Optional = true)]
        public Vector2 Anchor2 { get; set; }

        [ContentSerializer(Optional = true)]
        public float Min { get; set; }

        [ContentSerializer(Optional = true)]
        public float Max { get; set; }

        public JointSettings()
        {
            JointType = JointType.Revolute;
            SpringConstant = 200f;
            DampingConstant = 10f;
            BreakPoint = 20.0f;
        }


        public Joint AddToPhysicsObject(World w, PhysicsStructure parentObject)
        {
            Joint j = null;
            Body body1 = null;
            Body body2 = null;

            if (BodyId1 >= 0)
                body1 = parentObject.Bodies[BodyId1];

            if (BodyId2 >= 0)
                body2 = parentObject.Bodies[BodyId2];

            switch (this.JointType)
            {
                case JointType.Revolute:
                    {
                        j = JointFactory.CreateRevoluteJoint(w, body1, body2, parentObject.Position + Anchor1);
                    }
                    break;
                case JointType.FixedRevolute:
                    {
                        j = JointFactory.CreateFixedRevoluteJoint(w, body1, Anchor1, parentObject.Position + Anchor1);
                    }
                    break;
                case JointType.Angle:
                    {
                        j = JointFactory.CreateAngleJoint(w, body1, body2);
                    }
                    break;
                default:
                    break;
            }

            if (j != null)
            {
                j.Breakpoint = BreakPoint;
                parentObject.Joints.Add(j);
            }
            return j;
        }
        //
        // Springs
        //
        //    AngleJoint aj = JointFactory.CreateAngleJoint(w, 
        //    case JointType.Angle:
        //        if (BodyId2 >= 0)
        //            s = JointFactory.CreateAngleSpring(body1, body2, SpringConstant, DampingConstant);
        //        else
        //        {
        //            s = SpringFactory.Instance.CreateFixedAngleSpring(body1, SpringConstant, DampingConstant);
        //            JointType = JointType.SpringAngleFixed;
        //        }
        //        break;

        //    case JointType.SpringAngleFixed:
        //            s = SpringFactory.Instance.CreateFixedAngleSpring(body1, SpringConstant, DampingConstant);
        //        break;

        //    case JointType.SpringLinear:
        //        if (BodyId2 >= 0)
        //            s = SpringFactory.Instance.CreateLinearSpring(body1, body1.GetLocalPoint(parentObject.pos + Anchor1), body2, body2.GetLocalPoint(parentObject.pos + Anchor2), SpringConstant, DampingConstant);
        //        else
        //        {
        //            s = SpringFactory.Instance.CreateFixedLinearSpring(body1, body1.GetLocalPoint(parentObject.pos + Anchor1), parentObject.pos + Anchor2, SpringConstant, DampingConstant);
        //            JointType = JointType.SpringLinearFixed;                       
        //        }
        //        break;

        //    case JointType.SpringLinearFixed:
        //        s = SpringFactory.Instance.CreateFixedLinearSpring(body1, body1.GetLocalPoint(parentObject.pos + Anchor1), parentObject.pos + Anchor2, SpringConstant, DampingConstant);
        //        break;

        //    //
        //    // Joints
        //    //


        //        break;
        //    case LinkType.JointAngle:
        //        j = JointFactory.Instance.CreateAngleJoint(body1, body2);
        //        break;
        //    case LinkType.JointAngleFixed:
        //        j = JointFactory.Instance.CreateFixedAngleJoint(body1);
        //        break;
        //    case LinkType.JointAngleLimit:
        //        j = JointFactory.Instance.CreateAngleLimitJoint(body1, body2, Min, Max);
        //        break;
        //    case LinkType.JointAngleLimitFixed:
        //        j = JointFactory.Instance.CreateFixedAngleLimitJoint(body1, Min, Max);
        //        break;
        //    case LinkType.JointPin:
        //        j = JointFactory.Instance.CreatePinJoint(body1, body1.GetLocalPoint(parentObject.pos + Anchor1), body2, body2.GetLocalPoint(parentObject.pos + Anchor2));
        //        break;
        //    case LinkType.JointSlider:
        //        j = JointFactory.Instance.CreateSliderJoint(body1, body1.GetLocalPoint(parentObject.pos + Anchor1), body2, body2.GetLocalPoint(parentObject.pos + Anchor2), Min, Max);
        //        break;
        //}
    }
}
