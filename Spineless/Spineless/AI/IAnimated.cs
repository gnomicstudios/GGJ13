using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gnomic.Anim;

namespace Spineless.AI
{
    public interface IAnimated
    {
        ClipInstance ClipInstance { get; set; }
        void PlayAnimation(string name, bool looping, float speedMod);
    }

    public class PlayAnimationAction<T> : Behaviour<T> where T: IAnimated
    {
        string _name; bool _looping; float _speedMod;
        public PlayAnimationAction(string name, bool looping, float speedMod)
        {
            _name     = name;
            _looping  = looping;
            _speedMod = speedMod;
        }

        public override bool Evaluate(T entity)
        {
            entity.ClipInstance.Play(_name, _looping, _speedMod);
            return true;
        }
    }
}
