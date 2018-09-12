using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stock
{
    public StockType Name;
    public float PriceChangePerTransaction;
    public float MarketCap;
    public int AmountOnMarket;
    public float Volatility;

    public List<StockRelation> Relations;

    public bool Closed;

    private int Timer;
    private int ClosedTimer;

    private float[] VolatilityFluctuationTimeMultipliers;
    private float[] VolatilityFluctuationScales;
    private float[] VolatilityFluctuationOffsets;

    private float startingMarketCap;
    private float startingVolatility;

    public Stock(StockType name,
        float marketCap,
        int amountOnMarket,
        float priceChangePerTransaction,
        float volatility,
        List<StockRelation> relations = null)
    {
        this.Name = name;
        this.MarketCap = marketCap;
        this.AmountOnMarket = amountOnMarket;
        this.PriceChangePerTransaction = priceChangePerTransaction;
        this.Volatility = volatility;
        this.Relations = relations == null ? new List<StockRelation>() : null;
        this.Closed = false;
        this.Timer = 0;
        this.ClosedTimer = -1;
        this.startingMarketCap = this.MarketCap;
        this.startingVolatility = this.Volatility;
        SetupVolatilityValues();
    }

    private void SetupVolatilityValues() {
        int length = StockManager.Instance.VolatilityFluctuationOffsetsMax.Length;
        VolatilityFluctuationTimeMultipliers = new float[length];
        VolatilityFluctuationScales = new float[length];
        VolatilityFluctuationOffsets = new float[length];
        for (int i = 0; i < length; i++) {
            VolatilityFluctuationTimeMultipliers[i] = Random.Range(StockManager.Instance.VolatilityFluctuationTimeMultipliersMin[i], StockManager.Instance.VolatilityFluctuationTimeMultipliersMax[i]);
            VolatilityFluctuationScales[i] = Random.Range(StockManager.Instance.VolatilityFluctuationScalesMin[i], StockManager.Instance.VolatilityFluctuationScalesMax[i]);
            VolatilityFluctuationOffsets[i] = Random.Range(StockManager.Instance.VolatilityFluctuationOffsetsMin[i], StockManager.Instance.VolatilityFluctuationOffsetsMax[i]);
        }
    }

    /// <summary>
    /// Add a relationship between this stock and another one
    /// </summary>
    /// <param name="relation"></param>
	public void AddRelation(StockRelation relation)
    {
        this.Relations.Add(relation);
    }

    /// <summary>
    /// Calculate the stock's price based on the market cap and the amount of stocks
    /// </summary>
    /// <value></value>
    public float Price
    {
        get
        {
            if (MarketCap > 0f)
            {
                return MarketCap / (float)AmountOnMarket;
            }
            return 0f;
        }
    }

    public float SellPrice()
    {
        float sellPrice = Price * StockManager.Instance.StockSellPriceMultiplier;
        if (sellPrice > 0) return sellPrice;
        return 0f;
    }
    public float BuyPrice()
    {
        float buyPrice = Price * StockManager.Instance.StockBuyPriceMultiplier;
        if (buyPrice > 0) return buyPrice;
        return 0f;
    }

    /// <summary>
    /// Do one "sell" operation on this stock and cause its effects
    /// </summary>
    /// <returns>Amount of money received from the transaction</returns>
    public float Sell()
    {
        float currentPrice = this.SellPrice();
        this.MarketCap += StockManager.Instance.StockSellPriceAbsoluteChange * AmountOnMarket;
        this.MarketCap = this.MarketCap * (1 - this.PriceChangePerTransaction) * Random.Range(StockManager.Instance.StockPriceRandomChangeBottom, StockManager.Instance.StockPriceRandomChangeTop);
        Volatility = Volatility * StockManager.Instance.VolatilityIncrease * Random.Range(StockManager.Instance.VolatilityRandomChangeBottom, StockManager.Instance.VolatilityRandomChangeTop);
        foreach (StockRelation rel in this.Relations)
        {
            rel.stock.RelatedSold(rel.priceChangePerTransaction);
        }
        return currentPrice;
    }

    /// <summary>
    /// Do one "buy" operation on this stock and cause its effects
    /// </summary>
    /// <returns>Amount of money spent on the transaction</returns>
    public float Buy()
    {
        float currentPrice = this.BuyPrice();
        this.MarketCap += StockManager.Instance.StockBuyPriceAbsoluteChange * AmountOnMarket;
        this.MarketCap = (this.MarketCap * (1 + this.PriceChangePerTransaction) * Random.Range(StockManager.Instance.StockPriceRandomChangeBottom, StockManager.Instance.StockPriceRandomChangeTop));
        Volatility = Volatility * StockManager.Instance.VolatilityIncrease * Random.Range(StockManager.Instance.VolatilityRandomChangeBottom, StockManager.Instance.VolatilityRandomChangeTop);
        foreach (StockRelation rel in this.Relations)
        {
            rel.stock.RelatedBought(rel.priceChangePerTransaction);
        }
        return currentPrice;
    }

    /// <summary>
    /// Propagate effects of a stock sold
    /// </summary>
    /// <param name="priceChange"></param>
    public void RelatedSold(float priceChange)
    {
        this.MarketCap += StockManager.Instance.StockSellPriceAbsoluteChange * AmountOnMarket;
        this.MarketCap = (this.MarketCap * (1 - priceChange) * Random.Range(StockManager.Instance.StockPriceRandomChangeBottom, StockManager.Instance.StockPriceRandomChangeTop));
        if (Mathf.Abs(priceChange) > StockManager.Instance.StockPriceChangeDistanceMinValue)
        {
            foreach (StockRelation rel in this.Relations)
            {
                rel.stock.RelatedSold(priceChange * StockManager.Instance.StockPriceChangeDistanceDecay);
            }
        }
    }

    /// <summary>
    /// Propagate effects of a stock bought
    /// </summary>
    /// <param name="priceChange"></param>
    public void RelatedBought(float priceChange)
    {
        this.MarketCap += StockManager.Instance.StockBuyPriceAbsoluteChange * AmountOnMarket;
        this.MarketCap = (this.MarketCap * (1 + priceChange) * Random.Range(StockManager.Instance.StockPriceRandomChangeBottom, StockManager.Instance.StockPriceRandomChangeTop));
        if (Mathf.Abs(priceChange) > StockManager.Instance.StockPriceChangeDistanceMinValue)
        {
            foreach (StockRelation rel in this.Relations)
            {
                rel.stock.RelatedBought(priceChange * StockManager.Instance.StockPriceChangeDistanceDecay);
            }
        }
    }

    public void Fluctuate()
    {
        this.Timer++;

        if (!this.Closed)
        {
            for (int i = 0; i < this.VolatilityFluctuationTimeMultipliers.Length; i++)
            {
                this.MarketCap = this.MarketCap + Mathf.Sin(Time.time * this.VolatilityFluctuationTimeMultipliers[i] + this.VolatilityFluctuationOffsets[i]) * Volatility * this.VolatilityFluctuationScales[i];
            }

            Volatility = Volatility * StockManager.Instance.VolatilityDecay;
            if (Volatility < StockManager.Instance.MinVolatility)
            {
                Volatility = StockManager.Instance.MinVolatility;
            }
            if (Volatility > StockManager.Instance.MaxVolatility)
            {
                Volatility = StockManager.Instance.MaxVolatility;
            }

            if (Volatility > StockManager.Instance.VolatilityThreshold)
            {
                Debug.Log("Stock " + Name + " closed");
                this.Closed = true;
                this.ClosedTimer = this.Timer;
            }
        }
        else
        {
            if (this.Timer >= this.ClosedTimer + StockManager.Instance.VolatilityLockTime)
            {
                Debug.Log("Stock " + Name + " reopened");
                this.Volatility = this.startingVolatility;
                this.MarketCap = this.startingMarketCap;
                this.Closed = false;
            }
        }
    }
}

public struct StockRelation
{
    public Stock stock;
    public float priceChangePerTransaction;
}