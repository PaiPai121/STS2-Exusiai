using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace MyFirstMod.Code;

public abstract class MyFirstModCardModel : CustomCardModel
{
    public override string PortraitPath => $"res://myfirstmod/images/cards/{GetType().Name}.png";

    protected MyFirstModCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
}
