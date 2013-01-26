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
    }

    public class PlayAnimationAction<T> : Behaviour<T> where T: IAnimated
    {
        string _name;
        bool _looping;
        bool _forceRestart;
        float _speedMod;

        public PlayAnimationAction(
            string name, bool looping, bool forceRestart, float speedMod)
        {
            _name     = name;
            _looping  = looping;
            _forceRestart = forceRestart;
            _speedMod = speedMod;
        }

        public override bool Evaluate(T entity, float dt)
        {
            string curr = entity.ClipInstance.CurrentAnimationName;

            if (!_forceRestart && curr.Contains(_name))
            {
                return true;    // already playing
            }

            entity.ClipInstance.Play(_name, _looping, _speedMod);
            return true;
        }
    }
}
