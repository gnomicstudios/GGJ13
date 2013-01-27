using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gnomic.Physics;

namespace Spineless
{
    public class SpinelessGame : Gnomic.GnomicGame
    {
        public SpinelessGame()
		{
            // Approx. 64 pixels for every physical unit (meter)
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64.0f);
		}

		public override void Initialize(Microsoft.Xna.Framework.Graphics.IGraphicsDeviceService deviceSvc)
        {
            AddScreen("level", new LevelScreen());
            AddScreen("hud", new HudScreen());

            base.Initialize(deviceSvc);

            base.Audio.PreloadSoundEffect("heartbeat1", "Audio/heartbeat", true, 0.4f, 0.0f, 1);
            base.Audio.PreloadSoundEffect("heartbeat2", "Audio/heartbeat", true, 0.6f, 0.3f, 1);
            base.Audio.PreloadSoundEffect("heartbeat3", "Audio/heartbeat", true, 1.0f, 0.8f, 1);
            base.Audio.PreloadSoundEffect("arrow03", "Audio/arrow03", false);
            base.Audio.PreloadSoundEffect("enemydeath01", "Audio/enemydeath01", false);
            base.Audio.PreloadSoundEffect("enemydeath02", "Audio/enemydeath02", false);
            base.Audio.PreloadSoundEffect("enemyhit01", "Audio/enemyhit01", false);
            base.Audio.PreloadSoundEffect("enemyhit02", "Audio/enemyhit02", false);
            base.Audio.PreloadSoundEffect("enemyhit03", "Audio/enemyhit03", false);
            base.Audio.PreloadSoundEffect("rockImpact01", "Audio/rockImpact01", false);
            base.Audio.PreloadSoundEffect("rockImpact02", "Audio/rockImpact02", false);
            base.Audio.PreloadSoundEffect("seige", "Audio/seige", true);

            GetScreen("level").Activate();
            GetScreen("hud").Activate();
		}
    }
}
