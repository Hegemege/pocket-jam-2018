using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public Image StockGraph;
    public GameObject BuySellOverlay;
    public Text StockNameText;

    public Text BuyPrice;
    public Text BuyAmount;
    public Text SellPrice;
    public Text SellAmount;

    private float _buyPrice;
    private int _buyAmount;
    private float _sellPrice;
    private float _sellAmount;

    private Dictionary<StockType, string> _stockNameLookup;
    private Dictionary<int, string> _integerCache;

    private int _currentPlayerSelectedStationIndex;

    void Awake()
    {
        _stockNameLookup = new Dictionary<StockType, string>();
        foreach (StockType type in Enum.GetValues(typeof(StockType)))
        {
            var name = type.ToString();
            _stockNameLookup[type] = name;
        }

        _currentPlayerSelectedStationIndex = GameManager.Instance.PlayerSelectedStation;
        StockNameText.text = "";

        BuySellOverlay.SetActive(false);
    }

    void Update()
    {
        var currentIndex = GameManager.Instance.PlayerSelectedStation;
        if (currentIndex == -1) return;

        var currentType = StockManager.Instance.GetType(GameManager.Instance.PlayerSelectedStation);

        if (GameManager.Instance.PlayerSelectedStation != _currentPlayerSelectedStationIndex)
        {
            StockNameText.text = _stockNameLookup[currentType];
        }

        BuySellOverlay.gameObject.SetActive(GameManager.Instance.CloseToTarget);

        if (currentIndex != -1 && GameManager.Instance.CloseToTarget)
        {
            var buyPrice = StockManager.Instance.Stocks[currentIndex].BuyPrice();
            var sellPrice = StockManager.Instance.Stocks[currentIndex].SellPrice();
            var buyAmount = StockManager.Instance.Stocks[currentIndex].AmountOnMarket -
                GameManager.Instance.PlayerPortfolio[currentType];
            var sellAmount = GameManager.Instance.PlayerPortfolio[currentType];

            var volatility = StockManager.Instance.Stocks[currentIndex].Volatility;

            // Update buy/sell counts and prices
            GameObject.Find("BuyText").GetComponent<Text>().text = buyPrice.ToString();
            GameObject.Find("BuyCount").GetComponent<Text>().text = buyAmount.ToString();
            GameObject.Find("SellText").GetComponent<Text>().text = sellPrice.ToString();
            GameObject.Find("SellCount").GetComponent<Text>().text = sellAmount.ToString();

            GameObject.Find("VolatilityCounter").GetComponent<Text>().text = "Volatility: " + volatility.ToString();
        }

        GameObject.Find("PlayerFunds").GetComponent<Text>().text = "Funds: " + GameManager.Instance.PlayerFunds.ToString();
    }

    public void PreviousStation()
    {
        GameManager.Instance.SetPlayerTarget(-1);
    }

    public void NextStation()
    {
        GameManager.Instance.SetPlayerTarget(1);
    }

    public void BuyClicked()
    {
        var currentType = StockManager.Instance.GetType(GameManager.Instance.PlayerSelectedStation);
        if (GameManager.Instance.CanBuyStock(currentType))
        {
            GameManager.Instance.BuyStock(currentType);
        }
    }

    public void SellClicked()
    {
        var currentType = StockManager.Instance.GetType(GameManager.Instance.PlayerSelectedStation);
        if (GameManager.Instance.CanSellStock(currentType))
        {
            GameManager.Instance.SellStock(currentType);
        }
    }

    private string GetCachedInteger(int i)
    {
        if (_integerCache.ContainsKey(i))
        {
            return _integerCache[i];
        }
        else
        {
            var addition = i.ToString();
            _integerCache[i] = addition;
            return addition;
        }
    }
}
