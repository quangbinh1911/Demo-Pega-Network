using System.Collections.Generic;

namespace Gameplay.Stats
{
    public class StatModification
    {
        public float BaseValue;
        public readonly List<Modifier> Modifiers = new();
    }
}