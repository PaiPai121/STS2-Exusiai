using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace MyFirstMod.Code;

public abstract class MyFirstModCardModel : CustomCardModel
{
    private sealed class UpgradeHighlightedVar : DynamicVar
    {
        public UpgradeHighlightedVar(string name, decimal value)
            : base(name, value)
        {
            WasJustUpgraded = true;
        }
    }

    public override string PortraitPath => ResolvePortraitPath();
    public override IEnumerable<string> ExtraRunAssetPaths => [PortraitPath];

    protected MyFirstModCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);

        if (!IsUpgraded || CombatState != null)
            return;

        CardModel baseCard;
        try
        {
            baseCard = ModelDb.GetById<CardModel>(Id);
        }
        catch
        {
            return;
        }

        foreach (var pair in DynamicVars)
        {
            if (!baseCard.DynamicVars.TryGetValue(pair.Key, out DynamicVar? baseVar) || baseVar == null)
                continue;

            DynamicVar currentVar = pair.Value;
            if (currentVar.BaseValue == baseVar.BaseValue)
                continue;

            description.AddObj(pair.Key, new UpgradeHighlightedVar(pair.Key, currentVar.BaseValue));
        }
    }

    private string ResolvePortraitPath()
    {
        string preferred = $"res://myfirstmod/images/cards/{GetType().Name}.jpg";
        if (ResourceLoader.Exists(preferred))
            return preferred;

        const string fallback = "res://myfirstmod/images/cards/CardTemplate.jpg";
        if (ResourceLoader.Exists(fallback))
        {
            GD.Print($"[exusiai] portrait fallback card={GetType().Name} missing={preferred} using={fallback}");
            return fallback;
        }

        GD.PrintErr($"[exusiai] portrait missing card={GetType().Name} preferred={preferred} fallback={fallback}");
        return preferred;
    }
}
