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

    private Dictionary<StockType, string> _stockNameLookup;

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
        if (GameManager.Instance.PlayerSelectedStation != _currentPlayerSelectedStationIndex)
        {
            var nextType = StockManager.Instance.GetType(GameManager.Instance.PlayerSelectedStation);
            StockNameText.text = _stockNameLookup[nextType];
        }

        BuySellOverlay.gameObject.SetActive(GameManager.Instance.CloseToTarget);
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
}
