using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gnomic.Util
{
    public static class VectorUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitBasis"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2 VectorToUnitBasis(ref Vector2 unitBasis, ref Vector2 vector)
        {
            return new Vector2(
                unitBasis.X * vector.X - unitBasis.Y * vector.Y, 
                unitBasis.Y * vector.X + unitBasis.X * vector.Y);
        }

        public static void ProjectVectorOnUnit2D(ref Vector2 vec, ref Vector2 projectOnUnit, out Vector2 result)
        {
            float dp;
            Vector2.Dot(ref vec, ref projectOnUnit, out dp);
            result = new Vector2(dp * projectOnUnit.X, dp * projectOnUnit.Y);
        }

        public static void ProjectVector2D(ref Vector2 vec, ref Vector2 projectOn, out Vector2 result)
        {
            float dp;
            Vector2.Dot(ref vec, ref projectOn, out dp);
            float oneOnLenSqr = 1.0f / projectOn.LengthSquared();
            result = new Vector2(
                        dp * oneOnLenSqr * projectOn.X,
                        dp * oneOnLenSqr * projectOn.Y);
        }

        public static void ProjectVectorOnUnit3D(ref Vector3 vec, ref Vector3 projectOnUnit, out Vector3 result)
        {
            float dp;
            Vector3.Dot(ref vec, ref projectOnUnit, out dp);
            result = new Vector3(
                        dp * projectOnUnit.X, 
                        dp * projectOnUnit.Y, 
                        dp * projectOnUnit.Z);
        }

        public static void ProjectVector3D(ref Vector3 vec, ref Vector3 projectOn, out Vector3 result)
        {
            float dp;
            Vector3.Dot(ref vec, ref projectOn, out dp);
            float oneOnLenSqr = 1.0f / projectOn.LengthSquared();
            result = new Vector3(
                        dp * oneOnLenSqr * projectOn.X,
                        dp * oneOnLenSqr * projectOn.Y,
                        dp * oneOnLenSqr * projectOn.Y);
        }

        public static Vector2 GetDirectionFromAngle(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        public static float GetRotationAngle(Vector2 vec)
        {
            return (float)Math.Atan2(vec.Y, vec.X);
        }

        public static Vector2 AsVec2(Vector3 vec3)
        {
            return new Vector2(vec3.X, vec3.Y);
        }
        public static Vector2 AsVec2(ref Vector3 vec3)
        {
            return new Vector2(vec3.X, vec3.Y);
        }
    }
    
    public static class BoundingBoxEx
    {
        public static Vector3 Middle(BoundingBox bb)
        {
            return bb.Min + (bb.Max - bb.Min) * 0.5f;
        }
        public static Vector3 Middle(ref BoundingBox bb)
        {
            return bb.Min + (bb.Max - bb.Min) * 0.5f;
        }
    }
}
