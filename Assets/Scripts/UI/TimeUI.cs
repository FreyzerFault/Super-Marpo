using System;
using System.Collections;
using Data;
using DG.Tweening;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TimeUI : MonoBehaviour
    {
        [Header("Go To Center of Screen Animation")]
        [SerializeField] Transform target;
        [SerializeField] private float moveDuration = 0.5f;
        
        private TMP_Text _txt;

        private void Awake() => _txt = GetComponent<TMP_Text>();

        private void Start() => GameManager.Instance.onGameStateChanged += OnGameStateChanged;
        private void OnDestroy() => GameManager.Instance.onGameStateChanged -= OnGameStateChanged;

        private void OnGameStateChanged()
        {
            _txt.color =
                GameManager.Instance.State == GameManager.GameState.GoalReached
                    ? Color.yellow
                    : Color.white;
            
            if (GameManager.Instance.State == GameManager.GameState.GoalReached)
                GoToCenterOfScreen();
        }

        private void Update()
        {
            if (!GameManager.Instance.IsPlaying) return;
            
            TimeSpan ts = TimeSpan.FromSeconds(DataManager.Instance.GameTime);
            _txt.text = ts.ToString($@"{(ts.Minutes > 0 ? @"mm\:" : "")}ss\:ff").Replace(":", " :");
        }

        private void GoToCenterOfScreen()
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.pivot = new Vector2(0.5f, 0.5f);
            _txt.alignment = TextAlignmentOptions.Center;
            rt.DOScale(new Vector3(3, 3, 3), moveDuration).SetEase(Ease.InOutSine);
            rt.DOMove(target.position, moveDuration).SetEase(Ease.InOutSine);
        }
    }
}
