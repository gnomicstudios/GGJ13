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
        
        void EnableBody(bool enabled);
        void MoveTowards(Vector2 position);
        void Stop();
    }

    public class StopAction<T> : Behaviour<T> where T: IMoveable
    {
        public override bool Evaluate(T entity, float dt)
        {
            entity.Stop();
            return true;
        }
    }

    public class SetSpeedAction<T> : Behaviour<T> where T: IMoveable
    {
        float _speed;

        public SetSpeedAction(float speed) { _speed = speed; }
        public override bool Evaluate(T entity, float dt)
        {
            entity.Speed = _speed;
            return true;
        }
    }

    public class SetBodyEnabledAction<T> : Behaviour<T> where T: IMoveable
    {
        bool _enabled;

        public SetBodyEnabledAction(bool enabled) { _enabled = enabled; }
        public override bool Evaluate(T entity, float dt)
        {
            entity.EnableBody(_enabled);
            return true;
        }
    }
}
