using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Spineless.AI;

namespace Spineless.Entities
{
    public enum UnitType
    {
        Grunt,
        Captain,
        Boss,
        Knight
    }

    public interface IBaseUnit : IMoveable, IPerceptive { }

    public class Unit : SpinelessEntity, IBaseUnit
    {
        public UnitType UnitType;
        public bool IsAdded;
        public UnitManager UnitManager;
        public Behaviour<IBaseUnit> Behaviour;

        public override void Update(float dt)
        {
            base.Update(dt);
            Behaviour.Evaluate(this);
        }

        #region IMoveable

#if false
        public Vector2 Heading { get; set; }
        public float Speed { get; set; }

        public void MoveTowards()
        {
            physics.ApplyForceToBody(0, Speed, Heading);
        }
#endif

        #endregion

        #region IPerceptive

        public Unit Selected { get; set; }

        public void SelectNearestType(UnitType type)
        {
            Selected = this.UnitManager.FindNearestTypeTo(type, this);
        }

        #endregion
    }
}
