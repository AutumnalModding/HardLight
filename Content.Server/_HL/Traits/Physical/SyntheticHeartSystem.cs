using Content.Shared.Body.Systems;
using Content.Shared._Shitmed.Body.Organ;
using Content.Shared.Body.Components;
using Content.Shared.Body.Organ;
using Robust.Shared.Containers;

namespace Content.Server._HL.Traits.Physical;

public sealed class SyntheticHeartSystem : EntitySystem
{
    [Dependency] private readonly SharedBodySystem _body = null!;
    [Dependency] private readonly SharedContainerSystem _containers = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SyntheticHeartComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, SyntheticHeartComponent comp, ComponentStartup args)
    {
        if (!TryComp<BodyComponent>(uid, out var body))
            return;

        if (_body.TryGetBodyOrganEntityComps<HeartComponent>((uid, body), out var livers))
        {
            var old = livers[0].Owner;
            if (_containers.TryGetContainingContainer((old, null, null), out var cont))
            {
                var part = cont.Owner;
                _body.RemoveOrgan(old);
                QueueDel(old);

                var spawn = Spawn("OrganSynthHeart", Transform(part).Coordinates);
                if (TryComp(spawn, out OrganComponent? organ))
                {
                    _body.InsertOrgan(part, spawn, "heart", null, organ);
                }
                else
                {
                    QueueDel(spawn);
                }
            }
        }
    }
}
