using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Utils.VFX
{
    /// <summary>
    /// Spawn a BLACK Image that covers the whole screen with a Fade effect.
    /// </summary>
    public class FadeUI : Singleton<FadeUI>
    {
        private Image _img;

        protected override void Awake()
        {
            base.Awake();
            _img = GetComponent<Image>();
        }

        /// Reuse the same instance everytime to do a Fade effect.
        /// The first time, if no FadeUI is found, it will instantiate one.
        public static void Fade(bool fadeOut, float durationInSecs = 1f, Action onEndFade = null)
        {
            if (Instance == null) Instantiate();
            
            Instance._img.SetAlpha(fadeOut ? 0 : 1);
            
            Instance._img
                .DOFade(fadeOut ? 1 : 0, durationInSecs)
                .OnComplete(() => onEndFade?.Invoke());
        }
        
        public static void FadeIn(float durationInSecs = 1f, Action onEndFade = null) =>
            Fade(fadeOut: false, durationInSecs, onEndFade);
        
        public static void FadeOut(float durationInSecs = 1f, Action onEndFade = null) =>
            Fade(fadeOut: true, durationInSecs, onEndFade);


        private static void Instantiate() => 
            Instantiate(Resources.Load<FadeUI>($"Prefabs/Fade UI"), GameObject.Find("Canvas").transform);
    }
}
