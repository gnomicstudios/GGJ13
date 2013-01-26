using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Spineless.Entities;

namespace Spineless.AI
{
    public interface IAggressive : IHealthy, IPerceptive
    {
        float AttackInterval { get; set; }
        float AttackTimer { get; set; }
        float Damage { get; set; }
    }

    public class AttackAction<T> : Behaviour<T> where T: IAggressive
    {
        public override bool Evaluate(T entity, float dt)
        {
            if (entity.AttackTimer < entity.AttackInterval)
            {
                entity.AttackTimer += dt;
                return false;
            }

            Unit target = entity.Selected;
            target.Health = Math.Max(0, target.Health - entity.Damage);

            entity.AttackTimer = 0.0f;

            return true;
        }
    }
}
