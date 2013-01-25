using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gnomic;
using Spineless.AI;

namespace Spineless.Entities
{
    public abstract class AIControlledEntity : SpinelessEntity
    {
        Behaviour<AIControlledEntity> m_behaviour;

        public abstract Behaviour<AIControlledEntity> CreateBehaviour();

        public override void Initialize(GameScreen parentScreen)
        {
            base.Initialize(parentScreen);
            m_behaviour = CreateBehaviour();
        }
        
        public override void Update(float dt)
        {
            base.Update(dt);
            m_behaviour.Evaluate(this);
        }
    }
}
