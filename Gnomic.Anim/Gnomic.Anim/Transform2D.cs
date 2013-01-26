using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gnomic.Anim
{
    public struct Transform2D
    {
        public Vector2 Pos;
        public Vector2 Scale;
		public Vector2 Origin;
        public float Rot;

        // Not flipped. x = -1 for horizontal flipping, y = -1 for vertical
        private static Vector2 DEFAULT_FLIPPING = Vector2.One;

        private static Transform2D _identity;
        public static Transform2D Identity { get { return _identity; } }

        static Transform2D()
        {
            _identity = new Transform2D();
            _identity.Scale = Vector2.One;
        }

        public static void Compose(ref Transform2D key1, ref Transform2D key2, ref Vector2 flipping, out Transform2D result)
        {
            result.Pos = key1.TransformPoint(key2.Pos, flipping);
            result.Rot = key1.Rot + key2.Rot;
            result.Scale = key1.Scale * key2.Scale;
			result.Origin = key2.Origin;
        }

        public static void Lerp(ref Transform2D key1, ref Transform2D key2, float amount, ref Transform2D result)
        {
            result.Pos = Vector2.Lerp(key1.Pos, key2.Pos, amount);
            result.Scale = Vector2.Lerp(key1.Scale, key2.Scale, amount);
            result.Rot = Math.Abs(key2.Rot - key1.Rot) < MathHelper.Pi ? 
                        MathHelper.Lerp(key1.Rot, key2.Rot, amount) : MathHelper.Lerp(key2.Rot, key1.Rot, amount);
			result.Origin = Vector2.Lerp(key1.Origin, key2.Origin, amount);
        }

        public Vector2 TransformPoint(Vector2 point)
        {
            return TransformPoint(point, DEFAULT_FLIPPING);
        }
        public Vector2 TransformPoint(Vector2 point, Vector2 flipping)
        {
            // TODO: this can be optimised significantly!
            Matrix xform = Matrix.CreateTranslation(new Vector3(-Origin.X, -Origin.Y, 0.0f)) *
                           Matrix.CreateScale(new Vector3(Scale.X * flipping.X, Scale.Y * flipping.Y, 1.0f)) * 
                           Matrix.CreateRotationZ(Rot * flipping.X * flipping.Y) * 
                           Matrix.CreateTranslation(new Vector3(Pos.X, Pos.Y, 0.0f));

            Vector2 result;
            Vector2.Transform(ref point, ref xform, out result);
            return result;
        }
    }
}
