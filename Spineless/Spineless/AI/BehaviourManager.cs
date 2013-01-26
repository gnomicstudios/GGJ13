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

            beh = new SelectNearestTypeAction<IBaseUnit>(UnitType.Knight);
            Add(UnitType.Grunt, beh);

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
