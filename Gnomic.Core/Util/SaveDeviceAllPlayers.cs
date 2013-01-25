#if XBOX360
using System;
using System.Collections.Generic;
using System.Text;
using EasyStorage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
namespace Arands
{
    /// <summary>
    /// A SaveDevice used for non player-specific saving of data.
    /// </summary>
    public sealed class SaveDeviceAllPlayers : SaveDevice
    {
        /// <summary>
        /// Creates a new SaveDevice.
        /// </summary>
        public SaveDeviceAllPlayers() { }

        /// <summary>
        /// Derived classes should implement this method to call the Guide.BeginShowStorageDeviceSelector
        /// method with the desired parameters, using the given callback.
        /// </summary>
        /// <param name="callback">The callback to pass to Guide.BeginShowStorageDeviceSelector.</param>
        protected override void GetStorageDevice(AsyncCallback callback)
        {
            StorageDevice.BeginShowSelector(callback, null);
        }

        protected override void PrepareEventArgs(SaveDeviceEventArgs args)
        {
            base.PrepareEventArgs(args);
            args.PlayerToPrompt = Arands.Core.Input.LastPlayerIndex;
        }
    }
}
#endif
