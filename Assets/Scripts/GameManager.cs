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

    // Publics
    public float PlayerFunds = 10000f;
    public Dictionary<string, int> PlayerPortfolio = new Dictionary<string, int>();

    public List<StockStationController> StockStations;
    public int PlayerSelectedStation = -1; // Initially none

    // Privates


    public bool CanBuyStock(string name)
    {
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

    public bool CanSellStock(string name)
    {
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

    /// <summary>
    /// Gets the world location of the given station name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Vector3 GetStockStationPosition(string name)
    {
        for (var i = 0; i < StockStations.Count; i++)
        {
            if (StockStations[i].StockName == name)
            {
                return StockStations[i].transform.position;
            }
        }

        Debug.LogWarning("Could not find stock station " + name);
        return Vector3.zero;
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