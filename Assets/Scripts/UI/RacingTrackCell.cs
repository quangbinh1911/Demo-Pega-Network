using EnhancedUI.EnhancedScroller;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RacingTrackCell : EnhancedScrollerCellView
    {
        [SerializeField] private Image avatar;
        [SerializeField] private TMP_Text idTxt;
        [SerializeField] private VelocityDisplay velocityTxt;


        public void SetInfo(PlayerInfo info)
        {
            avatar.sprite = Resources.Load<Sprite>($"avatar_{Mathf.Repeat(info.AvatarID, 3):000}");
            idTxt.SetText(info.Name.ToString());
        }

        public void SetVelocity(float velocity)
        {
            velocityTxt.UpdateView(velocity);
        }
    }
}