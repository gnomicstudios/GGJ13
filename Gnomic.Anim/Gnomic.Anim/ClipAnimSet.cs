using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Gnomic.Anim
{
    public class ClipAnimSet
    {
        static Random s_rnd = new Random();

        [ContentSerializer(FlattenContent = true, CollectionItemName = "Anim")] 
        public List<ClipAnim> Anims = new List<ClipAnim>();

        private int animId = 0;
        private bool nextAnimRandom = false;

        public ClipAnim this[string animName]
        {
            get
            {
                foreach (ClipAnim anim in this.Anims)
                {
                    if (anim.Name == animName)
                        return anim;
                }
                return null;
            }
        }
        
        public ClipAnimSet CreateSingleAnimSet(string animName)
        {
            ClipAnimSet set = new ClipAnimSet();
            set.Anims.Add(this[animName]);
            return set;
        }

        public ClipAnimSet CreateSubSet(bool playRandomly, params string[] animNames)
        {
            nextAnimRandom = playRandomly;
            ClipAnimSet set = new ClipAnimSet();
            foreach (string an in animNames)
            {
                set.Anims.Add(this[an]);
            }
            return set;
        }

        [ContentSerializerIgnore()]
        public ClipAnim CurrentAnim
        {
            get { return Anims[animId]; }
        }

        public ClipAnim GetNextAnim()
        {
            if (nextAnimRandom)
            {
                return GetRandomAnim(true);
            }
            else
            {
                if (Anims == null || Anims.Count == 0)
                    return null;

                animId++;
                animId %= Anims.Count;
                return Anims[animId];
            }
        }

        public ClipAnim GetRandomAnim(bool allowSameAsLast)
        {
            if (!allowSameAsLast)
            {
                int animIdLast = animId;
                while (animIdLast == animId)
                {
                    animId = s_rnd.Next(Anims.Count);
                }
            }
            else
            {
                animId = s_rnd.Next(Anims.Count);
            }

            return Anims[animId];
        }
    }
}
