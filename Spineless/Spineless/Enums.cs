using System;
using FarseerPhysics.Dynamics;

namespace Spineless
{
    [Flags]
    enum SpinelessCollisionCategories
    {
        All = Category.All,
        Princess = Category.Cat1,
        DirectHitProjectile = Category.Cat2,
        SplashProjectile = Category.Cat3,
        Terrain = Category.Cat4,
        Siege = Category.Cat5,
        Knight = Category.Cat6,
        Enemy = Category.Cat7,
        Border = Category.Cat8,
        AllProjectiles = DirectHitProjectile | SplashProjectile,
    }
}
