using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public float PlayerFunds = 10000f;
    public Dictionary<string, int> PlayerPortfolio = new Dictionary<string, int>();

    public bool CanBuyStock(string name) {
        bool can = false;
        StockManager.Instance.Stocks.ForEach((stock) =>
        {
            if (stock.Name == name && PlayerFunds >= stock.Price)
            {
                can = true;
                return;
            }
        });
        return can;
    }

    public bool CanSellStock(string name) {
        return PlayerPortfolio[name] > 0;
    }

    public void BuyStock(string name)
    {
        StockManager.Instance.Stocks.ForEach((stock) =>
        {
            if (stock.Name == name && PlayerFunds >= stock.Price)
            {
                PlayerFunds -= stock.Buy();
                PlayerPortfolio[name]++;
                return;
            }
        });
    }

    public void SellStock(string name)
    {
        if (PlayerPortfolio[name] > 0)
        {
            StockManager.Instance.Stocks.ForEach((stock) =>
            {
                if (stock.Name == name)
                {
                    PlayerFunds += stock.Sell();
                    PlayerPortfolio[name]--;
                    return;
                }
            });
        }
    }

    void Awake()
    {
        // Setup singleton
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        _instance = this;
    }


}