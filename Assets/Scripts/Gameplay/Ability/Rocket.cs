using System;
using FishNet.Connection;
using FishNet.Object;
using Gameplay.Input;
using Gameplay.Pega;
using Gameplay.Stats;
using Gameplay.UX;
using UnityEngine;

namespace Gameplay.Ability
{
    public class Rocket : NetworkBehaviour
    {
        public float force = 1f;
        public float fixedY = 1.75f;
        public float maxDistance = 100f;

        private Transform _cachedTransform;
        private bool _isShooting = false;
        private float _currentDistance;


        private void Awake()
        {
            _cachedTransform = transform;
        }
        
        private void OnDisable()
        {
            _isShooting = false;
        }


        public void Shoot()
        {
            if (!IsHost) return;
            
            _isShooting = true;
            _currentDistance = 0f;
        }

        private void Update()
        {
            if (!IsServer) return;


            var oldPosition = _cachedTransform.position;
            var newPosition = oldPosition;
            {
                newPosition += _cachedTransform.forward * (force * Time.deltaTime);
                newPosition = new Vector3(newPosition.x, fixedY, newPosition.z);
            }

            SetPosition(newPosition);


            _currentDistance += Vector3.Distance(oldPosition, newPosition);
            if (_currentDistance >= maxDistance) ReturnPool();
        }

        public void SetPosition(Vector3 position)
        {
            position = new Vector3(position.x, fixedY, position.z);
            _cachedTransform.position = position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isShooting && IsServer)
            {
                OnHit(other);
            }
        }

        private void OnHit(Collider other)
        {
            if (other.attachedRigidbody)
            {
                var root = other.attachedRigidbody;

                var target = root.GetComponent<NetworkBehaviour>();
                if (target && target.Owner.ClientId == Owner.ClientId) return;


                var targetStats = root.GetComponent<PegaPredicted>();
                if (targetStats != null)
                {
                    targetStats.AddModifier(PegaStat.MaxSpeed, new Modifier(-0.5f, ModType.PercentAdd));
                }
            }


            Debug.Log($"Hit {other}");
            Fx.Instance.Spawn("rocket_hit", _cachedTransform.position, _cachedTransform.rotation);
            
            ReturnPool();
        }

        private void ReturnPool()
        {
            ServerManager.Despawn(gameObject);
        }
    }
}