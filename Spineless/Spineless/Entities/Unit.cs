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

    public class Unit : SpinelessEntity, IBaseUnit
    {
        public UnitType UnitType;
        public bool IsAdded;
        public UnitManager UnitManager;
        public Behaviour<IBaseUnit> Behaviour;

        public override void Update(float dt)
        {
            base.Update(dt);
            Behaviour.Evaluate((IBaseUnit)this);
        }

        #region IMoveable

        public float Speed { get; set; }

        public void MoveTowards(Vector2 target)
        {
            Vector2 direction = target - Position;
            direction.Normalize();

            physics.ApplyForceToBody(0, Speed, direction);
        }

        public void Stop()
        {
            
        }

        #endregion

        #region IPerceptive

        public Unit Selected { get; set; }

        public void SelectNearestType(UnitType type)
        {
            Selected = this.UnitManager.FindNearestTypeTo(type, this);
        }

        #endregion

        #region IBaseUnit

        #endregion
    }
}
