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

			base.Initialize(deviceSvc);

            GetScreen("level").Activate();
		}
    }
}
