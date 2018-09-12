using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockManager : MonoBehaviour
{
    public float OperationsPerSecond = 1f;
    public float VolatilityThreshold = 500f;
    public float VolatilityLockTime = 500f;
    public float MaxVolatility = 600f;
    public float MinVolatility = 20f;
    public float VolatilityDecay = 0.98f;
    public float VolatilityIncrease = 1.05f;
    public float[] VolatilityFluctuationTimeMultipliersMin;
    public float[] VolatilityFluctuationTimeMultipliersMax;
    public float[] VolatilityFluctuationScalesMin;
    public float[] VolatilityFluctuationScalesMax;
    public float[] VolatilityFluctuationOffsetsMin;
    public float[] VolatilityFluctuationOffsetsMax;
    public float StockSellPriceMultiplier = 0.95f;
    public float StockBuyPriceMultiplier = 1.05f;
    public float StockSellPriceAbsoluteChange = -1f;
    public float StockBuyPriceAbsoluteChange = 1f;
    public float StockPriceChangeDistanceDecay = 0.25f;
    public float StockPriceChangeDistanceMinValue = 0.001f;
    public float VolatilityRandomChangeBottom = 0.995f;
    public float VolatilityRandomChangeTop = 1.005f;
    public float StockPriceRandomChangeBottom = 0.995f;
    public float StockPriceRandomChangeTop = 1.005f;

    public float StartingPanic = 0f;
    public float PanicPerLock = 10f;
    public float PanicAdditionMultiplier = 0.1f;
    public float PanicDecay = 1f;
    public float PanicMaximum = 100f;
    private float totalPanic;
    private float panicMeterFromLocks;
    private float panicMeterFromVolatility;

    //Singleton
    private static StockManager _instance;
    public static StockManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private LineRenderer graph;
    private Camera _graphCamera;

    public List<Stock> Stocks = new List<Stock>();

    private Dictionary<StockType, float[]> stockHistory = new Dictionary<StockType, float[]>();

    void Awake()
    {
        var activeInstance = _instance ? _instance : this;
        activeInstance.graph = GameObject.Find("Graph").GetComponent<LineRenderer>();
        activeInstance.graph.enabled = false;
        activeInstance._graphCamera = activeInstance.graph.transform.parent.GetComponent<Camera>();

        // Setup singleton
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        _instance = this;

        panicMeterFromVolatility = StartingPanic;
        CreateStocks();
        StartCoroutine(Tick());
    }

    private IEnumerator Tick()
    {
        while (true)
        {
            if (GameManager.Instance.isPlaying)
            {
                UpdateStocks();
                UpdateGraph();
                UpdatePanic();
            }
            yield return new WaitForSeconds(1 / OperationsPerSecond);
        }
    }

    private void UpdatePanic()
    {
        float largestVolatility = 0;
        foreach (Stock stock in Stocks)
        {
            if (stock.Volatility > largestVolatility)
            {
                largestVolatility = stock.Volatility;
            }
        }

        panicMeterFromVolatility = PanicAdditionMultiplier * (largestVolatility - MinVolatility);
        totalPanic = panicMeterFromVolatility + panicMeterFromLocks;

        if (totalPanic >= PanicMaximum)
        {
            GameManager.Instance.gameWon = true;
            Debug.Log("Game won!");
        }

        if (panicMeterFromLocks > 0)
        {
            panicMeterFromLocks -= PanicDecay;
        }
        else
        {
            panicMeterFromLocks = 0f;
        }
        if (totalPanic < 0) {
            totalPanic = 0;
        }

        Debug.Log("Panic: " + totalPanic);
    }

    public void CreateLockPanic()
    {
        panicMeterFromLocks += PanicPerLock;
    }

    private void UpdateGraph()
    {
        if (GameManager.Instance.PlayerSelectedStation != -1)
        {
            graph.enabled = true;
            float[] array = stockHistory[StockManager.Instance.GetType(GameManager.Instance.PlayerSelectedStation)];
            Vector3[] positions = new Vector3[array.Length];

            float largest = 0;
            foreach (float f in array)
            {
                if (largest < f)
                {
                    largest = f;
                }
            }

            // Get camera width so we can fill the screen
            var viewWidth = 2f * _graphCamera.orthographicSize * _graphCamera.aspect;
            for (int i = 0; i < array.Length; i++)
            {
                Vector3 pos = new Vector3();
                pos.x = ((float)i).Remap(0f, 99f, -viewWidth / 2f + 0.1f, viewWidth / 2f - 0.1f);
                pos.y = array[i].Remap(0f, largest * 2, 3.2f, 5.7f);
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
        Stock alcohol = new Stock(StockType.Alcohol, 1200, 100, 0.03f, MinVolatility);
        Stock restoration = new Stock(StockType.Restaurants, 800, 100, 0.03f, MinVolatility);
        Stock food = new Stock(StockType.Food, 1000, 100, 0.03f, MinVolatility);
        Stock chemicals = new Stock(StockType.Chemicals, 1200, 100, 0.03f, MinVolatility);
        Stock technology = new Stock(StockType.Technology, 800, 100, 0.03f, MinVolatility);
        Stock fuel = new Stock(StockType.Fuel, 1000, 100, 0.03f, MinVolatility);
        Stock tourism = new Stock(StockType.Tourism, 1200, 100, 0.03f, MinVolatility);
        Stock entertainment = new Stock(StockType.Entertainment, 800, 100, 0.03f, MinVolatility);

        CreateRelation(alcohol, restoration, 0.01f);
        CreateRelation(alcohol, entertainment, 0.01f);
        CreateRelation(alcohol, chemicals, -0.01f);
        CreateRelation(alcohol, technology, -0.01f);

        CreateRelation(restoration, alcohol, 0.01f);
        CreateRelation(restoration, food, 0.01f);
        CreateRelation(restoration, fuel, -0.01f);
        CreateRelation(restoration, tourism, -0.01f);

        CreateRelation(food, restoration, 0.01f);
        CreateRelation(food, chemicals, 0.01f);
        CreateRelation(food, fuel, -0.01f);
        CreateRelation(food, entertainment, -0.01f);

        CreateRelation(chemicals, food, 0.01f);
        CreateRelation(chemicals, technology, 0.01f);
        CreateRelation(chemicals, alcohol, -0.01f);
        CreateRelation(chemicals, tourism, -0.01f);

        CreateRelation(technology, chemicals, 0.01f);
        CreateRelation(technology, fuel, 0.01f);
        CreateRelation(technology, alcohol, -0.01f);
        CreateRelation(technology, entertainment, -0.01f);

        CreateRelation(fuel, technology, 0.01f);
        CreateRelation(fuel, tourism, 0.01f);
        CreateRelation(fuel, food, -0.01f);
        CreateRelation(fuel, restoration, -0.01f);

        CreateRelation(tourism, fuel, 0.01f);
        CreateRelation(tourism, entertainment, 0.01f);
        CreateRelation(tourism, restoration, -0.01f);
        CreateRelation(tourism, chemicals, -0.01f);

        CreateRelation(entertainment, alcohol, 0.01f);
        CreateRelation(entertainment, tourism, 0.01f);
        CreateRelation(entertainment, food, -0.01f);
        CreateRelation(entertainment, technology, -0.01f);

        this.Stocks.Add(alcohol);
        this.Stocks.Add(restoration);
        this.Stocks.Add(food);
        this.Stocks.Add(chemicals);
        this.Stocks.Add(technology);
        this.Stocks.Add(fuel);
        this.Stocks.Add(tourism);
        this.Stocks.Add(entertainment);

        this.stockHistory.Add(StockType.Alcohol, new float[100]);
        this.stockHistory.Add(StockType.Restaurants, new float[100]);
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
    None, Alcohol, Restaurants, Food, Chemicals, Technology, Fuel, Tourism, Entertainment
}