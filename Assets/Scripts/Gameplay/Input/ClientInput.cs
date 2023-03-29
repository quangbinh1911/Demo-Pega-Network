using UnityEngine;

namespace Gameplay.Input
{
    public class ClientInput : MonoBehaviour
    {
        public static ClientInput Self;

        public MoveInput move;

        private void Awake()
        {
            Self = this;
        }
    }
}