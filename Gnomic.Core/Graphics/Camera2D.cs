using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gnomic
{
    public class Camera2D
    {
        public Camera2D(Viewport viewport)
        {
            Origin = new Vector2(viewport.Width / 2.0f, viewport.Height / 2.0f);
            Zoom = 1.0f;
        }

        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public float Zoom { get; set; }
        public float Rotation { get; set; }

        public Matrix GetViewMatrix(Vector2 parallax)
        {
            return Matrix.CreateTranslation(new Vector3(-Position * parallax, 0.0f)) *
                   Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(Zoom, Zoom, 1.0f) *
                   Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        public Matrix GetViewMatrixInvertY(Vector2 parallax)
        {
            return Matrix.CreateTranslation(new Vector3(new Vector2(-Position.X, Position.Y) * parallax, 0.0f)) *
                   Matrix.CreateTranslation(new Vector3(new Vector2(-Origin.X, Origin.Y), 0.0f)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(Zoom, Zoom, 1.0f) *
                   Matrix.CreateTranslation(new Vector3(Origin.X, -Origin.Y, 0.0f));
        }
    }
}
