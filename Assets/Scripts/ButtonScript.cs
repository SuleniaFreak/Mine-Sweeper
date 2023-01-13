using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonScript : MonoBehaviour
{
    [Header("Button Position Settings")]//con ellas conocemos la posición del botón
    public int x;
    public int y;
    public bool IsBomb;// nos indica si en el botón hay bomba o no
  

    public void Click()
    {
        GameManager.gm.emojiFace.sprite = GameManager.gm.chooseFace[0];
        if (GetComponent<Button>().interactable == false)
        {
            return;
        }
        
        IsThereABomb();
    }

    private void IsThereABomb()
    {
        GetComponent<Button>().interactable = false;

        //si hay bomba
        if (IsBomb == true)
        {
            //GameOver
            Debug.Log("cagaste");
            GetComponent<Image>().color = Color.red;
            ChangeTextColor(8);
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "F";
            GameManager.gm.ExplodeMap();
             
        }
        else
        {//sino hay bomba
          //variable que guardará el nº de bombas alrededor del botón que hagamos click
            int num = GameManager.gm.CheckBombNumber(x, y);
            GameManager.gm.DecreaseCounter();//llamamos al metodo solo cuando pinchamos y NO es una bomba

            //sino hay bomba en ninguna casilla adyacente
            if( num == 0)
            {
                GameManager.gm.ClickAround(x, y);
                GameManager.gm.emojiFace.sprite = GameManager.gm.chooseFace[2];
            }
            else
            {
                //cojemos el hijo del botón (texto) y que lo muestres con el valor almacenado en num
                //convertido en una cadena de caracteres para que pueda mostrarse
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = num.ToString();
                ChangeTextColor(num);
            }
            
        }

    }

    private void ChangeTextColor(int num)
    {
        TextMeshProUGUI changeColor = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        switch (num)
        {
            case 1:
                changeColor.color = Color.blue;
                break;
                
            case 2:
                changeColor.color = Color.cyan;
                break;
            case 3:
                changeColor.color = Color.green;
                break;

            case 4:
                changeColor.color = Color.yellow;
                break;
            case 5:
                changeColor.color = Color.red;
                break;

            case 6:
                changeColor.color = Color.magenta;
                break;
            case 7:
                changeColor.color = Color.grey;
                break;

            case 8:
                changeColor.color = Color.black;
                break;
            


        }
    }

}
