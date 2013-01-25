﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Gnomic;
using Gnomic.Entities;
using Gnomic.Physics;

namespace Spineless.Entities
{
    public class SpinelessEntitySettings : ClipEntitySettings
    {
        public PhysicsStructureSettings Physics;
        [ContentSerializer(Optional = true)]
        public float MoveForce = 50.0f;
        [ContentSerializer(Optional = true)]
        public float MaxSpeed = 0.1f;

        public SpinelessEntitySettings()
        {
            this.EntityClass = "Spineless.Entities.SpinelessEntity,Spineless";
        }
    }

    public class SpinelessEntity : ClipEntity
    {
        new SpinelessEntitySettings Settings;

        protected PhysicsStructure physics;

        public override void Initialize(GameScreen parentScreen)
        {
            base.Initialize(parentScreen);

            physics = Settings.Physics.CreateStructure(
                parentScreen.Physics.World, this.Position);
            physics.SetVelocityLimit(
                parentScreen.Physics.World, Settings.MaxSpeed, 0.0f);
        }
    }
}
