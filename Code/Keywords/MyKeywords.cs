using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace MyFirstMod.Code.Keywords;

public class MyKeywords
{
    [CustomEnum("RAPID_FIRE")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword RapidFire;
}
