using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Spineless;

namespace Spineless.AI
{
    public interface IMoveable
    {
        Vector2 Position { get; set; }
        float Speed { get; set; }

        void MoveTowards(Vector2 position);
    }

    public class SetSpeedAction<T> : Behaviour<T> where T: IMoveable
    {
        float _speed;

        public SetSpeedAction(float speed) { _speed = speed; }
        public override bool Evaluate(T entity)
        {
            entity.Speed = _speed;
            return true;
        }
    }
}
