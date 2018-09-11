using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public Image StockGraph;
    public Text StockNameText;

    void Awake()
    {

    }

    public void PreviousStation()
    {
        GameManager.Instance.SetPlayerTarget(-1);
    }

    public void NextStation()
    {
        GameManager.Instance.SetPlayerTarget(1);
    }
}
