using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Spineless.Entities;

namespace Spineless.AI
{
    public interface IPerceptive
    {
        Unit Selected { get; set; }
        void SelectNearestType(UnitType type);
    }

    public class SelectNearestTypeAction<T> : Behaviour<T> where T: IPerceptive
    {
        UnitType _type;

        public SelectNearestTypeAction(UnitType type)
        {
            _type = type;
        }

        public override bool Evaluate(T entity)
        {
            entity.SelectNearestType(_type);
            return true;
        }
    }
}
