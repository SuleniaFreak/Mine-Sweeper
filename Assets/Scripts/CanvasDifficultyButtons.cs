using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasDifficultyButtons : MonoBehaviour
{
    public GameObject gamePanel;
   
    public void Easymode()
    {
        GameManager.gm.GenerateBoard(10,5);
    
        gameObject.SetActive(false);
        gamePanel.SetActive(true);
    }
    public void Normalmode()
    {
        GameManager.gm.GenerateBoard(15,10);
        gameObject.SetActive(false);
        gamePanel.SetActive(true);
    }

    public void HardMode()
    {
        GameManager.gm.GenerateBoard(20,10);
        gameObject.SetActive(false);
        gamePanel.SetActive(true);
    }

}
