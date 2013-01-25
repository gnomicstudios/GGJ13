#if ANDROID || IOS || WINRT || WINDOWS_PHONE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Gnomic
{
    public enum TouchStickStyle
    {
        FreeFollow,
        Free, 
        Fixed
    }

    public static class TouchTwinStick
    {
        /// <summary>
        /// Tap gestures can not be detected whilst another touch is active. TapStart struct is
        /// used to store the time, id and position of touch down events so we can decide later if they are taps
        /// </summary>
        struct TapStart
        {
            public int Id;
            public double Time;
            public Vector2 Pos;

            public TapStart(int id, double time, Vector2 pos)
            {
                Id = id;
                Time = time;
                Pos = pos;
            }
        }

        // Total game time
        static double totalTime;

        // How far from the alive zone we can get before the touch stick starts to follow in FreeFollow mode
        const float aliveZoneFollowFactor = 1.3f;

        // How quickly the touch stick follows in FreeFollow mode
        const float aliveZoneFollowSpeed = 0.05f;

        // Current state of the TouchPanel
        static TouchCollection state;

        // If we let the touch origin get too close to the screen edge,
        // the direction is less accurate, so push it away from the edge.
        static float DistFromScreenEdge;

        static float deadZoneSizeLeft;
        public static float DeadZoneSizeLeft
        {
            get { return deadZoneSizeLeft; }
            set { deadZoneSizeLeft = value; }
        }

        static float deadZoneSizeRight;
        public static float DeadZoneSizeRight
        {
            get { return deadZoneSizeRight; }
            set { deadZoneSizeRight = value; }
        }

        static float aliveZoneSize;
        public static float AliveZoneSize
        {
            get { return aliveZoneSize; }
            set { aliveZoneSize = value; }
        }

#region LEFT touch stick properties
        static TouchLocation? leftStick = null;
        public static bool UsingLeftStick { get { return leftStick.HasValue; } }

        public static Rectangle leftStickStartRegion;
        public static Vector2 leftStartLocation;
        public static Vector2 leftFixedLocation;
        public static Vector2 leftStickDirection;
        public static Vector2 leftStickPos;
        public static float leftStickMagnitude;

        static TouchStickStyle leftStickStyle;
        public static TouchStickStyle LeftStickStyle
        {
            get { return leftStickStyle; }
            set 
            { 
                leftStickStyle = value;
                if (leftStickStyle == TouchStickStyle.Fixed)
                    leftStartLocation = leftFixedLocation;
            }
        }
#endregion

#region RIGHT touch stick properties
        static TouchLocation? rightStick = null;
        public static bool UsingRightStick { get { return rightStick.HasValue; } }

        public static Rectangle rightStickStartRegion;
        public static List<Rectangle> rightStickStartExcludeRegions = new List<Rectangle>(5);
        static int lastExcludedRightTouchId = -1;
        public static Vector2 rightStartLocation;
        public static Vector2 rightFixedLocation;
        public static Vector2 rightStickDirection;
        public static Vector2 rightStickPos;
        public static float rightStickMagnitude;

        static TouchStickStyle rightStickStyle;
        public static TouchStickStyle RightStickStyle
        {
            get { return rightStickStyle; }
            set 
            {
                rightStickStyle = value;
                if (rightStickStyle == TouchStickStyle.Fixed)
                    rightStartLocation = rightFixedLocation;
            }
        }
#endregion

        static bool justTouched = false;
        public static bool JustTouched
        {
            get { return justTouched; }
            set { justTouched = value; }
        }
        static Vector2 touchStartPos;
        public static Vector2 TouchStartPos
        {
            get { return touchStartPos; }
            set { touchStartPos = value; }
        }
        static int? touchStartId = null;
        public static int? TouchStartId
        {
            get { return touchStartId; }
            set { touchStartId = value; }
        }

        static bool justTapped = false;
        public static bool JustTapped
        {
            get { return justTapped; }
            set { justTapped = value; }
        }
        static Vector2 tapPosition;
        public static Vector2 TapPosition
        {
            get { return tapPosition; }
            set { tapPosition = value; }
        }

        static TapStart[] tapStarts = new TapStart[4];
        static int tapStartCount = 0;
        
        public static int TouchCount
        {
            get
            {
                return state.Count;
            }
        }
        public static Vector2 FirstTouch
        {
            get
            {
                if (state.Count > 0)
                    return state[0].Position;
                else
                    return Vector2.Zero;
            }
        }
        public static int? FirstTouchId
        {
            get
            {
                if (state.Count > 0)
                    return state[0].Id;
                else
                    return null;
            }
        }

        static TouchTwinStick()
        {
            //TouchPanel.EnabledGestures = GestureType.None;
            //TouchPanel.DisplayOrientation = DisplayOrientation.LandscapeLeft;

            DeadZoneSizeLeft = 5.0f;
            DeadZoneSizeRight = 5.0f;
            DistFromScreenEdge = 25.0f;
            AliveZoneSize = 65.0f;

            leftStickStartRegion = new Rectangle(
                0, 100,
                (int)(TouchPanel.DisplayWidth * 0.3f), (int)TouchPanel.DisplayHeight - 100);

            // Make the Fixed mode left origin in the bottom left
            leftFixedLocation = new Vector2(
                AliveZoneSize * aliveZoneFollowFactor, 
                TouchPanel.DisplayHeight - AliveZoneSize * aliveZoneFollowFactor);

            rightStickStartRegion = new Rectangle(
                (int)(TouchPanel.DisplayWidth * 0.5f), 100,
                (int)(TouchPanel.DisplayWidth * 0.5f), (int)TouchPanel.DisplayHeight - 100);

            // Make the Fixed mode right origin in the bottom right
            rightFixedLocation = new Vector2(
                TouchPanel.DisplayWidth - AliveZoneSize * aliveZoneFollowFactor,
                TouchPanel.DisplayHeight - AliveZoneSize * aliveZoneFollowFactor);
        }


        public static void ClearTaps()
        {
            tapStartCount = 0;
        }

        public static bool TryGetTouchPos(int touchId, out Vector2 pos)
        {
            foreach (TouchLocation loc in state)
            {
                if (loc.Id == touchId)
                {
                    pos = loc.Position;
                    return true;
                }
            }
            pos = Vector2.Zero;
            return false;
        }
                
        public static bool TryGetTouch(Rectangle rect, out int touchId, out Vector2 pos)
        {
            foreach (TouchLocation loc in state)
            {
                if (rect.Contains((int)loc.Position.X, (int)loc.Position.Y))
                {
                    touchId = loc.Id;
                    pos = loc.Position;
                    return true;
                }
            }
            pos = Vector2.Zero;
            touchId = -1;
            return false;
        }

        /// <summary>
        /// Returns a GamePadState from the current left and right stick directions. Aids in porting Xbox games.
        /// </summary>
        /// <returns></returns>
        public static GamePadState GetGamePadState()
        {
            // Get gamepad 0 state, simply to detect the WP7 hardware Back button
            GamePadState gs0 = GamePad.GetState(0);

            GamePadState gs = new GamePadState(
                new GamePadThumbSticks(leftStickDirection, rightStickDirection),
                new GamePadTriggers(0.0f, 0.0f),
                gs0.Buttons,
                new GamePadDPad());

            return gs;
        }


        public static void Update(float dt)
        {
            totalTime += dt;

            justTouched = false;
            justTapped = false;
            touchStartId = null;

            //while (TouchPanel.IsGestureAvailable)
            //{
            //    GestureSample gesture = TouchPanel.ReadGesture();
            //    if (gesture.GestureType == GestureType.Tap)
            //    {
            //        justTapped = true;
            //        tapPosition = gesture.Position;
            //    }
            //}

            //stateLast = state;
            state = TouchPanel.GetState();
            TouchLocation? leftTouch = null, rightTouch = null;

            if (tapStartCount > state.Count)
            {
                tapStartCount = state.Count;

                // Workaround. Sometimes very fast taps won't be registered as TouchLocations with state of Released
                // meaning the algorithm in the for loop below falls down :(
                // Here we assume that only one tap was missed
                if (state.Count == 0)
                {
                    justTapped = true;
                    tapPosition = tapStarts[0].Pos;
                }
            }

            foreach (TouchLocation loc in state)
            {
                if (loc.State == TouchLocationState.Released)
                {
                    int tapStartId = -1;
                    for (int i = 0; i < tapStartCount; ++i)
                    {
                        if (tapStarts[i].Id == loc.Id)
                        {
                            // This touch was released. Check if it was a tap
                            tapStartId = i;

                            // COMMENTED CODE WAS TO ENSURE TAPS ARE NOT REGISTERED FOR HOLDS (LONG TAPS)
                            //if ((Engine.Instance.TimeTotal - tapStarts[i].Time) < 1.0f)
                            //{
                                justTapped = true;
                                tapPosition = loc.Position;
                            //}
                            //else
                            //{
                            //  System.Diagnostics.Debug.WriteLine("Rejected touch: Held too long");
                            //}
                            
                            break;
                        }
                    }
                    if (tapStartId >= 0)
                    {
                        // Remove the tap start as it has been released
                        for (int i = tapStartId; i < tapStartCount - 1; ++i)
                            tapStarts[i] = tapStarts[i + 1];

                        tapStartCount--;
                    }
                    continue;
                }
                else if (loc.State == TouchLocationState.Pressed && tapStartCount < tapStarts.Length)
                {
                    // Started new touch
                    tapStarts[tapStartCount] = new TapStart(loc.Id, totalTime, loc.Position);
                    tapStartCount++;
                    justTouched = true;
                    touchStartId = loc.Id;
                    touchStartPos = loc.Position;
                }
                // COMMENTED CODE WAS TO REMOVE TAPS THAT DEVIATE TOO FAR FROM THEIR ORIGINAL POSITION
                //else
                //{
                //    int removeTapId = -1;
                //    for (int i = 0; i < tapStartCount; ++i)
                //    {
                //        if (tapStarts[i].Id == loc.Id)
                //        {
                //            // Remove any tap that deviates too far from it's original position
                //            float distSqr = Vector2.DistanceSquared(tapStarts[i].Pos, loc.Position);
                //            if (distSqr > 3600.0f)
                //            {
                //                //System.Diagnostics.Debug.WriteLine("Rejected touch: Deviated too far");
                //                removeTapId = i;
                //            }
                //            break;
                //        }
                //    }
                //    if (removeTapId >= 0)
                //    {
                //        // Remove the tap start as it has moved further than is valid
                //        for (int i = removeTapId; i < tapStartCount - 1; ++i)
                //            tapStarts[i] = tapStarts[i + 1];

                //        tapStartCount--;
                //    }
                //}

                if (leftStick.HasValue && loc.Id == leftStick.Value.Id)
                {
                    // Continue left touch
                    leftTouch = loc;
                    continue;
                }
                if (rightStick.HasValue && loc.Id == rightStick.Value.Id)
                {
                    // Continue right touch
                    rightTouch = loc;
                    continue;
                }

                TouchLocation locPrev;
                if (!loc.TryGetPreviousLocation(out locPrev))
                    locPrev = loc;

                if (!leftStick.HasValue)
                {
                    // if we are not currently tracking a left thumbstick and this touch is on the left
                    // half of the screen, start tracking this touch as our left stick
                    if (leftStickStartRegion.Contains((int)locPrev.Position.X, (int)locPrev.Position.Y))
                    {
                        if (leftStickStyle == TouchStickStyle.Fixed)
                        {
                            if (Vector2.Distance(locPrev.Position, leftStartLocation) < aliveZoneSize)
                            {
                                leftTouch = locPrev;
                            }
                        }
                        else
                        {
                            leftTouch = locPrev;
                            leftStartLocation = leftTouch.Value.Position;

                            if (leftStartLocation.X < leftStickStartRegion.Left + DistFromScreenEdge)
                                leftStartLocation.X = leftStickStartRegion.Left + DistFromScreenEdge;
                            if (leftStartLocation.Y > leftStickStartRegion.Bottom - DistFromScreenEdge)
                                leftStartLocation.Y = leftStickStartRegion.Bottom - DistFromScreenEdge;
                        }
                        continue;
                    }
                }

                if (!rightStick.HasValue && locPrev.Id != lastExcludedRightTouchId)
                {
                    // if we are not currently tracking a right thumbstick and this touch is on the right
                    // half of the screen, start tracking this touch as our right stick
                    if (rightStickStartRegion.Contains((int)locPrev.Position.X, (int)locPrev.Position.Y))
                    {
                        // Check if any of the excluded regions contain the point
                        bool excluded = false;
                        foreach (Rectangle r in rightStickStartExcludeRegions)
                        {
                            if (r.Contains((int)locPrev.Position.X, (int)locPrev.Position.Y))
                            {
                                excluded = true;
                                lastExcludedRightTouchId = locPrev.Id;
                                continue;
                            }
                        }

                        if (excluded)
                            continue;

                        lastExcludedRightTouchId = -1;

                        if (rightStickStyle == TouchStickStyle.Fixed)
                        {
                            if (Vector2.Distance(locPrev.Position, rightStartLocation) < aliveZoneSize)
                            {
                                rightTouch = locPrev;
                            }
                        }
                        else
                        {
                            rightTouch = locPrev;
                            rightStartLocation = rightTouch.Value.Position;

                            // Ensure touch is not too close to screen edge
                            if (rightStartLocation.X > rightStickStartRegion.Right - DistFromScreenEdge)
                                rightStartLocation.X = rightStickStartRegion.Right - DistFromScreenEdge;
                            if (rightStartLocation.Y > rightStickStartRegion.Bottom - DistFromScreenEdge)
                                rightStartLocation.Y = rightStickStartRegion.Bottom - DistFromScreenEdge;
                        }
                        continue;
                    }
                }
            }

            if (leftTouch.HasValue)
            {
                leftStick = leftTouch;
                leftStickPos = leftTouch.Value.Position;
                EvaluateLeftPoint(leftStickPos, dt);
            }
            else
            {
                bool foundNew = false;
                if (leftStick.HasValue)
                {
                    // No left touch now but previously there was. Check to see if the TouchPanel decided
                    // to reset our touch id. Search for any touch within 10 pixel radius.
                    foreach (TouchLocation loc in state)
                    {
                        Vector2 pos = loc.Position;
                        float distSqr; Vector2.DistanceSquared(ref pos, ref leftStickPos, out distSqr);
                        if (distSqr < 100f)
                        {
                            foundNew = true;
                            leftStick = loc;
                            leftStickPos = loc.Position;
                            EvaluateLeftPoint(leftStickPos, dt);
                        }
                    }
                }

                if (!foundNew)
                {
                    leftStick = null;
                    leftStickDirection = Vector2.Zero;
                    leftStickMagnitude = 0.0f;
                }
            }

            if (rightTouch.HasValue)
            {
                rightStick = rightTouch;
                rightStickPos = rightTouch.Value.Position;
                EvaluateRightPoint(rightStickPos, dt);
            }            
            else
            {
                bool foundNew = false;
                if (rightStick.HasValue)
                {
                    // No right touch now but previously there was. Check to see if the TouchPanel decided
                    // to reset our touch id. Search for any touch within 10 pixel radius.
                    foreach (TouchLocation loc in state)
                    {
                        Vector2 pos = loc.Position;
                        float distSqr; Vector2.DistanceSquared(ref pos, ref rightStickPos, out distSqr);
                        if (distSqr < 100f)
                        {
                            foundNew = true;
                            rightStick = loc;
                            rightStickPos = loc.Position;
                            EvaluateRightPoint(rightStickPos, dt);
                        }
                    }
                }

                // 
                if (!foundNew)
                {
                    rightStick = null;
                    rightStickDirection = Vector2.Zero;
                    rightStickMagnitude = 0.0f;
                }
            }

        }


        /// <summary>
        /// Calculate the left stick's direction and magnitude
        /// </summary>
        /// <param name="pos">The current left touch position</param>
        static void EvaluateLeftPoint(Vector2 pos, float dt)
        {
            leftStickDirection = pos - leftStartLocation;
            float leftStickLength = leftStickDirection.Length();
            if (leftStickLength <= deadZoneSizeLeft)
            {
                leftStickDirection = Vector2.Zero;
                leftStickMagnitude = 0.0f;
            }
            else
            {
                leftStickDirection.Normalize();
                leftStickDirection.Y *= -1.0f;
                if (leftStickLength < aliveZoneSize)
                {
                    leftStickMagnitude = leftStickLength / aliveZoneSize;
                    leftStickDirection = new Vector2(leftStickDirection.X * leftStickMagnitude, leftStickDirection.Y * leftStickMagnitude);
                }
                else
                {
                    leftStickMagnitude = 1.0f;

                    if (leftStickStyle == TouchStickStyle.FreeFollow &&
                        leftStickLength > aliveZoneSize * aliveZoneFollowFactor)
                    {
                        Vector2 targetLoc = new Vector2(
                                    pos.X - leftStickDirection.X * aliveZoneSize * aliveZoneFollowFactor,
                                    pos.Y + leftStickDirection.Y * aliveZoneSize * aliveZoneFollowFactor);

                        Vector2.Lerp(ref leftStartLocation, ref targetLoc,
                                     (leftStickLength - aliveZoneSize * aliveZoneFollowFactor) * aliveZoneFollowSpeed * dt,
                                     out leftStartLocation);

                        if (leftStartLocation.X > leftStickStartRegion.Width)
                            leftStartLocation.X = (float)leftStickStartRegion.Width;
                        if (leftStartLocation.Y < leftStickStartRegion.Top)
                            leftStartLocation.Y = (float)leftStickStartRegion.Top;

                        if (leftStartLocation.X < leftStickStartRegion.Left + DistFromScreenEdge)
                            leftStartLocation.X = leftStickStartRegion.Left + DistFromScreenEdge;
                        if (leftStartLocation.Y > leftStickStartRegion.Bottom - DistFromScreenEdge)
                            leftStartLocation.Y = leftStickStartRegion.Bottom - DistFromScreenEdge;
                    }
                }
            }
        }

        /// <summary>
        /// Calculate the right stick's direction and magnitude
        /// </summary>
        /// <param name="pos">The current left touch position</param>
        static void EvaluateRightPoint(Vector2 pos, float dt)
        {
            rightStickDirection = pos - rightStartLocation;
            float rightStickLength = rightStickDirection.Length();
            if (rightStickLength <= deadZoneSizeRight)
            {
                rightStickDirection = Vector2.Zero;
                rightStickMagnitude = 0.0f;
            }
            else
            {
                rightStickDirection.Normalize();
                rightStickDirection.Y *= -1.0f;
                if (rightStickLength < aliveZoneSize)
                {
                    rightStickMagnitude = rightStickLength / aliveZoneSize;
                    rightStickDirection = new Vector2(rightStickDirection.X * rightStickMagnitude, rightStickDirection.Y * rightStickMagnitude);
                }
                else 
                {
                    rightStickMagnitude = 1.0f;

                    if (rightStickStyle == TouchStickStyle.FreeFollow &&
                        rightStickLength > aliveZoneSize * aliveZoneFollowFactor)
                    {
                        Vector2 targetLoc = new Vector2(
                                                pos.X - rightStickDirection.X * aliveZoneSize * aliveZoneFollowFactor,
                                                pos.Y + rightStickDirection.Y * aliveZoneSize * aliveZoneFollowFactor);

                        Vector2.Lerp(ref rightStartLocation, ref targetLoc,
                                     (rightStickLength - aliveZoneSize * aliveZoneFollowFactor) * aliveZoneFollowSpeed * dt,
                                     out rightStartLocation);

                        if (rightStartLocation.X < rightStickStartRegion.Left)
                            rightStartLocation.X = (float)rightStickStartRegion.Left;
                        if (rightStartLocation.Y < rightStickStartRegion.Top)
                            rightStartLocation.Y = (float)rightStickStartRegion.Top;

                        if (rightStartLocation.X > rightStickStartRegion.Right - DistFromScreenEdge)
                            rightStartLocation.X = rightStickStartRegion.Right - DistFromScreenEdge;
                        if (rightStartLocation.Y > rightStickStartRegion.Bottom - DistFromScreenEdge)
                            rightStartLocation.Y = rightStickStartRegion.Bottom - DistFromScreenEdge;
                    }
                }
            }
        }
    }
}
#endif



