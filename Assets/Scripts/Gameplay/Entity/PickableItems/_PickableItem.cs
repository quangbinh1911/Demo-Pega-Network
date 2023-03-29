using System.Linq;
using FishNet;
using FishNet.Object;
using Gameplay.Ability;
using Gameplay.Pega;
using UnityEngine;

namespace Gameplay.Entity.PickableItems
{
    public abstract class _PickableItem : NetworkBehaviour
    {
        public _AbilityType type;
        private bool _picked;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer && _picked) return;

            if(other.attachedRigidbody == null) return;
            
            var picker = other.attachedRigidbody.GetComponent<PegaPredicted>();
            if (picker == null) return;

            Debug.Log($"{picker} pickup {name}");
            _picked = true;
            
            OnPickup(picker);
            ReturnPool();
        }

        protected abstract void OnPickup(PegaPredicted picker);

        protected virtual void ReturnPool()
        {
            InstanceFinder.ServerManager.Despawn(gameObject);
        }
    }
}