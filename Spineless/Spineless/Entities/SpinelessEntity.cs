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
        public Vector2 Offset = Vector2.Zero;
        public float RotationalInertia = float.MaxValue;
    }

    public class SpinelessEntitySettings : ClipEntitySettings
    {
        // public PhysicsStructureSettings Physics;
        [ContentSerializer(Optional = true)]
        public float MoveForce = 500.0f;
        [ContentSerializer(Optional = true)]
        public float MaxSpeed = 0.1f;
        [ContentSerializer(Optional = true)]
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
        public PhysicsStructure Physics
        { 
            get { return physics; }
        }

        public LevelScreen LevelScreen { get; set; }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
        }

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
                    Settings.Physics.Width * Settings.Scale.X,
                    Settings.Physics.Height * Settings.Scale.Y,
                    Settings.Physics.Density,
                    ConvertUnits.ToSimUnits(Settings.Position),
                    Settings.Physics.Offset);

                body.BodyType = BodyType.Dynamic;
                body.Inertia = Settings.Physics.RotationalInertia;

                physics = new PhysicsStructure
                {
                    Bodies = { body }
                };

                physics.Enabled = Settings.ActivateByDefault;
            }

            this.LevelScreen = (LevelScreen)parentScreen;
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (physics != null && physics.Enabled)
            {
                this.Position = physics.Position;
                //this.Rotation = physics.Bodies[0].Rotation;
            }   
        }

        public override void Draw2D(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw2D(spriteBatch);
        }
    }
}
