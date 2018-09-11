using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Stock
{
    public StockType Name;
    public float PriceChangePerTransaction;
    public float MarketCap;
    public float AmountOnMarket;
    public float Volatility;

    public List<StockRelation> Relations;

    public bool Closed;

    private int Timer;
    private int ClosedTimer;

    public Stock(StockType name,
        float marketCap,
        float amountOnMarket,
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
            return MarketCap / AmountOnMarket;
        }
    }

    public float SellPrice()
    {
        return Price * 0.95f;
    }
    public float BuyPrice()
    {
        return Price * 1.05f;
    }

    /// <summary>
    /// Do one "sell" operation on this stock and cause its effects
    /// </summary>
    /// <returns>Amount of money received from the transaction</returns>
    public float Sell()
    {
        float currentPrice = this.Price;
        this.MarketCap -= this.PriceChangePerTransaction;
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
        float currentPrice = this.Price;
        this.MarketCap += this.PriceChangePerTransaction;
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
        this.MarketCap -= priceChange;
        if (Mathf.Abs(priceChange) > 0.1f)
        {
            foreach (StockRelation rel in this.Relations)
            {
                rel.stock.RelatedSold(rel.priceChangePerTransaction / 4);
            }
        }
    }

    /// <summary>
    /// Propagate effects of a stock bought
    /// </summary>
    /// <param name="priceChange"></param>
    public void RelatedBought(float priceChange)
    {
        this.MarketCap += priceChange;
        if (Mathf.Abs(priceChange) > 0.1f)
        {
            foreach (StockRelation rel in this.Relations)
            {
                rel.stock.RelatedBought(rel.priceChangePerTransaction / 4);
            }
        }
    }

    public void Fluctuate()
    {
        this.Timer++;

        if (!this.Closed)
        {
            this.MarketCap = this.MarketCap + Mathf.Sin(Time.time) * Volatility;

            if (Volatility > StockManager.Instance.VolatilityThreshold)
            {
                this.Closed = true;
                this.ClosedTimer = this.Timer;
            }
        } else {
            if (this.ClosedTimer >= this.Timer + StockManager.Instance.VolatilityLockTime) {
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