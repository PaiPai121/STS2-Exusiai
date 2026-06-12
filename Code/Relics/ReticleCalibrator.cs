using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstMod.Code;
using MyFirstMod.Code.RelicPools;

namespace MyFirstMod.Code.Relics;

[Pool(typeof(ExusiaiRelicPool))]
public class ReticleCalibrator : MyFirstModRelicModel
{
    private const int AttackThreshold = 3;

    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override bool ShowCounter => true;
    public override int DisplayAmount => CurrentProgress();
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3, ValueProp.Move)];

    private int _attacksPlayed;

    [SavedProperty]
    public int AttacksPlayed
    {
        get => _attacksPlayed;
        set
        {
            AssertMutable();
            _attacksPlayed = value;
            UpdateDisplay();
        }
    }

    public override Task BeforeCombatStart()
    {
        AttacksPlayed = 0;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == null)
            return;

        if (cardPlay.Card.Owner != Owner)
            return;

        if (cardPlay.Card.Type != CardType.Attack)
            return;

        AttacksPlayed++;
        if (CurrentProgress() != 0)
            return;

        if (!CombatGuards.HasLivingEnemy(Owner.Creature?.CombatState))
            return;

        if (Owner.Creature == null)
            return;

        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        AttacksPlayed = 0;
        return Task.CompletedTask;
    }

    private int CurrentProgress()
    {
        return AttacksPlayed % AttackThreshold;
    }

    private void UpdateDisplay()
    {
        Status = CurrentProgress() == AttackThreshold - 1 ? RelicStatus.Active : RelicStatus.Normal;
        InvokeDisplayAmountChanged();
    }
}
