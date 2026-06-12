using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MyFirstMod.Code.CardPools;
using MyFirstMod.Code.Powers;

namespace MyFirstMod.Code.Cards;

[Pool(typeof(ExusiaiCardPool))]
public class Overclock : MyFirstModCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;


    public override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2)
    ];

    public override List<(string, string)> Localization => [("title", "Overclock"), ("description", "This turn, your next {Cards:diff()} Attacks cost 0. Every other turn, refresh this. Additional Overclocks increase future refreshes.")];

    public Overclock() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<OverclockPower>(
            Owner.Creature,
            DynamicVars.Cards.IntValue,
            Owner.Creature,
            this);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
