using Managers;
using UnityEngine;
using Utils.VFX;

namespace Player
{
    public class GoalTrigger : PlayerTrigger
    {
        [Header("Goal Fade Out")]
        public float fadeDuration = 1f;
        
        protected override void HandlePlayerTrigger()
        {
            base.HandlePlayerTrigger();
            FadeUI.FadeOut(fadeDuration, () => GameManager.LoadScene("End"));
        }
    }
}
