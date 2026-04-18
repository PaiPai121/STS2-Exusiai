using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models.PotionPools;

namespace MyFirstMod.Code.PotionPools;

public class ExusiaiPotionPool : CustomPotionPoolModel
{
    public override string? TextEnergyIconPath => "res://myfirstmod/images/exusiai/energy_exusiai.png";
    public override string? BigEnergyIconPath => "res://myfirstmod/images/exusiai/energy_exusiai_big.png";
}
