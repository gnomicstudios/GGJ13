#if WINDOWS
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System.Xml;

namespace Gnomic.Core
{
    public partial class ContentTracker
    {
        private bool TrySearchForAssetSource<T>(string assetName, out string fileName)
        {
            string[] validExtensions = null;
            //if (typeof(T) == typeof(Model))
            //    validExtensions = new string[] { ".x" };
            //else 
            if (typeof(T) == typeof(Texture2D))
                validExtensions = new string[] { ".jpg", ".png", ".bmp", ".tga", ".dds" };
            //else if (typeof(T) == typeof(Effect))
            //    validExtensions = new string[] { ".fx" };
            else
                validExtensions = new string[] { ".xml", ".clipxml" };

            string searchFolder;
            if (Path.IsPathRooted(assetName))
                searchFolder = Path.GetDirectoryName(assetName);
            else
                searchFolder = Path.Combine(base.RootDirectory, Path.GetDirectoryName(assetName));

            string[] files = Directory.GetFiles(searchFolder, Path.GetFileName(assetName) + ".*", SearchOption.TopDirectoryOnly);
            foreach (string f in files)
            {
                foreach (string ext in validExtensions)
                {
                    if (f.ToLower().EndsWith(ext))
                    {
                        fileName = f;
                        return true;
                    }
                }
            }
            fileName = "";
            return false;
        }

        private T ReadAssetSourceFile<T>(string fileName, Action<IDisposable> recordDisposableObject)
        {
            GraphicsDevice device = ((IGraphicsDeviceService)ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

            //if (typeof(T) == typeof(Model))
            //{
            //    //
            //    // 
            //    //return (T)(object)ModelInjector.X2Model.ModelFromFile(device, fileName);
            //}
            //else 
            //else if (typeof(T) == typeof(Effect))
            //{
                //return (T)(object)ModelInjector.EffectCompiler.FromFile(device, fileName);
            //}
            if (typeof(T) == typeof(Texture2D))
            {
                object texture = null;
                using (Stream fs = File.OpenRead(fileName))
                {
                    texture = Texture2D.FromStream(device, fs);
                }
                return (T)texture;
            }
            else
            {
                using (XmlReader inXml = XmlReader.Create(fileName))
                {
                    return IntermediateSerializer.Deserialize<T>(inXml, fileName);
                }
            }
            return default(T);
        }
    }
}
#endif