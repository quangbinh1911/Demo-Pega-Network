using Gameplay.Pega;

namespace Gameplay.Entity.PickableItems
{
    public abstract class ItemInstant : _PickableItem
    {
        protected sealed override void OnPickup(PegaPredicted picker)
        {
            Execute(picker);
        }

        protected abstract void Execute(PegaPredicted picker);
    }
}