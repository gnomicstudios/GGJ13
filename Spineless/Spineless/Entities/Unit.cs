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
        public int LaneId = 0;

        bool isAlive;

        public Unit()
        {
            AttackTimer = 0.0f;
        }

        protected override void OnActivate()
        {
            isAlive = true;
            base.OnActivate();
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            Behaviour.Evaluate((IBaseUnit)this, dt);

            if (isAlive)
            {
                if (Health <= 0.0f)
                {
                    isAlive = false;
                    Deactivate(2.0f);
                }
            }
        }

        #region IMoveable

        public float Speed { get; set; }

        public void MoveTowards(Vector2 target)
        {
            Vector2 direction = target - Position;
            if (direction.Length() > 0.01f)
            {
                direction.Normalize();

                physics.ApplyForceToBody(0, Speed, direction);
            }
        }

        public void Stop()
        {
            physics.ZeroVelocity();
        }

        public void EnableBody(bool enabled)
        {
            physics.Enabled = enabled;
        }

        #endregion

        #region IPerceptive

        public Unit Selected { get; set; }

        public void SelectNearestType(UnitType type)
        {
            Selected = this.UnitManager.FindNearestTypeTo(type, this);
        }

        #endregion

        #region IAggressive

        public float AttackInterval { get; set; }
        public float AttackTimer { get; set; }
        public float Damage { get; set; }

        #endregion

        #region IHealthy

        public float Health 
        { 
            get; 
            set; 
        }

        #endregion


    }
}
