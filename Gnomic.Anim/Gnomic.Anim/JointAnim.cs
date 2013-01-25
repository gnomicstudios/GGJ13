using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Gnomic.Anim
{
    public abstract class JointAnim
    {
        public string JointName;
        [ContentSerializer(Optional=true)]
        public bool IsLooping = true;

        public abstract int FrameCount { get; }
        public abstract int GetFrameNumber(int i);

        protected Joint joint;
        [ContentSerializerIgnore()]
        public int JointId;

        protected ClipAnim parentClipAnim;
        [ContentSerializerIgnore()]
        public ClipAnim ParentClipAnim { get { return parentClipAnim; } }

        public virtual void ApplySate(int currentKeyframeIndex, int nextKeyframeIndex, float lerpValue, ref SpriteState jointState) { }
        public virtual void Init(ContentManager content, ClipAnim clipAnim)
        {
            parentClipAnim = clipAnim;
            for (int i = 0; i < parentClipAnim.ParentClip.Joints.Length; ++i)
            {
                Joint j = parentClipAnim.ParentClip.Joints[i];
                if (j.Name == JointName)
                {
                    joint = j;
                    JointId = i;
                    break;
                }
            }
        }


        public abstract JointAnimState<JointAnim> CreateState();

    }
}
