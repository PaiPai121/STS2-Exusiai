using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MyFirstMod.Code.Keywords;

namespace MyFirstMod.Code.Powers;

public class RapidFireSupportPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/powers/FireControlPower.png";
    public override string CustomBigIconPath => "res://myfirstmod/images/powers/FireControlPower.png";
    public override List<(string, string)> Localization => [
        ("title", "Open Fire Discipline"),
        ("description", "Whenever you play a Rapid Fire card, deal [red]{Amount}[/red] damage to its target."),
        ("smartDescription", "Whenever you play a Rapid Fire card, deal [red]{Amount}[/red] damage to its target.")
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == null)
            return;

        if (cardPlay.Card.Owner != Owner.Player)
            return;

        if (!cardPlay.Card.Keywords.Contains(MyKeywords.RapidFire))
            return;

        if (!CombatGuards.HasLivingEnemy(Owner.CombatState))
            return;

        if (cardPlay.Target == null || !cardPlay.Target.IsAlive)
            return;

        await DamageCmd.Attack(Amount).FromCard(cardPlay.Card).Targeting(cardPlay.Target).Execute(choiceContext);
    }
}
