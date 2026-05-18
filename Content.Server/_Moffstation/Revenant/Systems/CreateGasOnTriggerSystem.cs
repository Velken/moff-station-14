using System.Linq;
using System.Numerics;
using Content.Server.Atmos.EntitySystems;
using Content.Server._Moffstation.Revenant.Components;
using Content.Shared.Trigger;
using Content.Shared.Trigger.Systems;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;

namespace Content.Server._Moffstation.Revenant.Systems;

public sealed class CreateGasOnTriggerSystem : XOnTriggerSystem<CreateGasOnTriggerComponent>
{
    [Dependency] private readonly AtmosphereSystem _atmosphere = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;

    protected override void OnTrigger(Entity<CreateGasOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        var component = ent.Comp;
        var xform = Transform(ent);

        if (!TryComp<MapGridComponent>(xform.GridUid, out var grid))
            return;

        var localpos = xform.Coordinates.Position;
        var radius = component.SpawnRadius;
        var tilerefs = _map.GetLocalTilesIntersecting(
            xform.GridUid.Value,
            grid,
            new Box2(localpos + new Vector2(-radius, -radius), localpos + new Vector2(radius, radius)))
            .ToArray();

        if (tilerefs.Length == 0)
            return;

        // Inject gas to the current tile
        var mixture = _atmosphere.GetTileMixture((ent.Owner, xform), true);
        if (mixture != null)
        {
            mixture.AdjustMoles(component.ReleasedGas, component.MoleAmount);
            mixture.Temperature += component.TempChange;
        }

        if (component.TileCount <= 0)
            return;

        _random.Shuffle(tilerefs);
        var amountCounter = 0;
        foreach (var tileref in tilerefs)
        {
            var mix = _atmosphere.GetTileMixture(xform.GridUid, xform.MapUid, tileref.GridIndices, true);
            if (mix == null)
                continue;

            mix.AdjustMoles(component.ReleasedGas, component.MoleAmount);
            mix.Temperature += component.TempChange;

            amountCounter++;
            if (amountCounter >= component.TileCount)
                break;
        }
    }
}
