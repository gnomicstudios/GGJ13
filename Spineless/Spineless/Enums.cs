using System;
using FarseerPhysics.Dynamics;

namespace Spineless
{
    [Flags]
    public enum SpinelessCollisionCategories
    {
        All = Category.All,
        Default = Category.Cat1,
        DirectHitProjectile = Category.Cat2,
        SplashProjectile = Category.Cat3,
        Terrain = Category.Cat4,
        Siege = Category.Cat5,
        Border = Category.Cat8,
        Princess = Category.Cat9,
        KnightLane1 = Category.Cat10,
        KnightLane2 = Category.Cat11,
        KnightLane3 = Category.Cat12,
        EnemyLane1 = Category.Cat20,
        EnemyLane2 = Category.Cat21,
        EnemyLane3 = Category.Cat22,
        Enemy = EnemyLane1 | EnemyLane2 | EnemyLane3,
        Knight = KnightLane1 | KnightLane2 | KnightLane3,
        Lane1 = KnightLane1 | EnemyLane1,
        Lane2 = KnightLane2 | EnemyLane2,
        Lane3 = KnightLane3 | EnemyLane3,
        AllProjectiles = DirectHitProjectile | SplashProjectile,
    }
}
