using Gameplay.Ability;
using Gameplay.Pega;

namespace Gameplay.Entity.PickableItems
{
    public class ItemAbility : _PickableItem
    {
        protected sealed override void OnPickup(PegaPredicted picker)
        {
            _ServerAbility.Instance.Spawn(picker, type);
            //xử lý UI các kiểu con đà điểu ở đây

            AfterPickup(picker);
        }

        protected virtual void AfterPickup(PegaPredicted picker)
        {
        }
    }
}