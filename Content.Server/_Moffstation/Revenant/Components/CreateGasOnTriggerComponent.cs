using Content.Shared.Atmos;
using Content.Shared.Trigger.Components.Effects;

namespace Content.Server._Moffstation.Revenant.Components;

[RegisterComponent]
public sealed partial class CreateGasOnTriggerComponent : BaseXOnTriggerComponent
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public Gas ReleasedGas = Gas.WaterVapor;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float MoleAmount = 50f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float SpawnRadius = 3f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public int TileCount = 1;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float TempChange = 0f;
}
