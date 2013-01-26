using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spineless.AI
{
    public interface IBaseUnit : IMoveable, IPerceptive, IAnimated,
        IHealthy, IAggressive
    {

    }

    public class MoveTowardsSelectedAction<T> : Behaviour<T> where T: IBaseUnit
    {
        public override bool Evaluate(T entity, float dt)
        {
            entity.MoveTowards(entity.Selected.Position);
            return true;
        }
    }

    public class CloseToSelectedCheck<T> : Behaviour<T> where T: IBaseUnit
    {
        float _dist;
        public CloseToSelectedCheck(float dist) { _dist = dist; }
        public override bool Evaluate(T entity, float dt)
        {
            if (entity.Selected == null)
            {
                return false;
            }

            float length = (entity.Selected.Position - entity.Position).Length();

            return length < _dist;
        }
    }
}
