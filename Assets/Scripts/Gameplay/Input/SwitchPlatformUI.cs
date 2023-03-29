using UnityEngine;

namespace Gameplay.Input
{
    public class SwitchPlatformUI : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            var input = Resources.Load<GameObject>($"Input/Keyboard");
            Instantiate(input, transform);

#elif UNITY_ANDROID || UNITY_IOS
            //load input UI mobile: Joystick, other buttons
#endif
        }
    }
}