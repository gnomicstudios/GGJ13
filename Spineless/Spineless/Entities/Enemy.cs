using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spineless.Entities
{
    public enum EnemyType
    {
        Grunt,
        Captain,
        Boss
    }

    public class Enemy : SpinelessEntity
    {
        public EnemyType EnemyType;
        public bool IsAdded;

    }
}
