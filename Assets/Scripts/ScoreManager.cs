using System;
using System.Collections.Generic;
using Objects;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

public class ScoreManager : Singleton<ScoreManager>
{
    public static readonly Dictionary<string, int> ScorePerElement = new()
    {
        { "Coin", 100 },
        { "Gem", 5000 },
        { "Time", -1 }
    };

    public int initialScore = 2000;
    
    public int Score => 
        Mathf.FloorToInt(initialScore
                        + _coinsCollected * ScorePerElement["Coin"]
                        + _gemsCollected * ScorePerElement["Gem"]
                        + GameManager.Instance.GameTime * ScorePerElement["Time"]);
    
    public int Coins => _coinsCollected;
    public int Gems => _gemsCollected;
    
    private int _coinsCollected;
    private int _gemsCollected;

    public Action onCoinCollected;
    public Action onGemCollected;

    public void AddCollectable(Collectable.CollectableType type)
    {
        switch (type)
        {
            case Collectable.CollectableType.Coin:
                AddCoin();
                break;
            case Collectable.CollectableType.Gem:
                AddGem();
                break;
        }
    }
    
    private void AddCoin()
    {
        _coinsCollected++;
        onCoinCollected?.Invoke();
    }
    
    private void AddGem()
    {
        _gemsCollected++;
        onGemCollected?.Invoke();
    }
}
