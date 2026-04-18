using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace MyFirstMod.Code.CardPools;

public class ExusiaiCardPool : CustomCardPoolModel
{
    public override string Title => "exusiai";
    public override string? TextEnergyIconPath => "res://myfirstmod/images/exusiai/energy_exusiai.png";
    public override string? BigEnergyIconPath => "res://myfirstmod/images/exusiai/energy_exusiai_big.png";
    public override Color DeckEntryCardColor => new(1f, 0.4f, 0.3f);
    public override Color ShaderColor => new(1f, 0.4f, 0.3f);
    public override bool IsColorless => false;
}
