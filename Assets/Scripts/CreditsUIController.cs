using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsUIController : MonoBehaviour
{
    public void BackToMenu()
    {
        if (GameManager.Instance)
        {
            GameObject.Destroy(GameManager.Instance.gameObject);
        }

        if (StockManager.Instance)
        {
            GameObject.Destroy(StockManager.Instance.gameObject);
        }

        GameManager.Instance = null;
        StockManager.Instance = null;

        SceneManager.LoadScene("TitleScene");
    }
}
