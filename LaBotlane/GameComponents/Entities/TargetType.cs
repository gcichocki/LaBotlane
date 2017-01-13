using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Labotlane.GameComponents.Entities
{
    /// <summary>
    /// Représente les types de cibles.
    /// </summary>
    public enum TargetType
    {
        // Modificateurs : 0x0001 -> 0x0100
        Invincible  = 0x0001,
        Neutral     = 0x0002,
        Targetable  = 0x0004,
        Dynamic     = 0x0008,
        Crossable   = 0x0010,

        // Entités Dynamiques
        Soldier     = 0x1000 | Targetable | Dynamic,
        Factory     = 0x1100 | Targetable | Dynamic,
        Bonus       = 0x1200 | Dynamic    | Crossable,

        // Terrain
        Ground      = 0x4000 | Neutral | Invincible | Crossable,
        Slow        = 0x4200 | Neutral | Invincible | Crossable,
        Mountain    = 0x4400 | Neutral | Invincible,
        Water       = 0x4800 | Neutral | Invincible
    }
}
