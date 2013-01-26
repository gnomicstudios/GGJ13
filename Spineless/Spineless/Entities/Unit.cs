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
        Boss
    }

    public class Unit : SpinelessEntity, IMoveable
    {
        public UnitType UnitType;
        public bool IsAdded;

        #region IMoveable

        public Vector2 Heading { get; set; }
        public float Speed { get; set; }

        public void MoveTowards()
        {
            physics.ApplyForceToBody(0, Speed, Heading);
        }

        #endregion
    }
}
