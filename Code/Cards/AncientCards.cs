using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstMod.Code.CardPools;

namespace MyFirstMod.Code.Cards;

[Pool(typeof(ExusiaiCardPool))]
public class SanctifiedCrossfire : RapidFireCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10, ValueProp.Move)];

    public override List<(string, string)> Localization => [
        ("title", "New Covenant Crossfire"),
        ("description", "Deal {Damage:diff()} damage. Add 1 Gunspark to your hand.")
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
        }

        await GeneratedTokenHelper.AddGunsparkToHand(this);
        await TryGenerateRapidFireCopy(choiceContext, cardPlay);
    }

    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4);
}
