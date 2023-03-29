using Gameplay.Pega;

namespace Gameplay.Entity.PickableItems
{
    public class ItemAbility_Rocket : ItemAbility
    {
        protected override void AfterPickup(PegaPredicted picker)
        {
            // Chẳng hạn Cảnh báo thằng này nhặt được quả tên lửa rất nguy hiểm
        }
    }
}