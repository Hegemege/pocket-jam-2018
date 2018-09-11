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
    public Dictionary<StockType, int> PlayerPortfolio = new Dictionary<StockType, int>();

    // Privates
    public List<StockStationController> StockStations;
    public int PlayerSelectedStation = -1; // Initially none

    public bool CanBuyStock(StockType name)
    {
        List<Stock> stocks = StockManager.Instance.Stocks;
        for (int i = 0; i < stocks.Count; i++)
        {
            if (stocks[i].Name == name && PlayerFunds >= stocks[i].Price)
            {
                return true;
            }
        }
        return false;
    }

    public bool CanSellStock(StockType name)
    {
        return PlayerPortfolio[name] > 0;
    }

    public void BuyStock(StockType name)
    {
        List<Stock> stocks = StockManager.Instance.Stocks;
        for (int i = 0; i < stocks.Count; i++)
        {
            if (stocks[i].Name == name && PlayerFunds >= stocks[i].Price)
            {
                PlayerFunds -= stocks[i].Buy();
                PlayerPortfolio[name]++;
                return;
            }
        }
    }

    public void SellStock(StockType name)
    {
        if (PlayerPortfolio[name] > 0)
        {
            List<Stock> stocks = StockManager.Instance.Stocks;
            for (int i = 0; i < stocks.Count; i++)
            {
                if (stocks[i].Name == name)
                {
                    PlayerFunds += stocks[i].Sell();
                    PlayerPortfolio[name]--;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Gets the world location of the given station name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Vector3 GetStockStationPosition(StockType name)
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

        SetupPortfolio();
    }

    private void SetupPortfolio()
    {
        PlayerPortfolio.Add(StockType.Alcohol, 0);
        PlayerPortfolio.Add(StockType.Restoration, 0);
        PlayerPortfolio.Add(StockType.Food, 0);
        PlayerPortfolio.Add(StockType.Chemicals, 0);
        PlayerPortfolio.Add(StockType.Technology, 0);
        PlayerPortfolio.Add(StockType.Fuel, 0);
        PlayerPortfolio.Add(StockType.Tourism, 0);
        PlayerPortfolio.Add(StockType.Entertainment, 0);
    }
}