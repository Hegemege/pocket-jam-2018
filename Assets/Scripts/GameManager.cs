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
    public bool isPlaying = true;
    public Dictionary<StockType, int> PlayerPortfolio = new Dictionary<StockType, int>();
    public bool CloseToTarget;

    // Referenes
    public PlayerController PlayerController;
    public WorldCameraController WorldCamera;

    // Privates
    public List<StockStationController> StockStations;
    public int PlayerSelectedStation = -1; // Initially none

    /// <summary>
    /// Check if the player has the required funds to buy a stock
    /// </summary>
    /// <param name="name">Stock name</param>
    /// <returns></returns>
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

    /// <summary>
    /// Check if the player has a stock of the chosen type to sell
    /// </summary>
    /// <param name="name">Stock name</param>
    /// <returns></returns>
    public bool CanSellStock(StockType name)
    {
        return PlayerPortfolio[name] > 0;
    }

    /// <summary>
    /// Buy a stock of the chosen type if able, and add it to the portfolio
    /// </summary>
    /// <param name="name">Stock name</param>
    public void BuyStock(StockType name)
    {
        List<Stock> stocks = StockManager.Instance.Stocks;
        for (int i = 0; i < stocks.Count; i++)
        {
            if (stocks[i].Name == name && PlayerFunds >= stocks[i].Price)
            {
                Debug.Log("Buying " + name);
                PlayerFunds -= stocks[i].Buy();
                PlayerPortfolio[name]++;
                return;
            }
        }
    }

    /// <summary>
    /// Sell a stock of the chosen type if able, and remove it from the portfolio
    /// </summary>
    /// <param name="name">Stock name</param>
    public void SellStock(StockType name)
    {
        if (PlayerPortfolio[name] > 0)
        {
            List<Stock> stocks = StockManager.Instance.Stocks;
            for (int i = 0; i < stocks.Count; i++)
            {
                if (stocks[i].Name == name)
                {
                    Debug.Log("Selling " + name);
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

    /// <summary>
    /// Set the target station for the player's index, called from left/right arrow buttons
    /// </summary>
    /// <param name="diff"></param>
    public void SetPlayerTarget(int diff)
    {
        PlayerSelectedStation = ModFix.Mod(PlayerSelectedStation + diff, StockStations.Count);
        StockType newType = StockManager.Instance.GetType(PlayerSelectedStation);
        if (PlayerController)
        {
            PlayerController.SetMoveTarget(GetStockStationPosition(newType));
        }
    }

    public void SetPlayerTarget(StockType stock)
    {

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

    /// <summary>
    /// Initialize the player's portfolio dictionary
    /// </summary>
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