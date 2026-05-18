using Content.Shared.Trigger.Components.Effects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Moffstation.Revenant.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SpawnMultipleOnTriggerComponent : BaseXOnTriggerComponent
{
    /// <summary>
    /// All types of entity spawns with their settings
    /// </summary>
    [DataField, AutoNetworkedField]
    public List<EntitySpawnSettingsEntry> Entries = new();

    /// <summary>
    /// Power modifier for the spawn points.
    /// </summary>
    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public float PowerModifier { get; set; } = 1f;
}

[DataRecord, Serializable]
public partial record struct EntitySpawnSettingsEntry()
{
    /// <summary>
    /// A list of entities that are random picked to be spawned on each pulse
    /// </summary>
    public List<EntProtoId> Spawns { get; set; } = new();

    public MultipleSpawnSettings Settings { get; set; } = new();
}

[DataRecord, Serializable]
public partial record struct MultipleSpawnSettings()
{
    /// <summary>
    /// should entities block spawning?
    /// </summary>
    public bool CanSpawnOnEntities { get; set; } = false;

    /// <summary>
    /// The minimum number of entities that spawn per pulse
    /// </summary>
    public int MinAmount { get; set; } = 0;

    /// <summary>
    /// The maximum number of entities that spawn per pulse
    /// scales with severity.
    /// </summary>
    public int MaxAmount { get; set; } = 1;

    /// <summary>
    /// The distance from the anomaly in which the entities will not appear
    /// </summary>
    public float MinRange { get; set; } = 0f;

    /// <summary>
    /// The maximum radius the entities will spawn in.
    /// </summary>
    public float MaxRange { get; set; } = 1f;
}
