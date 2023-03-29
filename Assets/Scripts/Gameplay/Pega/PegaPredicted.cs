using FishNet;
using FishNet.Component.Animating;
using FishNet.Connection;
using FishNet.Object;
using Gameplay.Input;
using UnityEngine;

namespace Gameplay.Pega
{
    public partial class PegaPredicted : NetworkBehaviour
    {
        private Rigidbody _rigidbody;
        private NetworkAnimator _netAnimator;
        private Transform _cTransform; // cached transform, high performance


        private void Awake()
        {
            _netAnimator = GetComponent<NetworkAnimator>();
            _rigidbody = GetComponent<Rigidbody>();
            _cTransform = transform;

            InstanceFinder.TimeManager.OnTick += Move_OnTick;
            InstanceFinder.TimeManager.OnPostTick += Move_OnPostTick;

            // _initOffset = -transform.position.x;
        }

        private void OnDestroy()
        {
            if (InstanceFinder.TimeManager != null)
            {
                InstanceFinder.TimeManager.OnTick -= Move_OnTick;
                InstanceFinder.TimeManager.OnPostTick -= Move_OnPostTick;
            }
        }


        public override void OnStartClient()
        {
            base.OnStartClient();

            InjectInputs();
        }

        private void InjectInputs()
        {
            if (IsOwner)
            {
                _moveInput = ClientInput.Self.move;
            }

            //if BOT input form AI class
        }
        
        
        public void SetOwnerShip(NetworkConnection connection)
        {
            GiveOwnership(connection);
        }
        
        public override void OnOwnershipServer(NetworkConnection prevOwner)
        {
            base.OnOwnershipServer(prevOwner);
            ClearReplicateCache(true);
        }
    }
}