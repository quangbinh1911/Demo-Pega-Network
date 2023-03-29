using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Gameplay.Stats;

namespace Gameplay.Pega
{
    public partial class PegaPredicted : NetworkBehaviour
    {
        [SyncObject] private readonly SyncDictionary<PegaStat, float> _values = new();

        public float ValueOf(PegaStat type)
        {
            return _values.ContainsKey(type) ? _values[type] : 1f;
        }


        private readonly Dictionary<PegaStat, StatModification> _modifications = new();

        private StatModification ModificationOf(PegaStat type)
        {
            if (!_modifications.ContainsKey(type)) _modifications.Add(type, new StatModification());
            return _modifications[type];
        }


        [Server]
        public void LoadStats(PegaConfig config)
        {
            foreach (var statConfig in config.configs)
            {
                var modification = ModificationOf(statConfig.type);
                modification.BaseValue = statConfig.baseValue;

                ReCalculate(statConfig.type);
            }
        }


        [ServerRpc(RequireOwnership = false)]
        public void AddModifier(PegaStat type, Modifier modifier)
        {
            var modifiers = ModificationOf(type).Modifiers;
            if (modifiers.Any(mod => mod.guid == modifier.guid)) return; //already contains

            modifiers.Add(modifier);
            ReCalculate(type);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RemoveModifier(PegaStat type, Modifier modifier)
        {
            var modifiers = ModificationOf(type).Modifiers;

            var modIndex = modifiers.FindIndex(mod => mod.guid == modifier.guid);
            if (modIndex < 0) return; //not contains

            modifiers.RemoveAt(modIndex);
            ReCalculate(type);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ReCalculate(PegaStat type)
        {
            int CompareModifierOrder(Modifier a, Modifier b)
            {
                if (a.order < b.order) return -1;
                return a.order > b.order ? 1 : 0;
            }


            var modification = ModificationOf(type);
            var baseVale = modification.BaseValue;
            var modifiers = modification.Modifiers;

            var finalValue = baseVale;
            float sumPercentAdd = 0;

            modifiers.Sort(CompareModifierOrder);

            for (var i = 0; i < modifiers.Count; i++)
            {
                var mod = modifiers[i];

                if (mod.type == ModType.Flat)
                {
                    finalValue += mod.value;
                }
                else if (mod.type == ModType.PercentAdd)
                {
                    sumPercentAdd += mod.value;

                    if (i + 1 >= modifiers.Count || modifiers[i + 1].type != ModType.PercentAdd)
                    {
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }
                }
                else if (mod.type == ModType.PercentMult)
                {
                    finalValue *= 1 + mod.value;
                }
            }

            SyncClients(type, finalValue);
        }

        [Server]
        private void SyncClients(PegaStat type, float value)
        {
            // Workaround for float calculation errors, like displaying 12.00001 instead of 12
            _values[type] = (float)Math.Round(value, 4);
        }
    }
}