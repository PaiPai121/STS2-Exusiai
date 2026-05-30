using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MyFirstMod.Code.Powers;

public class SweepModePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/exusiai/character_icon_exusiai.png";
    public override string CustomBigIconPath => "res://myfirstmod/images/exusiai/character_icon_exusiai.png";
    public override List<(string, string)> Localization => [
        ("title", "扫射模式"),
        ("description", "每当你打出攻击牌，对所有敌人造成[red]{Amount}[/red]点伤害。"),
        ("smartDescription", "每当你打出攻击牌，对所有敌人造成[red]{Amount}[/red]点伤害。")
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == null)
            return;

        if (cardPlay.Card.Owner != Owner.Player)
            return;

        if (cardPlay.Card.Type != CardType.Attack)
            return;

        if (Owner.CombatState == null)
            return;

        await DamageCmd.Attack(Amount).FromCard(cardPlay.Card).TargetingAllOpponents(Owner.CombatState).Execute(choiceContext);
    }
}
