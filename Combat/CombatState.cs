using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecalpon.Combat
{
    public enum CombatState
    {
        Initializing,
        PlayerTurn,
        SelectingMove,
        SelectingTarget,
        UsingPower,
        EnemyTurn,
        Resolving,
        Victory,
        Defeat
    }
}
