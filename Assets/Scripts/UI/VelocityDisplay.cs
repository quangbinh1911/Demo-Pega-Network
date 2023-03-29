using TMPro;
using UnityEngine;

namespace UI
{
    public class VelocityDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text txt;

        public void UpdateView(float velocity)
        {
            txt.SetText($"{velocity:0.00} km/h");
        }
    }
}