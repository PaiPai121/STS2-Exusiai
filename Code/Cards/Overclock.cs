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

    public override List<(string, string)> Localization => [("title", "Overclock"), ("description", "This turn, the next {Cards:diff()} Attacks you play cost 0. Every other turn, gain this effect again at the start of your turn.")];

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
