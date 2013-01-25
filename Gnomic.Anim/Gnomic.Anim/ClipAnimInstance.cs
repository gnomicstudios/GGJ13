using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gnomic.Anim
{
    public enum AnimPlayingState
    {
        Stopped,
        Playing,
        PlayingInReverse,
    }

    public class ClipAnimInstance
    {
        const float ENDPOINT_EPSILON = 0.0001f;
        public ClipAnim Anim;
        public float AnimPos;
        public bool Loop;
        
        public JointAnimState<JointAnim>[] JointAnimStates;

        public float DurationInSeconds
        {
            get { return durationInSeconds; }
        }
        public float DurationInSecondsRemaining
        {
            get 
            {
                if (playingState == AnimPlayingState.PlayingInReverse)
                    return AnimPos;
                else
                    return durationInSeconds - AnimPos; 
            }
        }

        ClipInstance parentClipInstance;
        float durationInSeconds;
        AnimPlayingState playingState = AnimPlayingState.Stopped;

        public ClipAnimInstance(ClipInstance clipInstance)
        {
            parentClipInstance = clipInstance;
            // There are potentially 3 types of animation per joint
            JointAnimStates = new JointAnimState<JointAnim>[parentClipInstance.Clip.Joints.Length * 3];
        }

        public void Play(ClipAnim anim)
        {
            Play(anim, true);
        }
        public void Play(ClipAnim anim, bool loop)
        {
            Play(anim, loop, true);
        }
        public void Play(ClipAnim anim, bool loop, bool forwards)
        {
            System.Diagnostics.Debug.Assert(anim != null);

            Anim = anim;
            AnimPos = 0.0f;
            Loop = loop;
            durationInSeconds = (float)Anim.Duration / Anim.Framerate;
            playingState = forwards ? AnimPlayingState.Playing : AnimPlayingState.PlayingInReverse;

            for (int i = 0; i < Anim.JointAnims.Count; ++i)
            {
                JointAnimStates[i] = Anim.JointAnims[i].CreateState();
            }
        }

        public void Update(float dt)
        {
            if (Anim != null)
            {
                if (playingState == AnimPlayingState.Stopped)
                    return;

                if (playingState == AnimPlayingState.Playing)
                {
                    AnimPos += dt;
                    if (AnimPos > durationInSeconds)
                    {
                        if (Loop)
                        {
                            while (AnimPos > durationInSeconds)
                                AnimPos -= durationInSeconds;
                        }
                        else
                        {
                            playingState = AnimPlayingState.Stopped;
                            dt = AnimPos - durationInSeconds - ENDPOINT_EPSILON;
                            AnimPos = durationInSeconds;
                        }
                    }
                }
                else if (playingState == AnimPlayingState.PlayingInReverse)
                {
                    dt = -dt;
                    AnimPos += dt;
                    if (AnimPos < 0.0f)
                    {
                        if (Loop)
                        {
                            while (AnimPos < 0.0f)
                                AnimPos += durationInSeconds;
                        }
                        else
                        {
                            playingState = AnimPlayingState.Stopped;
                            dt = dt - AnimPos + ENDPOINT_EPSILON;
                            AnimPos = 0.0f;
                        }
                    }
                }

                for (int i = 0; i < Anim.JointAnims.Count; ++i)
                {
                    JointAnimStates[i].Update(dt, ref parentClipInstance.JointStates[JointAnimStates[i].JointAnim.JointId]);
                }
            }
        }
    }
}
