using System.Collections;
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

	private void CreateStocks () {
		Stock wood = new Stock("Wood", 10000, 100, 2);
		Stock paper = new Stock("Paper", 20000, 100, 2);
		Stock napkin = new Stock("Napkin", 1000, 10, 5);

		Stock iron = new Stock("Raw Iron", 20000, 80, 3);
		Stock steel = new Stock("Steel", 20000, 150, 2);
		Stock construction = new Stock("Construction Materials", 12000, 40, 4);

		Stock computers = new Stock("Computer Hardware", 15000, 120, 5);
		Stock software = new Stock("Software", 16000, 200, 4);

		CreateRelation(wood, paper, 0.5f);
		CreateRelation(wood, iron, -0.2f);
		CreateRelation(wood, computers, 0.1f);
		CreateRelation(paper, napkin, 0.4f);
		CreateRelation(napkin, software, 0.4f);
		CreateRelation(iron, steel, 0.6f);
		CreateRelation(iron, wood, -0.2f);
		CreateRelation(steel, construction, 0.2f);
		CreateRelation(iron, construction, 0.1f);
		CreateRelation(computers, software, 1f);
		CreateRelation(software, napkin, 0.2f);

		this.Stocks.Add(wood);
		this.Stocks.Add(paper);
		this.Stocks.Add(napkin);
		this.Stocks.Add(iron);
		this.Stocks.Add(steel);
		this.Stocks.Add(construction);
		this.Stocks.Add(computers);
		this.Stocks.Add(software);
	}

	private void CreateRelation(Stock from, Stock to, float multiplier)
	{
		StockRelation rel;
		rel.stock = to;
		rel.priceChangePerTransaction = multiplier;
		from.AddRelation(rel);
	}
}
