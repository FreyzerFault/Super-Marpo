using System;
using Data;
using Managers;
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
            
            DataManager.Instance.OnCollectableAdded += HandleOnCollectableAdded;
        }
        
        private void OnDestroy() => DataManager.Instance.OnCollectableAdded -= HandleOnCollectableAdded;

        private void Update() => UpdateScore();
        
        
        private void HandleOnCollectableAdded(CollectableType type)
        {
            switch (type)
            {
                case CollectableType.Coin:
                    UpdateCoins();
                    break;
                case CollectableType.Gem:
                    UpdateGems();
                    break;
            }
            
            UpdateScore();
        }
        
        private void UpdateScore() => _score.text = DataManager.Instance.Score.ToString();
        private void UpdateCoins() => _coins.text = DataManager.Instance.Coins.ToString();
        private void UpdateGems() => _gems.text = DataManager.Instance.Gems.ToString();
    }
}
