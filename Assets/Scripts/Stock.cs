using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Stock
{
    public string Name;
    public float PriceChangePerTransaction;
    public float MarketCap;
    public float AmountOnMarket;

    public List<StockRelation> Relations;

    public Stock(string name,
        float marketCap,
        float amountOnMarket,
        float priceChangePerTransaction,
        List<StockRelation> relations = null)
    {
        this.Name = name;
        this.MarketCap = marketCap;
        this.AmountOnMarket = amountOnMarket;
        this.PriceChangePerTransaction = priceChangePerTransaction;
        this.Relations = relations == null ? new List<StockRelation>() : null;
    }

	public void AddRelation(StockRelation relation)
	{
		this.Relations.Add(relation);
	}

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

    public void RelatedSold(float priceChange)
    {
        this.MarketCap -= priceChange;
        if (Mathf.Abs(priceChange) > 0.5f)
        {
            foreach (StockRelation rel in this.Relations)
            {
                rel.stock.RelatedSold(rel.priceChangePerTransaction / 2);
            }
        }
    }

    public void RelatedBought(float priceChange)
    {
        this.MarketCap += priceChange;
        if (Mathf.Abs(priceChange) > 0.5f)
        {
            foreach (StockRelation rel in this.Relations)
            {
                rel.stock.RelatedBought(rel.priceChangePerTransaction / 2);
            }
        }
    }
}

public struct StockRelation
{
    public Stock stock;
    public float priceChangePerTransaction;
}