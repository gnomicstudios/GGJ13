using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Gnomic;
using Gnomic.Entities;
using Gnomic.Physics;

namespace Spineless.Entities
{
    public class SpinelessPhysicsSettings
    {
        // simulation units
        public float Density = 1f;
        public float Width = 5f;
        public float Height = 5f;
    }

    public class SpinelessEntitySettings : ClipEntitySettings
    {
        // public PhysicsStructureSettings Physics;
        [ContentSerializer(Optional = true)]
        public float MoveForce = 50.0f;
        [ContentSerializer(Optional = true)]
        public float MaxSpeed = 0.1f;

        public SpinelessPhysicsSettings Physics;
        
        public SpinelessEntitySettings()
        {
            this.EntityClass = "Spineless.Entities.SpinelessEntity,Spineless";
        }
    }

    public class SpinelessEntity : ClipEntity
    {
        new SpinelessEntitySettings Settings;

        protected PhysicsStructure physics;

        public override void ApplySettings(EntitySettings settings)
        {
            base.ApplySettings(settings);
            this.Settings = CastSettings<SpinelessEntitySettings>(settings);
        }

        public override void Initialize(GameScreen parentScreen)
        {
            base.Initialize(parentScreen);

            if (Settings.Physics != null)
            {
                var body = BodyFactory.CreateRectangle(
                    parentScreen.Physics.World,
                    Settings.Physics.Width,
                    Settings.Physics.Height,
                    Settings.Physics.Density,
                    ConvertUnits.ToSimUnits(Settings.Position));
                body.BodyType = BodyType.Dynamic;

                physics = new PhysicsStructure
                {
                    Bodies = { body }
                };
            }
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            this.Position = physics.Position;
        }
    }
}
