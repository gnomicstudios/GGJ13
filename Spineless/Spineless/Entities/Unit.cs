using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Spineless.AI;
using FarseerPhysics.Dynamics;

namespace Spineless.Entities
{
    public enum UnitType
    {
        Grunt,
        Captain,
        Boss,
        Knight
    }

    public enum UnitState
    {
        Idle,
        Move,
        Attack,
        AttackOrMove,
        Hit,
        Die
    }

    public class Unit : SpinelessEntity //, IBaseUnit
    {
        const float ATTACK_RANGE = 60;

        public UnitType UnitType;
        public bool IsAdded;
        public UnitManager UnitManager;
        //public Behaviour<IBaseUnit> Behaviour;
        public int LaneId = 0;

        public bool IsAlive;

        UnitState currentState;
        UnitState nextState;
        float countdownToNextState = 0.0f;
        static Random random = new Random();
        Unit targetUnit = null;

        public bool IsEnemy
        {
            get { return UnitType != Entities.UnitType.Knight; }
        }

        public Unit()
        {
            AttackTimer = 0.0f;
        }

        protected override void OnActivate()
        {
            SwitchToState(UnitState.Idle);
            IsAlive = true;
            base.OnActivate();
        }
        
        void TransitionToState(UnitState state, float delay)
        {
            countdownToNextState = delay;
            nextState = state;
        }

        void SwitchToState(UnitState state)
        {
            // cancel any pending transition
            countdownToNextState = 0.0f;
            currentState = state;
            switch (state)
            {
                case UnitState.Idle:
                    ClipInstance.Play("idle", true);
                    TransitionToState(UnitState.AttackOrMove, 0.5f);
                    break;
                case UnitState.Move:
                    ClipInstance.Play("walk", true);
                    TransitionToState(UnitState.AttackOrMove, 0.5f);
                    break;
                case UnitState.AttackOrMove:
                    AttackOrMove();
                    break;
                case UnitState.Attack:
                    {
                        if (IsEnemy)
                            ClipInstance.Play("attack", false);
                        else
                            if (random.Next(2) == 0)
                                ClipInstance.Play("attackA", false);
                            else
                                ClipInstance.Play("attackB", false);

                        targetUnit.Hit(this.Damage);
                        TransitionToState(UnitState.Idle, ClipInstance.CurrentAnim.DurationInSeconds);
                    }
                    break;
                case UnitState.Hit:
                    ClipInstance.Play("hit", false);
                    TransitionToState(UnitState.Idle, ClipInstance.CurrentAnim.DurationInSeconds);
                    break;
                case UnitState.Die:
                    ClipInstance.Play("death", false);
                    Deactivate(ClipInstance.CurrentAnim.DurationInSeconds + 3.0f);
                    IsAlive = false;
                    Physics.Bodies[0].CollidesWith = (Category)SpinelessCollisionCategories.Terrain;
                    break;
            }
        }

        private void Hit(float dmg)
        {
            if (IsAlive)
            {
                Health -= dmg;
                if (Health < 0.0f)
                {
                    SwitchToState(UnitState.Die);
                }
            }
        }

        void AttackOrMove()
        {
            float dist;
            targetUnit = UnitManager.NearestEnemy(this, out dist);
            if (targetUnit != null && dist < ATTACK_RANGE)
            {
                SwitchToState(UnitState.Attack);
            }
            else
            {
                SwitchToState(UnitState.Move);
            }
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            //Behaviour.Evaluate((IBaseUnit)this, dt);

            if (countdownToNextState > 0.0f)
            {
                countdownToNextState -= dt;
                if (countdownToNextState <= 0.0f)
                    SwitchToState(nextState);
            }

            switch (currentState)
            {
                case UnitState.Idle:
                    break;
                case UnitState.Move:
                    physics.ApplyForceToBody(0, Speed, Vector2.UnitX * (IsEnemy ? -1.0f : 1.0f));
                    break;
            }

            //if (IsEnemy)
            //{
            //    UpdateEnemy();
            //}
            //else
            //{
            //    UpdateKnight();
            //}

            if (IsAlive)
            {
                if (Health <= 0.0f)
                {
                    IsAlive = false;
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
