using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace MyFirstMod.Code;

internal static class CombatGuards
{
    public static bool HasLivingEnemy(CombatState? combatState)
    {
        if (combatState == null)
            return false;

        foreach (Creature enemy in combatState.Enemies)
        {
            if (enemy.IsAlive)
                return true;
        }

        return false;
    }
}
