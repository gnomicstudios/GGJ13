using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spineless.Entities
{
    public class EnemyEntity : SpinelessEntity
    {
        public override void Initialize(GameScreen parentScreen)
        {
            base.Initialize(parentScreen);

            // var settings = new ClipEntitySettings();
            // settings.ClipFile = "player_player.clipxml";
            // settings.Position = new Vector2(ParentGame.ScreenWidth / 2,
            //                                 ParentGame.ScreenHeight / 2);
            // settings.DefaultAnimName = "run-right";

            physics = Settings.Physics.CreateStructure(
                parentScreen.Physics.World, this.Position);
            physics.SetVelocityLimit(
                parentScreen.Physics.World, Settings.MaxSpeed, 0.0f);
        }
    }
}
