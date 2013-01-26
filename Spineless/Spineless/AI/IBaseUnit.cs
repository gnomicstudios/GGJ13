using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spineless.AI
{
    public interface IBaseUnit : IMoveable, IPerceptive
    {

    }

    public class MoveTowardsSelectedAction<T> : Behaviour<T> where T: IBaseUnit
    {
        public override bool Evaluate(T entity)
        {
            entity.MoveTowards(entity.Selected.Position);
            return true;
        }
    }
}
