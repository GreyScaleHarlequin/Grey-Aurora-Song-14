using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Chat.TypingIndicator;
using Content.Shared.Chemistry.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared._AS.Traits;

public sealed partial class ReplicantSystem : EntitySystem
{
    [Dependency] private SharedBloodstreamSystem _bloodSystem = default!;
    [Dependency] private SharedTypingIndicatorSystem _typingIndicator = default!;

    private static readonly ProtoId<TypingIndicatorPrototype> TypingIndicator = "robot";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ReplicantComponent, ComponentStartup>(OnReplicantStartup);
    }

    private void OnReplicantStartup(EntityUid uid, ReplicantComponent component, ComponentStartup args)
    {
        _typingIndicator.SetTypingIndicator(uid, TypingIndicator);

        if (!TryComp<BloodstreamComponent>(uid, out var bloodstreamComponent))
            return;

        var replicantBlood = new Solution(component.OxidantReagent);

        replicantBlood.ScaleTo(bloodstreamComponent.BloodReferenceSolution.Volume); // Scale to current bloodstream

        _bloodSystem.ChangeBloodReagents(uid, replicantBlood); // VDS - update to use new ChangeBloodReagents
    }
}
