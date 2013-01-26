using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spineless.Entities
{
    public enum UnitType
    {
        Grunt,
        Captain,
        Boss
    }

    public class Unit : SpinelessEntity
    {
        public UnitType UnitType;
        public bool IsAdded;

    }
}
