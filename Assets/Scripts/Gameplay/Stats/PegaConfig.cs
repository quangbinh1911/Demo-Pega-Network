using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Stats
{
    [CreateAssetMenu(fileName = nameof(PegaConfig), menuName = "Configs/PegaConfig")]

    public class PegaConfig : ScriptableObject
    {
        public List<PegaStatConfig> configs;
    }

    [Serializable]
    public class PegaStatConfig
    {
        public PegaStat type;
        public float baseValue;
    }
}