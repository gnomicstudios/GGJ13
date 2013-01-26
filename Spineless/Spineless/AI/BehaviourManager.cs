using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Spineless;
using Spineless.Entities;

namespace Spineless.AI
{
    class BehaviourManager
    {
        private Dictionary<UnitType, object> behaviours =
            new Dictionary<UnitType, object>();

        public void Add<T>(UnitType type, Behaviour<T> beh)
        {
            behaviours[type] = (object)beh;
        }

        public Behaviour<T> Create<T>(UnitType type)
        {
            if (behaviours.ContainsKey(type))
            {
                return (Behaviour<T>)behaviours[type];
            }

            return new IdentityBehaviour<T>();
        }

        public BehaviourManager()
        {
            Behaviour<IBaseUnit> beh = null;

            #region UnitType.Grunt

            beh = new Selector<IBaseUnit> {
                Children =
                {
                    new Sequence<IBaseUnit> {
                        Children =
                        {
                            new IsDeadCheck<IBaseUnit>(),
                            new PlayAnimationAction<IBaseUnit>("death", false, false, 1.0f),
                            new SetBodyEnabledAction<IBaseUnit>(false)
                        }
                    },
                    new Sequence<IBaseUnit> {
                        Children =
                        {
                            new CloseToSelectedCheck<IBaseUnit>(60.0f),
                            new StopAction<IBaseUnit>(),
                            new PlayAnimationAction<IBaseUnit>("attack", true, false, 1.0f),
                            new AttackAction<IBaseUnit>()
                        }
                    },
                    new Sequence<IBaseUnit> {
                        Children =
                        {
                            new SelectNearestTypeAction<IBaseUnit>(UnitType.Knight),
                            new MoveTowardsSelectedAction<IBaseUnit>()
                        }
                    }
                }
            };

            Add(UnitType.Grunt, beh);

            #endregion

            #region UnitType.Knight

            beh = new Selector<IBaseUnit> {
                Children =
                {
                    new Sequence<IBaseUnit> {
                        Children =
                        {
                            new CloseToSelectedCheck<IBaseUnit>(60.0f),
                            new StopAction<IBaseUnit>(),
                            new PlayAnimationAction<IBaseUnit>("attackA", true, false, 1.0f),
                            new AttackAction<IBaseUnit>()
                        }
                    },
                    new Sequence<IBaseUnit> {
                        Children =
                        {
                            new SelectNearestTypeAction<IBaseUnit>(UnitType.Grunt),
                            new MoveTowardsSelectedAction<IBaseUnit>()
                        }
                    }
                }
            };

            Add(UnitType.Knight, beh);

            #endregion

            #region UnitType.Captain

            beh = new SelectNearestTypeAction<IBaseUnit>(UnitType.Knight);
            Add(UnitType.Captain, beh);

            #endregion

            #region UnitType.Boss

            beh = new SelectNearestTypeAction<IBaseUnit>(UnitType.Knight);
            Add(UnitType.Boss, beh);

            #endregion
        }
    }
}
