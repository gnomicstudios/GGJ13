using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spineless
{
    public class SpinelessGame : Gnomic.GnomicGame
    {
        public SpinelessGame()
		{
		}

		public override void Initialize(Microsoft.Xna.Framework.Graphics.IGraphicsDeviceService deviceSvc)
		{
            AddScreen("level", new LevelScreen());

			base.Initialize(deviceSvc);

            GetScreen("level").Activate();
		}
    }
}
