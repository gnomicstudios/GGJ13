using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gnomic.Anim;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using Gnomic.Graphics;

namespace Gnomic.Entities
{
    public class ClipEntitySettings : EntitySettings
    {
        public string ClipFile;

        [ContentSerializer(Optional = true)]
        public Vector2 Position;

        [ContentSerializer(Optional = true)]
        public Vector2 Origin;

        [ContentSerializer(Optional = true)]
        public Vector2 Scale = Vector2.One;
        
        [ContentSerializer(Optional=true)]
        public float Rotation;

        [ContentSerializer(Optional = true)]
        public bool FlipX;

        [ContentSerializer(Optional = true)]
        public bool FlipY;

        [ContentSerializer(Optional=true)]
        public string DefaultAnimName = null;

        [ContentSerializer(Optional = true)]
        public bool DefaultAnimLooping = true;

        [ContentSerializer(Optional = true)]
        public float DefaultAnimOffset = 0.0f;

        public ClipEntitySettings()
        {
            base.EntityClass = "Gnomic.Entities.ClipEntity";
        }
    }

    public class ClipEntity : Entity, IDrawable2D
    {
        public new ClipEntitySettings Settings;
        public override void ApplySettings(EntitySettings settings)
        {
            base.ApplySettings(settings);
            this.Settings = CastSettings<ClipEntitySettings>(settings);
        }

        public ClipInstance ClipInstance;
        public Body DynamicBody;

        public virtual Vector2 Position
        {
            get { return ClipInstance.Position; }
            set { ClipInstance.Position = value; }
        }

        public Vector2 Origin
        {
            get { return ClipInstance.Position; }
            set { ClipInstance.Position = value; }
        }

        public Vector2 Scale
        {
            get { return ClipInstance.Scale; }
            set { ClipInstance.Scale = value; }
        }

        public float Rotation
        {
            get { return ClipInstance.Rotation; }
            set { ClipInstance.Rotation = value; }
        }

        public bool FlipX
        {
            get { return ClipInstance.FlipX; }
            set { ClipInstance.FlipX = value; }
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
            set { ClipInstance.FlipY = !value; }
        }

        public float DrawOrder 
        { 
            get
            {
                if (ClipInstance != null)
                {
                    return ClipInstance.Position.Y;
                }
                return 0.0f; 
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

            // Set the initial position of the clip instance
            ClipInstance.Position = Settings.Position;
            ClipInstance.Origin = Settings.Origin;
            ClipInstance.Rotation = MathHelper.ToRadians(Settings.Rotation);
            ClipInstance.Scale = Settings.Scale;
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
            base.Update(dt);
        }

        public virtual void Draw2D(SpriteBatch spriteBatch)
        {
            ClipInstance.Draw(spriteBatch);
        }

        protected int layerID = 0;
        public virtual int LayerID { get { return layerID; } }
    }
}
