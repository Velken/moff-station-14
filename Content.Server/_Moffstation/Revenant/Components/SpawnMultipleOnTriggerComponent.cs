using Content.Server._Moffstation.Revenant.Systems;
using Content.Shared.Trigger.Components.Effects;
using Robust.Shared.Prototypes;

namespace Content.Server._Moffstation.Revenant.Components;

public sealed partial class SpawnMultipleOnTriggerComponent : BaseXOnTriggerComponent
{
    /// <summary>
    /// All types of entity spawns with their settings
    /// </summary>
    [DataField]
    public List<EntitySpawnSettingsEntry> Entries = new();

    /// <summary>
    /// Power modifier for the spawn points.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float PowerModifier { get; set; } = 1f;
}

[DataRecord]
public partial record struct EntitySpawnSettingsEntry()
{
    /// <summary>
    /// A list of entities that are random picked to be spawned on each pulse
    /// </summary>
    public List<EntProtoId> Spawns { get; set; } = new();

    public MultipleSpawnSettings Settings { get; set; } = new();
}
