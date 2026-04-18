using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MyFirstMod.Code.RelicPools;

namespace MyFirstMod.Code.Relics;

[Pool(typeof(ExusiaiRelicPool))]
public class SniperChipset : MyFirstModRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    private bool _usedThisCombat;

    public override Task BeforeCombatStart()
    {
        _usedThisCombat = false;
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (_usedThisCombat)
            return playCount;
        if (card.Owner != Owner)
            return playCount;
        if (card.Type != CardType.Attack)
            return playCount;
        return playCount + 1;
    }

    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        _usedThisCombat = true;
        Flash();
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        _usedThisCombat = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}
