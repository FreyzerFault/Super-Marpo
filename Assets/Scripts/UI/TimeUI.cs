using System;
using Data;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TimeUI : MonoBehaviour
    {
        private TMP_Text _txt;

        private void Awake() => _txt = GetComponent<TMP_Text>();

        private void Start() => GameManager.Instance.onGameStateChanged += OnGameStateChanged;

        private void OnGameStateChanged() => _txt.color = 
            GameManager.Instance.State == GameManager.GameState.GoalReached 
                ? Color.yellow 
                : Color.white;

        private void Update()
        {
            if (!GameManager.Instance.IsPlaying) return;
            
            TimeSpan ts = TimeSpan.FromSeconds(DataManager.Instance.GameTime);
            _txt.text = ts.ToString($@"{(ts.Minutes > 0 ? @"mm\:" : "")}ss\:ff").Replace(":", " :");
        }
    }
}
