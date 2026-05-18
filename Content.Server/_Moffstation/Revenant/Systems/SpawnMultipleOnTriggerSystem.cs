using System.Linq;
using System.Numerics;
using Content.Shared._Moffstation.Revenant.Components;
using Content.Shared._Moffstation.Revenant.Systems;
using Content.Shared.Physics;
using Content.Shared.Trigger;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Random;

namespace Content.Server._Moffstation.Revenant.Systems;

public sealed class SpawnMultipleOnTriggerSystem : SharedSpawnMultipleOnTriggerSystem
{
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    [Dependency] private readonly EntityQuery<PhysicsComponent> _physQuery = default!;


    protected override void OnTrigger(Entity<SpawnMultipleOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        SpawnEntities(ent);
    }

    private void SpawnEntities(Entity<SpawnMultipleOnTriggerComponent> component)
    {
        foreach (var entry in component.Comp.Entries)
        {
            SpawnEntities(component, entry, 1f);
        }
    }

    private void SpawnEntities(Entity<SpawnMultipleOnTriggerComponent> ent, EntitySpawnSettingsEntry entry, float powerMod)
    {
        var xform = Transform(ent);
        if (!TryComp<MapGridComponent>(xform.GridUid, out var grid))
            return;

        var tiles = GetSpawningPoints(ent, entry.Settings, powerMod);
        if (tiles == null)
            return;

        foreach (var tileref in tiles)
        {
            Spawn(_random.Pick(entry.Spawns), _map.ToCenterCoordinates(tileref, grid));
        }
    }

    /// <summary>
    /// Gets random points around the entity based on the given parameters.
    /// </summary>
    public List<TileRef>? GetSpawningPoints(EntityUid uid, MultipleSpawnSettings settings, float powerModifier = 1f)
    {
        var xform = Transform(uid);

        if (!TryComp<MapGridComponent>(xform.GridUid, out var grid))
            return null;

        // How many spawn points we will be aiming to return
        var amount = (int) (MathHelper.Lerp(settings.MinAmount, settings.MaxAmount, powerModifier) + 0.5f);

        // When the entity is in a container or buckled (such as a hosted anomaly), local coordinates will not be comparable
        // to tile coordinates.
        // Get the world coordinates for the anomalous entity
        var worldPos = _transform.GetWorldPosition(uid);

        // Get a list of the tiles within the maximum range of the effect
        var tilerefs = _map.GetTilesIntersecting(
                xform.GridUid.Value,
                grid,
                new Box2(worldPos + new Vector2(-settings.MaxRange), worldPos + new Vector2(settings.MaxRange)))
            .ToList();

        if (tilerefs.Count == 0)
            return null;

        var resultList = new List<TileRef>();
        while (resultList.Count < amount)
        {
            if (tilerefs.Count == 0)
                break;

            var tileref = _random.Pick(tilerefs);

            // Get the world position of the tile to calculate the distance to the anomalous object
            var tileWorldPos = _map.GridTileToWorldPos(xform.GridUid.Value, grid, tileref.GridIndices);
            var distance = Vector2.Distance(tileWorldPos, worldPos);

            //cut outer & inner circle
            if (distance > settings.MaxRange || distance < settings.MinRange)
            {
                tilerefs.Remove(tileref);
                continue;
            }

            if (!settings.CanSpawnOnEntities)
            {
                // If it can't spawn on entities, ensure that maximum one entity will be spawned here this pulse.
                tilerefs.Remove(tileref);

                var valid = true;
                foreach (var ent in _map.GetAnchoredEntities(xform.GridUid.Value, grid, tileref.GridIndices))
                {
                    if (!_physQuery.TryGetComponent(ent, out var body))
                        continue;

                    if (body.BodyType != BodyType.Static ||
                        !body.Hard ||
                        (body.CollisionLayer & (int) CollisionGroup.Impassable) == 0)
                        continue;

                    valid = false;
                    break;
                }
                if (!valid)
                {
                    continue;
                }
            }

            resultList.Add(tileref);
        }
        return resultList;
    }

}
