using System;
using Objects;
using Utils;

public class ScoreManager : Singleton<ScoreManager>
{
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
