﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockManager : MonoBehaviour {

    //Singleton
    private static StockManager _instance;
    public static StockManager Instance
    {
        get
        {
            return _instance;
        }
    }

	public List<Stock> Stocks = new List<Stock>();

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

		CreateStocks();
    }

	/// <summary>
	/// Initialize the stocks
	/// </summary>
	private void CreateStocks () {
		Stock alcohol = new Stock(StockType.Alcohol, 12000, 100, 2);
		Stock restoration = new Stock(StockType.Restoration, 8000, 100, 2);
		Stock food = new Stock(StockType.Food, 10000, 100, 2);
		Stock chemicals = new Stock(StockType.Chemicals, 12000, 100, 2);
		Stock technology = new Stock(StockType.Technology, 8000, 100, 2);
		Stock fuel = new Stock(StockType.Fuel, 10000, 100, 2);
		Stock tourism = new Stock(StockType.Tourism, 12000, 100, 2);
		Stock entertainment = new Stock(StockType.Entertainment, 8000, 100, 2);

		CreateRelation(alcohol, restoration, 1f);
		CreateRelation(alcohol, entertainment, 1f);
		CreateRelation(alcohol, chemicals, -1f);
		CreateRelation(alcohol, technology, -1f);
		
		CreateRelation(restoration, alcohol, 1f);
		CreateRelation(restoration, food, 1f);
		CreateRelation(restoration, fuel, -1f);
		CreateRelation(restoration, tourism, -1f);
		
		CreateRelation(food, restoration, 1f);
		CreateRelation(food, chemicals, 1f);
		CreateRelation(food, fuel, -1f);
		CreateRelation(food, entertainment, -1f);
		
		CreateRelation(chemicals, food, 1f);
		CreateRelation(chemicals, technology, 1f);
		CreateRelation(chemicals, alcohol, -1f);
		CreateRelation(chemicals, tourism, -1f);
		
		CreateRelation(technology, chemicals, 1f);
		CreateRelation(technology, fuel, 1f);
		CreateRelation(technology, alcohol, -1f);
		CreateRelation(technology, entertainment, -1f);
		
		CreateRelation(fuel, technology, 1f);
		CreateRelation(fuel, tourism, 1f);
		CreateRelation(fuel, food, -1f);
		CreateRelation(fuel, restoration, -1f);
		
		CreateRelation(tourism, fuel, 1f);
		CreateRelation(tourism, entertainment, 1f);
		CreateRelation(tourism, restoration, -1f);
		CreateRelation(tourism, chemicals, -1f);
		
		CreateRelation(entertainment, alcohol, 1f);
		CreateRelation(entertainment, tourism, 1f);
		CreateRelation(entertainment, food, -1f);
		CreateRelation(entertainment, technology, -1f);

		this.Stocks.Add(alcohol);
		this.Stocks.Add(restoration);
		this.Stocks.Add(food);
		this.Stocks.Add(chemicals);
		this.Stocks.Add(technology);
		this.Stocks.Add(fuel);
		this.Stocks.Add(tourism);
		this.Stocks.Add(entertainment);
	}

	/// <summary>
	/// Create a relationship between two stocks
	/// </summary>
	/// <param name="from"></param>
	/// <param name="to"></param>
	/// <param name="multiplier"></param>
	private void CreateRelation(Stock from, Stock to, float multiplier)
	{
		StockRelation rel;
		rel.stock = to;
		rel.priceChangePerTransaction = multiplier;
		from.AddRelation(rel);
	}
}

public enum StockType {
	Alcohol, Restoration, Food, Chemicals, Technology, Fuel, Tourism, Entertainment
}