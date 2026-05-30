using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace MyFirstMod.Code;

public abstract class MyFirstModCardModel : CustomCardModel
{
    public override string PortraitPath => ResolvePortraitPath();

    protected MyFirstModCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    private string ResolvePortraitPath()
    {
        string preferred = $"res://myfirstmod/images/cards/{GetType().Name}.jpg";
        if (ResourceLoader.Exists(preferred))
            return preferred;

        const string fallback = "res://myfirstmod/images/cards/CardTemplate.jpg";
        if (ResourceLoader.Exists(fallback))
        {
            GD.Print($"[myfirstmod] portrait fallback card={GetType().Name} missing={preferred} using={fallback}");
            return fallback;
        }

        GD.PrintErr($"[myfirstmod] portrait missing card={GetType().Name} preferred={preferred} fallback={fallback}");
        return preferred;
    }
}
