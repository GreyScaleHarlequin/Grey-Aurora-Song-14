using Content.Shared.Body.Systems;
using Content.Shared.Chat.TypingIndicator;
using Content.Shared.Chemistry.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared._AS.Traits;

public sealed partial class ReplicantSystem : EntitySystem
{
    private static readonly ProtoId<TypingIndicatorPrototype> TypingIndicator = "robot";
//    private static readonly ProtoId<ReagentPrototype> Blood = "Oxidant"; // VDS - use solution in component instead.

    [Dependency] private SharedBloodstreamSystem _bloodSystem = default!;
    [Dependency] private SharedTypingIndicatorSystem _typingIndicator = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ReplicantComponent, ComponentStartup>(OnReplicantStartup);
    }

    private void OnReplicantStartup(EntityUid uid, ReplicantComponent component, ComponentStartup args)
    {
        _typingIndicator.SetTypingIndicator(uid, TypingIndicator);
        var metaData = MetaData(uid); // Aurora's Song | Grab the MetaData of the player entity so we can see if it's a fairy

        var prototype = metaData.EntityPrototype;
        var protoId = prototype?.ID;
        if (protoId == "MobFairy") // Aurora's Song | For now we're only checking if they're a fairy,
            // in the future this may become reliant on a tag or component that is used to flag smaller lifeforms, but for now this is fine.
        {
            var replicantBlood = new Solution("Oxidant", 120);
            _bloodSystem.ChangeBloodReagents(uid, replicantBlood); // VDS - update to use new ChangeBloodReagents
        }
        else
        {
            // Aurora's Song - Moved the normal ChangeBloodReagents call into the selection logic
            _bloodSystem.ChangeBloodReagents(uid, component.OxidantReagent); // VDS - update to use new ChangeBloodReagents
        }

    }
}
