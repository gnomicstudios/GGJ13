using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gnomic.Util
{
    static class ContentManagerExtensions
    {
        static GraphicsDevice s_graphicsDevice = null;

        static GraphicsDevice GetGraphicsDevice(this ContentManager content)
        {
            if (s_graphicsDevice == null)
            {
                s_graphicsDevice = ((IGraphicsDeviceService)content.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
#if NETFX_CORE
                // TODO MonoGame's .Net Core's GraphicsDevice does not have the Disposing event yet
                //      Check whether anything relies on whatever happens in disposing event.
#else
                s_graphicsDevice.Disposing += new EventHandler<EventArgs>(s_graphicsDevice_Disposing);
#endif
            }
            return s_graphicsDevice;
        }

        static void s_graphicsDevice_Disposing(object sender, EventArgs e)
        {
            // Clear device reference when it is disposed
            s_graphicsDevice = null;
        }

#if NETFX_CORE
        // TODO: need to use Windows.Storage.FileIO
        
#else
        public static Texture2D LoadTextureStream(this ContentManager content, string assetName)
        {
            Texture2D result;

            using (Stream titleStream = File.OpenRead(System.IO.Path.Combine(content.RootDirectory, assetName + ".png")))
            {
                result = Texture2D.FromStream(content.GetGraphicsDevice(), titleStream);
            }
            return result;
        }
#endif
    }
}
