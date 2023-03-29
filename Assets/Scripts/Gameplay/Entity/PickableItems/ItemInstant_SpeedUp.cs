using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using Gameplay.Pega;
using Gameplay.Stats;

namespace Gameplay.Entity.PickableItems
{
    public class ItemInstant_SpeedUp : ItemInstant
    {
        public float duration = 5f;

        protected override void Execute(PegaPredicted picker)
        {
            AddEffect(picker, duration).Forget();
        }

        private static async UniTaskVoid AddEffect(PegaPredicted picker, float duration)
        {
            var modifier = new Modifier(0.5f, ModType.PercentAdd);
            picker.AddModifier(PegaStat.MaxSpeed, modifier);

            var notify = AddNotify.New($"Speed Up {modifier.value:P0} on {duration} seconds!");
            InstanceFinder.ServerManager.Broadcast(new HashSet<NetworkConnection> { picker.Owner }, notify);

            await UniTask.Delay(TimeSpan.FromSeconds(duration));

            picker.RemoveModifier(PegaStat.MaxSpeed, modifier);
            InstanceFinder.ServerManager.Broadcast(new HashSet<NetworkConnection> { picker.Owner }, notify.GetRemove());
        }
    }
}

public struct AddNotify : IBroadcast
{
    public string Guid;
    public string Content;

    public static AddNotify New(string content)
    {
        return new AddNotify()
        {
            Guid = System.Guid.NewGuid().ToString(),
            Content = content,
        };
    }

    public RemoveNotify GetRemove()
    {
        return new RemoveNotify()
        {
            Guid = this.Guid,
        };
    }
}

public struct RemoveNotify : IBroadcast
{
    public string Guid;
}