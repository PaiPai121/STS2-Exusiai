using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace MyFirstMod.Code;

public abstract class MyFirstModRelicModel : CustomRelicModel
{
    // 统一管理遗物图标路径
    public override string PackedIconPath => $"res://myfirstmod/images/relics/{GetType().Name}.png";
    protected override string PackedIconOutlinePath => $"res://myfirstmod/images/relics/{GetType().Name}.png";
    protected override string BigIconPath => $"res://myfirstmod/images/relics/{GetType().Name}.png";
}
