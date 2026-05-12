using System.Numerics;
using Content.Server._Moffstation.Revenant.Curse.Components;
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.Mobs.Components;
using Content.Shared.Projectiles;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._Moffstation.Revenant.Curse.Systems;

//TODO: DOCUMENT STUFF

/// <summary>
/// This handles <see cref="ProjectileCurseComponent"/>.
/// </summary>
public sealed class ProjectileCurseSystem : EntitySystem
{
    [Dependency] private readonly TransformSystem _xform = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly GunSystem _gunSystem = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    private EntityQuery<TransformComponent> _xFormQuery;
    private EntityQuery<MobStateComponent> _mobQuery;

    /// <summary> Pre-allocated collection for calculating entities in range. </summary>
    private readonly HashSet<EntityUid> _inRange = new();

    public override void Initialize()
    {
        _xFormQuery = GetEntityQuery<TransformComponent>();
        _mobQuery = GetEntityQuery<MobStateComponent>();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<ProjectileCurseComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.NextShootTime > _gameTiming.CurTime)
                continue;

            ShootProjectilesAtEntities(uid, comp);

            // Set next shoot time
            comp.NextShootTime = _gameTiming.CurTime + TimeSpan.FromSeconds(_random.NextFloat(comp.ShootInterval.X, comp.ShootInterval.Y));
        }
    }

    private void ShootProjectilesAtEntities(EntityUid uid, ProjectileCurseComponent component)
    {
        if(!component.CanShoot)
            return;

        var projectileCount = _random.Next(component.MinProjectiles, component.MaxProjectiles + 1);

        var xform = _xFormQuery.GetComponent(uid);

        _inRange.Clear();
        _lookup.GetEntitiesInRange(uid, component.ProjectileRange, _inRange, LookupFlags.Dynamic);

        if (_inRange.Count == 0)
            return;

        var priority = new List<EntityUid>();
        foreach (var entity in _inRange)
        {
            if (_mobQuery.HasComponent(entity))
                priority.Add(entity);
        }

        while (projectileCount > 0)
        {
            var target = priority.Count > 0
                ? _random.PickAndTake(priority)
                : _random.Pick(_inRange);

            var targetXForm= _xFormQuery.GetComponent(target);
            var targetCoords = targetXForm.Coordinates.Offset(_random.NextVector2(0.5f));

            ShootProjectile(
                uid,
                component,
                xform.Coordinates,
                targetCoords
            );
            projectileCount--;
        }
    }

    private void ShootProjectile(
        EntityUid uid,
        ProjectileCurseComponent component,
        EntityCoordinates coords,
        EntityCoordinates targetCoords
    )
    {
        var mapPos = _xform.ToMapCoordinates(coords);

        var spawnCoords = _mapManager.TryFindGridAt(mapPos, out var gridUid, out _)
                ? _xform.WithEntityId(coords, gridUid)
                : new(_map.GetMapOrInvalid(mapPos.MapId), mapPos.Position);

        var ent = Spawn(component.ProjectilePrototype, spawnCoords);
        var direction = _xform.ToMapCoordinates(targetCoords).Position - mapPos.Position;

        if (!TryComp<ProjectileComponent>(ent, out var comp))
            return;

        _gunSystem.ShootProjectile(ent, direction, Vector2.Zero, uid, uid, component.ProjectileSpeed);
    }
}
