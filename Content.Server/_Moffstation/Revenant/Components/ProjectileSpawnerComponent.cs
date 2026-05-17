using System.Numerics;
using Content.Shared.Trigger.Components.Triggers;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._Moffstation.Revenant.Components;

[RegisterComponent]
public sealed partial class ProjectileSpawnerComponent : BaseTriggerOnXComponent
{
    /// <summary>
    /// If the entity can shoot projectiles.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public bool CanShoot = true;

    /// <summary>
    /// If a trigger signal is sent after a successful shooting.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public bool SendTriggerSignal = true;

    /// <summary>
    /// The sound to play when the entity shoots projectile(s)
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier? SoundOnShoot;

    /// <summary>
    /// The prototype of the projectile that will spawn from the cursed object.
    /// </summary>
    [DataField(required: true, customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>)), ViewVariables(VVAccess.ReadWrite)]
    public string ProjectilePrototype = default!;

    /// <summary>
    /// The interval between shots.
    /// The first float corresponds to the minimum amount of time.
    /// The second corresponds to the maximum amount of time.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public Vector2 ShootInterval = new(5, 30);

    /// <summary>
    /// The next time the entity will shoot projectiles.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan NextShootTime = TimeSpan.Zero;

    /// <summary>
    /// The speed <see cref="ProjectilePrototype"/> can travel
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float ProjectileSpeed = 30f;

    /// <summary>
    /// The minimum number of projectiles shot per firing.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public int MinProjectiles = 1;

    /// <summary>
    /// The MAXIMUM number of projectiles shot per firing.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public int MaxProjectiles = 3;

    /// <summary>
    /// The MAXIMUM range for targeting entities.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float ProjectileRange = 50f;
}
