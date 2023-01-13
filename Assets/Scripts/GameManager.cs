using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;//libreria que activamos para usar el m�todo DialButton
using TMPro;
using UnityEngine.SceneManagement;//libreria para poder reiniciar la escena a traves del emoji

public class GameManager : MonoBehaviour
{
    public static GameManager gm; //singleton (puede ser usado en otros scripts)
    public GameObject gameButton;
    public GameObject gamePanel;
    public GameObject winPanel;

    [Header("Mouse Settings")]
    PointerEventData myPointer; //guarda el click del rat�n en la pantalla
    public GraphicRaycaster myRaycaster; // rastrea el clicl del rat�n en el canvas

    [Header("GamePanel Settings")]
    public int width;
    public int height;

    [Header("Emoji Settings")]
    public Image emojiFace;
    public Sprite[] chooseFace;

    int bombsAmount;//contabiliza el n� de bombas que habr� en la matriz
    int counter; //variable que contar� el n� de casillas interactuables

    bool die;//checkear si hemos muerto o no

    //matriz
    ButtonScript[,] map;
    void Start()
    {
        BombsQuantitySettings();
        gm = this;
        map = new ButtonScript[width,height];//definimos el ancho y el alto de la matriz
        die = false;
        ButtonGenerator();
        GenerateBombs();
        counter = (width * height) - bombsAmount;
    }

    void Update()
    {
        DialButton();
    }
    //creamos los botones poniendo como l�mite el ancho y el alto del gamepanel 
    private void ButtonGenerator()
    {
        //cojemos el componente del gridlayoutgroup para que se adapte al ancho establecido en width
        gamePanel.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gamePanel.GetComponent<GridLayoutGroup>().constraintCount = width;

        for(int i= 0; i < width * height; i++)
        {
            //con estas variables determinamos la posici�n de los botones,
            //mediante la divisi�n de i por la altura de la matriz
            int x = i / height; //coeficiente de la divisi�n
            int y = i % height;//resto de la divisi�n

            //le decimos a la matriz qu� gameObject crear (botones) y donde colocarlo (en el transform del gamePanel)
            map[x,y] = Instantiate(gameButton, gamePanel.transform).GetComponent<ButtonScript>();
            //mediante el getcomponent del button script le decimos a las variables x e y
            //cuales son sus valores inicianiz�ndolas (creando las coordenadas de la matriz)
            map[x, y].x = x;
            map[x, y].y = y;
        }
    }

    //m�todo que colocar� las bombas en la matriz
    public void GenerateBombs()
    {
        
        //bucle que dar� vueltas mientras i sea menor que el valor guardado en bombsAmount
        //y de manera aleatoria colocar� bombas en las casillas de la matriz (activando el bool)
        for (int i = 0; i < bombsAmount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            //condicional que evita que meta 2 bombas en la m�sma casilla
            if(map[x, y].IsBomb == true)
            {
                i--;
            }
            else
            {
                map[x, y].IsBomb = true;
            }
        }
    }

    //m�todo para clickar todas las casillas adyacentes que no tengan bombas de golpe
    public void ClickAround(int x, int y)
    {
        //botones de la izq
        if(x > 0)
        {
            //izquierda
            map[x - 1, y].Click();
            //izq-abajo
            if(y > 0)
            {
                map[x - 1, y - 1].Click();
            }
            //izq-arriba
            if(y < height - 1)
            {
                map[x - 1, y + 1].Click();
            }

        }
        //botones de la derecha
        if(x < width - 1)
        {
            //derecha
            map[x + 1, y].Click();
            //der-abajo
            if(y > 0)
            {
                map[x + 1, y - 1].Click();
            }
            //der-arriba
            if(y < height - 1)
            {
                map[x + 1, y + 1].Click();
            }

        }

        //centro abajo
        if(y > 0)
        {
            map[x, y - 1].Click();
        }
        //centro arriba
        if(y < height - 1)
        {
            map[x, y + 1].Click();
        }


    }

    //m�todo que checkea si hay bombas alrededor de una casilla y devuelve el n�mero de las que hay
    public int CheckBombNumber(int x, int y)
    {
        int result = 0;
        //condicional que comprueba si estoy al l�mite de la izq
        if(x > 0)
        {
            //si hay bomba a la izq
            if (map[x - 1, y].IsBomb)
            {
                result++;
            }
             //si hay bomba a la izq- abajo
            if (y > 0 && map[x - 1, y - 1].IsBomb)
            {
                result++;
            }
            //si hay bomba a la izq- arriba (y que no se pasa el l�mite de altura)
            if ( y < height - 1 && map[x - 1, y + 1].IsBomb)
            {
                result++;
            }
        }
        //condicional que comprueba el l�mite de la derecha
        if(x < width - 1)
        {
            //si hay bomba a la der
            if (map[x + 1, y].IsBomb)
            {
                result++;
            }
             //si hay bomba a la der- abajo
            if (y > 0 && map[x + 1, y - 1].IsBomb)
            {
                result++;
            }
            //si hay bomba a la der - arriba
            if (y < height -1 && map[x + 1, y + 1].IsBomb)
            {
                result++;
            }

        }

        //condicional si hay bomba justo arriba
        if(y < height - 1 && map[x, y + 1].IsBomb)
        {
            result++;
        }

        //condicional si hay bomba justo abajo 
        if (y > 0 && map[x, y - 1].IsBomb)
        {
            result++;
        }

        return result;

    }

    public void Replay()
    {
        //con esto recargamos de nuevo la escena usando buildindex
        //(solo tenemos 1 escena si hay m�s hay que tener cuidado)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //m�todo que libera el mapa mostrando todas las casillas recorriendo la matriz
    public void ExplodeMap()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                die = true;// has fenecido
                map[i, j].Click();//clickea todo el mapa mostrando las casillas
            }
        }
        emojiFace.sprite = chooseFace[1];
    }

    public void DecreaseCounter()
    {
        counter--;
        Debug.Log(counter);

        if(die == false && counter == 0)
        {
            winPanel.SetActive(true);
        }
    }

    //m�todo para marcar las casillas en las que puede haber bombas (con click derecho)
    private void DialButton()
    {
        //condicional para saber donde estamos clickando con click derecho
        //manera alternativa de indicar si estamos pulsando el bot�n derecho del rat�n
        if (Input.GetButtonDown("Fire2"))
        {
            // se crea una variable que guarda la info del clickado derecho en el canvas
            myPointer = new PointerEventData(EventSystem.current);
            //lista donde se almacena toda la info que devuelve al pulsar el click derecho
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            //se llama de nuevo a la variable para que guarde la posici�n exacta del rat�n
            myPointer.position = Input.mousePosition;
            //muestra toda la info almacenada en la lista
            myRaycaster.Raycast(myPointer, raycastResults);

            //si hemos pulsado click derecho
            if(raycastResults.Count > 0)
            {
                //bucle que recorre la info almacenada en la lista raycastResults
                for(int i = 0; i < raycastResults.Count; i++)
                {
                    //variable que guarda el bot�n pulsado
                    Button buttonResult = raycastResults[i].gameObject.GetComponent<Button>();

                    //si lo que he clickado es un bot�n:
                    if (buttonResult)
                    {
                        //variable que guarda el texto hijo del bot�n que hemos sacado
                        //clickando con el bot�n derecho que est� almacenado en la lista 
                        TextMeshProUGUI changeText = raycastResults[i].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                        //cambiamos la imagen del bot�n
                        Image changeImage = raycastResults[i].gameObject.GetComponent<Image>();
                        //si se puede interactuar con el bot�n, cambiamos texto, color, y lo desactivamos
                        if (buttonResult.interactable)
                        {
                            changeText.text = "?";
                            changeImage.color = Color.yellow;
                            buttonResult.interactable = false;
                            emojiFace.sprite = chooseFace[3];
                        }
                        else if (changeText.text.Equals("?"))
                        {
                            changeText.text = "";
                            changeImage.color = Color.white;
                            buttonResult.interactable = true;
                            emojiFace.sprite = chooseFace[0];
                        }


                    }
                // debug que da el nombre de todo lo que pulsemos recorriendo el bucle (con click derecho)
                Debug.Log(raycastResults[i].gameObject.name);
                }
                
            }
        }
    }


    //m�todo que genera el n� de bombas
    private void BombsQuantitySettings()
    {
        bombsAmount = Random.Range(10, (width * height) / 2);

        //condiciones especiales para tableros peque�os
        if (width * height <= 10)
        {
            bombsAmount = 1;
        }
        else if (width * height > 10 && width * height < 30)
        {
            bombsAmount = 5;
        }
    }

    
}
