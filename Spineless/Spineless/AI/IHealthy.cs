using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spineless.AI
{
    public interface IHealthy
    {
        float Health { get; set; }
    }

    public class IsAliveCheck<T> : Behaviour<T> where T: IHealthy
    {
        public override bool Evaluate(T entity, float dt)
        {
            return entity.Health > 0.0f;
        }
    }

    public class IsDeadCheck<T> : Behaviour<T> where T: IHealthy
    {
        public override bool Evaluate(T entity, float dt)
        {
            return entity.Health <= 0.0f;
        }
    }
}
