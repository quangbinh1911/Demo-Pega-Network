using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gameplay.Ability
{
    public class Ability_Rocket : _AbilityBase
    {
        private AddNotify _notify;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsServer)
            {
                _notify = AddNotify.New($"Press space to use Rocket!!");

                InstanceFinder.ServerManager.Broadcast(new HashSet<NetworkConnection> { Owner }, _notify);
            }
        }

        protected override void Execute()
        {
            var rocketGo = Object.Instantiate(Resources.Load<GameObject>("Rocket"));

            rocketGo.GetComponent<Rocket>().SetPosition(owner.transform.position);
            rocketGo.transform.rotation = Quaternion.LookRotation(owner.transform.forward);

            InstanceFinder.ServerManager.Spawn(rocketGo, owner.Owner);
            rocketGo.GetComponent<Rocket>().Shoot();
        }

        //test
        protected void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                Cast();

                if (IsServer)
                {
                    InstanceFinder.ServerManager.Broadcast(new HashSet<NetworkConnection> { Owner }, _notify.GetRemove());
                }
            }
        }
    }
}