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

    public Text FundsText;
    public Text VolatilityText;

    public GameObject VolatilityLabel;

    private float _buyPrice;
    private float _sellPrice;
    private int _funds;
    private float _volatility;

    private Dictionary<StockType, string> _stockNameLookup;
    private Dictionary<int, string> _integerCache;
    private Dictionary<int, string> _floatCache; // Two decimal float cache, key is the wanted float *100f floored

    private int _currentPlayerSelectedStationIndex;

    void Awake()
    {
        _integerCache = new Dictionary<int, string>();
        _floatCache = new Dictionary<int, string>();

        _stockNameLookup = new Dictionary<StockType, string>();
        foreach (StockType type in Enum.GetValues(typeof(StockType)))
        {
            var name = type.ToString();
            _stockNameLookup[type] = name;
        }

        _currentPlayerSelectedStationIndex = GameManager.Instance.PlayerSelectedStation;
        StockNameText.text = "";
        FundsText.text = GetCachedFloatTwoDecimals(GameManager.Instance.PlayerFunds);

        VolatilityText.text = "";
        VolatilityLabel.SetActive(false);

        BuySellOverlay.SetActive(false);
    }

    void Update()
    {
        var currentIndex = GameManager.Instance.PlayerSelectedStation;
        if (currentIndex == -1) return;

        // Reactivate volatility label
        if (!VolatilityLabel.activeInHierarchy)
        {
            VolatilityLabel.SetActive(true);
        }

        var currentType = StockManager.Instance.GetType(GameManager.Instance.PlayerSelectedStation);

        // Update the name of the stock station
        if (GameManager.Instance.PlayerSelectedStation != _currentPlayerSelectedStationIndex)
        {
            StockNameText.text = _stockNameLookup[currentType];
        }

        BuySellOverlay.gameObject.SetActive(GameManager.Instance.CloseToTarget);

        if (currentIndex != -1 && GameManager.Instance.CloseToTarget)
        {
            if (GameManager.Instance.CloseToTarget)
            {
                var buyPrice = StockManager.Instance.Stocks[currentIndex].BuyPrice();
                var sellPrice = StockManager.Instance.Stocks[currentIndex].SellPrice();
                var buyAmount = StockManager.Instance.Stocks[currentIndex].AmountOnMarket -
                    GameManager.Instance.PlayerPortfolio[currentType];
                var sellAmount = GameManager.Instance.PlayerPortfolio[currentType];

                // Update the floats, truncated to 2 decimals
                if (Mathf.Abs(_buyPrice - buyPrice) > 0.01f)
                {
                    _buyPrice = buyPrice;
                    BuyPrice.text = GetCachedFloatTwoDecimals(buyPrice);
                }

                if (Mathf.Abs(_sellPrice - sellPrice) > 0.01f)
                {
                    _sellPrice = sellPrice;
                    SellPrice.text = GetCachedFloatTwoDecimals(sellPrice);
                }

                // Buy and sell amounts
                BuyAmount.text = GetCachedInteger(buyAmount);
                SellAmount.text = GetCachedInteger(sellAmount);
            }

            var volatility = StockManager.Instance.Stocks[currentIndex].Volatility;
            var funds = GameManager.Instance.PlayerFunds;

            // Volatility
            if (Mathf.Abs(_volatility - volatility) > 0.01f)
            {
                _volatility = volatility;
                VolatilityText.text = GetCachedFloatTwoDecimals(volatility);
            }

            // Funds
            FundsText.text = GetCachedFloatTwoDecimals(funds);
        }
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
            var sfx = GameManager.Instance.BuySFXPool.GetPooledObject();
            sfx.SetActive(true);
            GameManager.Instance.BuyStock(currentType);
        }
    }

    public void SellClicked()
    {
        var currentType = StockManager.Instance.GetType(GameManager.Instance.PlayerSelectedStation);
        if (GameManager.Instance.CanSellStock(currentType))
        {
            var sfx = GameManager.Instance.SellSFXPool.GetPooledObject();
            sfx.SetActive(true);
            GameManager.Instance.SellStock(currentType);
        }
    }

    private string GetCachedInteger(int i)
    {
        if (_integerCache.ContainsKey(i))
        {
            return _integerCache[i];
        }

        var addition = i.ToString();
        _integerCache[i] = addition;
        return addition;
    }

    private string GetCachedFloatTwoDecimals(float f)
    {
        var truncatedInteger = Mathf.FloorToInt(f * 100f);
        if (_floatCache.ContainsKey(truncatedInteger))
        {
            return _floatCache[truncatedInteger];
        }

        var addition = (truncatedInteger / 100f).ToString();
        _floatCache[truncatedInteger] = addition;
        return addition;
    }
}
