using Gnomic.Anim;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using Gnomic.Graphics;

namespace Gnomic.Entities
{
    public class ClipEntity3DSettings : ClipEntitySettings
    {
        [ContentSerializer(Optional = true)]
        public float PositionZ = 0.0f;

        public ClipEntity3DSettings()
        {
            base.EntityClass = "Gnomic.Entities.ClipEntity3D";
        }
    }

    public class ClipEntity3D : Entity, IDrawable3D
    {
        // Shared temporary verts for transforming 2D quads into two triangles on the Z = positionZ plane.
        static VertexPositionColorTexture[] s_quadVerts = new VertexPositionColorTexture[6];
        static float PIXELS_PER_METER = 32.0f;

        public new ClipEntity3DSettings Settings;
        public override void ApplySettings(EntitySettings settings)
        {
            base.ApplySettings(settings);
            this.Settings = CastSettings<ClipEntity3DSettings>(settings);
        }

        public ClipInstance ClipInstance;
        public Body DynamicBody;

        public float PositionZ
        {
            get { return Settings.PositionZ; }
        }

        public Vector2 Position
        {
            get { return ClipInstance.Position; }
            set { ClipInstance.Position = value; }
        }

        public Vector2 Origin
        {
            get { return ClipInstance.Origin; }
            set { ClipInstance.Origin = value; }
        }

        public Vector2 Scale
        {
            get { return ClipInstance.Scale * PIXELS_PER_METER; }
            set { ClipInstance.Scale = value / PIXELS_PER_METER; }
        }

        public float Rotation
        {
            get { return MathHelper.WrapAngle(ClipInstance.Rotation + MathHelper.Pi); }
            set { ClipInstance.Rotation = MathHelper.WrapAngle(value - MathHelper.Pi); }
        }

        public bool FlipX
        {
            // Note that FlipX is negated for the 3D clip entity
            get { return !ClipInstance.FlipX; }
            set { ClipInstance.FlipX = !value; }
        }
        
        public bool FlipY
        {
            get { return ClipInstance.FlipY; }
            set { ClipInstance.FlipY = value; }
        }

        bool isVisible = true;
        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        public float DrawOrder 
        { 
            get
            {
                return Settings.PositionZ;
            } 
        }

        public override void Initialize(GameScreen parentScreen)
        {
            base.Initialize(parentScreen);

            // Load the clip. Note that this instance is shared with all
            // entities that want this clip file.
            Clip clip = parentScreen.Content.Load<Clip>(Settings.ClipFile);
            clip.Init(parentScreen.Content);

            // Create the ClipInstance. This allows different entities to
            // share the same clip and play their own animations on it
            ClipInstance = new ClipInstance(clip);
            //ClipInstance.Scale = new Vector2(-1.0f, 1.0f);

            // Set the initial position of the clip instance
            Position = Settings.Position;
            Origin = Settings.Origin;
            Rotation = MathHelper.ToRadians(Settings.Rotation);
            Scale = Settings.Scale;
            FlipX = Settings.FlipX;
            FlipY = Settings.FlipY;

            if (string.IsNullOrEmpty(Settings.DefaultAnimName))
            {
                ClipInstance.Play(clip.AnimSet.Anims[0], Settings.DefaultAnimLooping);
            }
            else
            {
                ClipInstance.Play(Settings.DefaultAnimName, Settings.DefaultAnimLooping);
            }
            ClipInstance.Update(Settings.DefaultAnimOffset);
        }

        public override void Update(float dt)
        {
            ClipInstance.Update(dt);
        }
        
        public void Draw3D()
        {
            int drawItems = ClipInstance.Clip.DrawOrder.Length;
            for (int i = 0; i < drawItems; ++i)
            {
                int index = ClipInstance.Clip.DrawOrder[i];
                SpriteState js = ClipInstance.JointStates[index];
                if (js.Texture != null && js.Visible)
                {
                    float tw = js.Texture.Width;
                    float th = js.Texture.Height;
                    float w = js.TextureRect.Width;
                    float h = js.TextureRect.Height;

                    // Apply the current transform to the quad corners
                    Transform2D xform = ClipInstance.AbsoluteTransforms[index];
                    Vector2 flipping = new Vector2(
                        ClipInstance.FlipX ? -1.0f : 1.0f,
                        ClipInstance.FlipY ? -1.0f : 1.0f);

                    Vector2 tl = xform.TransformPoint(new Vector2(0, 0), flipping);
                    Vector2 tr = xform.TransformPoint(new Vector2(w, 0), flipping);
                    Vector2 bl = xform.TransformPoint(new Vector2(0, h), flipping);
                    Vector2 br = xform.TransformPoint(new Vector2(w, h), flipping);

                    // Convert pixel rectangle to texture coordinates
                    Vector2 uvMin;
                    uvMin.X = js.TextureRect.X / tw;
                    uvMin.Y = js.TextureRect.Y / th;
                    Vector2 uvMax;
                    uvMax.X = (js.TextureRect.X + w) / tw;
                    uvMax.Y = (js.TextureRect.Y + h) / th;

                    // Swap X components if flipped horizontally
                    if (0 != (js.FlipState & SpriteEffects.FlipHorizontally))
                    {
                        float tmp = uvMin.X;
                        uvMin.X = uvMax.X;
                        uvMax.X = tmp;
                    }
                    
                    // Swap Y components if flipped horizontally
                    if (0 != (js.FlipState & SpriteEffects.FlipVertically))
                    {
                        float tmp = uvMin.Y;
                        uvMin.Y = uvMax.Y;
                        uvMax.Y = tmp;
                    }

                    // Assign the 6 vertex values
                    // Verts laid out to render as a list of two clockwise triangles:
                    // 0--1 4
                    // | / /|
                    // |/ / |
                    // 2 3--5
                    float z = Settings.PositionZ;
                    s_quadVerts[0].Position.X = tl.X;
                    s_quadVerts[0].Position.Y = tl.Y;
                    s_quadVerts[0].Position.Z = z;
                    s_quadVerts[0].Color = js.Color;
                    s_quadVerts[0].TextureCoordinate.X = uvMin.X;
                    s_quadVerts[0].TextureCoordinate.Y = uvMin.Y;
                    s_quadVerts[1].Position.X = tr.X;
                    s_quadVerts[1].Position.Y = tr.Y;
                    s_quadVerts[1].Position.Z = z;
                    s_quadVerts[1].Color = js.Color;
                    s_quadVerts[1].TextureCoordinate.X = uvMax.X;
                    s_quadVerts[1].TextureCoordinate.Y = uvMin.Y;
                    s_quadVerts[2].Position.X = bl.X;
                    s_quadVerts[2].Position.Y = bl.Y;
                    s_quadVerts[2].Position.Z = z;
                    s_quadVerts[2].Color = js.Color;
                    s_quadVerts[2].TextureCoordinate.X = uvMin.X;
                    s_quadVerts[2].TextureCoordinate.Y = uvMax.Y;
                    s_quadVerts[3].Position.X = bl.X;
                    s_quadVerts[3].Position.Y = bl.Y;
                    s_quadVerts[3].Position.Z = z;
                    s_quadVerts[3].Color = js.Color;
                    s_quadVerts[3].TextureCoordinate.X = uvMin.X;
                    s_quadVerts[3].TextureCoordinate.Y = uvMax.Y;
                    s_quadVerts[4].Position.X = tr.X;
                    s_quadVerts[4].Position.Y = tr.Y;
                    s_quadVerts[4].Position.Z = z;
                    s_quadVerts[4].Color = js.Color;
                    s_quadVerts[4].TextureCoordinate.X = uvMax.X;
                    s_quadVerts[4].TextureCoordinate.Y = uvMin.Y;
                    s_quadVerts[5].Position.X = br.X;
                    s_quadVerts[5].Position.Y = br.Y;
                    s_quadVerts[5].Position.Z = z;
                    s_quadVerts[5].Color = js.Color;
                    s_quadVerts[5].TextureCoordinate.X = uvMax.X;
                    s_quadVerts[5].TextureCoordinate.Y = uvMax.Y;

                    // Send quad to screen's RenderBatch
                    parentScreen.RenderBatch.AddGeometry(s_quadVerts, js.Texture);
                }
            }
        }

        protected int drawGroup = 0;
        public int DrawGroup
        {
            get { return drawGroup; }
        }
    }
}
