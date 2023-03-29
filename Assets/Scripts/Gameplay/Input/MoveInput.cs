using UnityEngine;

namespace Gameplay.Input
{
    public abstract class MoveInput : MonoBehaviour
    {
        public abstract Pega.PegaPredicted.MoveData Generate();
    }
}