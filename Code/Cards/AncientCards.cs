using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstMod.Code.CardPools;

namespace MyFirstMod.Code.Cards;

[Pool(typeof(ExusiaiCardPool))]
public class SanctifiedCrossfire : RapidFireCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(16, ValueProp.Move),
        new PowerVar<VulnerablePower>(2),
        new PowerVar<WeakPower>(1)
    ];

    public override List<(string, string)> Localization => [
        ("title", "New Covenant Crossfire"),
        ("description", "Deal {Damage:diff()} damage. Apply {VulnerablePower:diff()} Vulnerable and {WeakPower:diff()} Weak. Add 1 Gunspark to your hand.")
    ];

    public SanctifiedCrossfire() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);

            await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars["VulnerablePower"].IntValue, Owner.Creature, this);
            await PowerCmd.Apply<WeakPower>(cardPlay.Target, DynamicVars["WeakPower"].IntValue, Owner.Creature, this);
        }

        await GeneratedTokenHelper.AddGunsparkToHand(this);
        await TryGenerateRapidFireCopy(choiceContext, cardPlay);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8);
        DynamicVars["VulnerablePower"].UpgradeValueBy(1);
    }
}
