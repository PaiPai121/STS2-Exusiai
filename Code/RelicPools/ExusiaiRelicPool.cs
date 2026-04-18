using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace MyFirstMod.Code.RelicPools;

public class ExusiaiRelicPool : CustomRelicPoolModel
{
    public override string? TextEnergyIconPath => "res://myfirstmod/images/exusiai/energy_exusiai.png";
    public override string? BigEnergyIconPath => "res://myfirstmod/images/exusiai/energy_exusiai_big.png";
}
