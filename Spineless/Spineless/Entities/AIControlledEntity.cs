using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Gnomic;
using Gnomic.Entities;
using Spineless.AI;

namespace Spineless.Entities
{
#if false
    public class AIControlledEntitySettings : SpinelessEntitySettings
    {
        public string BehaviourName;

        public override void ApplySettings(EntitySettings settings)
        {
            base.ApplySettings(settings);
            this.Settings = CastSettings<AIControlledEntitySettings>(settings);
        }

        public AIControlledEntitySettings()
        {
            this.EntityClass = "Spineless.Entities.AIControlledEntity,Spineless";
        }
    }

    public class AIControlledEntity : SpinelessEntity, IMoveable
    {
        new AIControlledEntitySettings Settings;

        public Behaviour<AIControlledEntity> Behaviour { get; set; }
        public Vector2 Heading { get; set; }
        public float Speed { get; set; }

        public override void Initialize(GameScreen parentScreen)
        {
            base.Initialize(parentScreen);

            LevelScreen screen = (LevelScreen)parentScreen;
            Behaviour = screen.BehaviourFactory.Create<AIControlledEntity>(
                Settings.BehaviourName);
        }

        public void MoveTowards()
        {
            physics.ApplyForceToBody(0, Speed, Heading);
        }
        
        public override void Update(float dt)
        {
            base.Update(dt);
            Behaviour.Evaluate(this);
        }
    }
#endif
}
