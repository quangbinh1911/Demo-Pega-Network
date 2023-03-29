using FishNet.Object;
using Gameplay.Pega;
using UnityEngine;

namespace Gameplay.Ability
{
    public class _ServerAbility : NetworkBehaviour
    {
        public static _ServerAbility Instance;

        public override void OnStartServer()
        {
            base.OnStartServer();
            Instance = this;
        }


        [ServerRpc(RequireOwnership = false)]
        public void Spawn(PegaPredicted owner, _AbilityType type)
        {
            var key = type.SpawnKey();
            var abilityGo = Object.Instantiate(Resources.Load<GameObject>(key));

            var ability = abilityGo.GetComponent<_AbilityBase>();
            {
                ability.owner = owner;
            }

            ServerManager.Spawn(abilityGo, owner.Owner);
        }
    }
}