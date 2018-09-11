using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockManager : MonoBehaviour
{
    public int VolatilityThreshold = 20;
    public int VolatilityLockTime = 20;

    //Singleton
    private static StockManager _instance;
    public static StockManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public LineRenderer graph;

    public List<Stock> Stocks = new List<Stock>();

    private Dictionary<StockType, float[]> stockHistory = new Dictionary<StockType, float[]>();

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
        StartCoroutine(Tick());
    }

    private IEnumerator Tick()
    {
        while (true)
        {
            Debug.Log(GameManager.Instance.isPlaying);
            if (GameManager.Instance.isPlaying)
            {
                UpdateStocks();
                UpdateGraph();
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void UpdateGraph()
    {
        if (GameManager.Instance.PlayerSelectedStation == 0)
        {
            float[] array = stockHistory[StockManager.Instance.GetType(GameManager.Instance.PlayerSelectedStation)];
            Vector3[] positions = new Vector3[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                Vector3 pos = new Vector3();
                pos.x = ((float)i).Remap(0f, 99f, -2.8f, 2.8f);
                pos.y = array[i].Remap(0f, 1000f, 3.2f, 5.7f);
                positions[i] = pos;
            }
            graph.SetPositions(positions);
        }
    }

    private void UpdateStocks()
    {
        foreach (Stock stock in this.Stocks)
        {
            stock.Fluctuate();
            AddToHistory(stock.Name, stock.Price);
        }
    }

    private void AddToHistory(StockType type, float price)
    {
        if (type != StockType.None)
        {
            float[] array = stockHistory[type];
            for (int i = 0; i < array.Length; i++)
            {
                if (i < array.Length - 1)
                {
                    array[i] = array[i + 1];
                }
                if (i == array.Length - 1)
                {
                    array[i] = price;
                }
            }
            stockHistory[type] = array;
        }
    }

    /// <summary>
    /// Initialize the stocks
    /// </summary>
    private void CreateStocks()
    {
        Stock alcohol = new Stock(StockType.Alcohol, 12000, 100, 2f, 2f);
        Stock restoration = new Stock(StockType.Restoration, 8000, 100, 2f, 2f);
        Stock food = new Stock(StockType.Food, 10000, 100, 2f, 2f);
        Stock chemicals = new Stock(StockType.Chemicals, 12000, 100, 2f, 2f);
        Stock technology = new Stock(StockType.Technology, 8000, 100, 2f, 2f);
        Stock fuel = new Stock(StockType.Fuel, 10000, 100, 2f, 2f);
        Stock tourism = new Stock(StockType.Tourism, 12000, 100, 2f, 2f);
        Stock entertainment = new Stock(StockType.Entertainment, 8000, 100, 2f, 2f);

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

        this.stockHistory.Add(StockType.Alcohol, new float[100]);
        this.stockHistory.Add(StockType.Restoration, new float[100]);
        this.stockHistory.Add(StockType.Food, new float[100]);
        this.stockHistory.Add(StockType.Chemicals, new float[100]);
        this.stockHistory.Add(StockType.Technology, new float[100]);
        this.stockHistory.Add(StockType.Fuel, new float[100]);
        this.stockHistory.Add(StockType.Tourism, new float[100]);
        this.stockHistory.Add(StockType.Entertainment, new float[100]);
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

    /// <summary>
    /// Get the index of the given stock type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetIndex(StockType type)
    {
        for (var i = 0; i < Stocks.Count; i++)
        {
            if (Stocks[i].Name == type)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Get the type of the given stock at index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public StockType GetType(int index)
    {
        if (index >= 0 && index < Stocks.Count)
        {
            return Stocks[index].Name;
        }

        return StockType.None;
    }
}

public enum StockType
{
    None, Alcohol, Restoration, Food, Chemicals, Technology, Fuel, Tourism, Entertainment
}