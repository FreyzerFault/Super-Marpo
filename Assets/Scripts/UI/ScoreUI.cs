using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreUI : MonoBehaviour
    {
        private TMP_Text _score;
        private TMP_Text _coins;
        private TMP_Text _gems;

        private void Awake()
        {
            TMP_Text[] txts = transform.GetComponentsInChildren<TMP_Text>();
            _score = txts[0];
            _coins = txts[1];
            _gems = txts[2];
        }
        
        private void Start()
        {
            UpdateScore();
            UpdateCoins();
            UpdateGems();
            
            ScoreManager.Instance.onCoinCollected += OnCoinCollected;
            ScoreManager.Instance.onGemCollected += OnGemCollected;
        }

        private void OnDestroy()
        {
            ScoreManager.Instance.onCoinCollected -= OnCoinCollected;
            ScoreManager.Instance.onGemCollected -= OnGemCollected;
        }

        private void Update() => UpdateScore();

        
        private void OnCoinCollected()
        {
            UpdateCoins();
            UpdateScore();
        }
    
        private void OnGemCollected()
        {
            UpdateGems();
            UpdateScore();
        }

        
        private void UpdateScore() => _score.text = ScoreManager.Instance.Score.ToString();
        private void UpdateCoins() => _coins.text = ScoreManager.Instance.Coins.ToString();
        private void UpdateGems() => _gems.text = ScoreManager.Instance.Gems.ToString();
    }
}
