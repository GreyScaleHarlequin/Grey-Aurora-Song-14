using System.Linq;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Chat.TypingIndicator;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._AS.Traits;

public sealed partial class ReplicantSystem : EntitySystem
{
    [Dependency] private SharedBloodstreamSystem _bloodSystem = default!;
    [Dependency] private SharedTypingIndicatorSystem _typingIndicator = default!;
    [Dependency] private IPrototypeManager _protoMan = default!;

    private static readonly ProtoId<TypingIndicatorPrototype> TypingIndicator = "robot";
    private readonly Dictionary<EntProtoId, FixedPoint2> _speciesBloodLevels = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ReplicantComponent, ComponentStartup>(OnReplicantStartup);

        foreach (var speciesProto in _protoMan.EnumeratePrototypes<SpeciesPrototype>()
            .Select(proto => _protoMan.Index(proto.Prototype)))
        {
            if (!speciesProto.Components.TryGetComponent<BloodstreamComponent>(EntityManager.ComponentFactory,
                    out var bloodstreamComp))
                continue;

            var bloodVolume = bloodstreamComp.BloodReferenceSolution.Volume;

            _speciesBloodLevels.Add(speciesProto.ID, bloodVolume);
        }
    }

    private void OnReplicantStartup(EntityUid uid, ReplicantComponent component, ComponentStartup args)
    {
        _typingIndicator.SetTypingIndicator(uid, TypingIndicator);

        if (!HasComp<BloodstreamComponent>(uid))
            return;

        var metaData = MetaData(uid);

        var replicantBlood = new Solution(component.OxidantReagent);

        if (metaData.EntityPrototype?.ID is { } protoId && _speciesBloodLevels.TryGetValue(protoId, out var bloodAmount))
            replicantBlood.ScaleTo(bloodAmount);

        _bloodSystem.ChangeBloodReagents(uid, replicantBlood); // VDS - update to use new ChangeBloodReagents
    }
}
