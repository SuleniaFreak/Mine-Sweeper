using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;//libreria que activamos para usar el método DialButton
using TMPro;
using UnityEngine.SceneManagement;//libreria para poder reiniciar la escena a traves del emoji

public class GameManager : MonoBehaviour
{
    public static GameManager gm; //singleton (puede ser usado en otros scripts)
    public GameObject gameButton;
    public GameObject gamePanel;
    public GameObject winPanel;

    [Header("Mouse Settings")]
    PointerEventData myPointer; //guarda el click del ratón en la pantalla
    public GraphicRaycaster myRaycaster; // rastrea el clicl del ratón en el canvas

    [Header("GamePanel Settings")]
    public int width;
    public int height;

    [Header("Emoji Settings")]
    public Image emojiFace;
    public Sprite[] chooseFace;

    int bombsAmount;//contabiliza el nº de bombas que habrá en la matriz
    int counter; //variable que contará el nº de casillas interactuables

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
    //creamos los botones poniendo como límite el ancho y el alto del gamepanel 
    private void ButtonGenerator()
    {
        //cojemos el componente del gridlayoutgroup para que se adapte al ancho establecido en width
        gamePanel.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gamePanel.GetComponent<GridLayoutGroup>().constraintCount = width;

        for(int i= 0; i < width * height; i++)
        {
            //con estas variables determinamos la posición de los botones,
            //mediante la división de i por la altura de la matriz
            int x = i / height; //coeficiente de la división
            int y = i % height;//resto de la división

            //le decimos a la matriz qué gameObject crear (botones) y donde colocarlo (en el transform del gamePanel)
            map[x,y] = Instantiate(gameButton, gamePanel.transform).GetComponent<ButtonScript>();
            //mediante el getcomponent del button script le decimos a las variables x e y
            //cuales son sus valores inicianizándolas (creando las coordenadas de la matriz)
            map[x, y].x = x;
            map[x, y].y = y;
        }
    }

    //método que colocará las bombas en la matriz
    public void GenerateBombs()
    {
        
        //bucle que dará vueltas mientras i sea menor que el valor guardado en bombsAmount
        //y de manera aleatoria colocará bombas en las casillas de la matriz (activando el bool)
        for (int i = 0; i < bombsAmount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            //condicional que evita que meta 2 bombas en la mísma casilla
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

    //método para clickar todas las casillas adyacentes que no tengan bombas de golpe
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

    //método que checkea si hay bombas alrededor de una casilla y devuelve el número de las que hay
    public int CheckBombNumber(int x, int y)
    {
        int result = 0;
        //condicional que comprueba si estoy al límite de la izq
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
            //si hay bomba a la izq- arriba (y que no se pasa el límite de altura)
            if ( y < height - 1 && map[x - 1, y + 1].IsBomb)
            {
                result++;
            }
        }
        //condicional que comprueba el límite de la derecha
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
        //(solo tenemos 1 escena si hay más hay que tener cuidado)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //método que libera el mapa mostrando todas las casillas recorriendo la matriz
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

    //método para marcar las casillas en las que puede haber bombas (con click derecho)
    private void DialButton()
    {
        //condicional para saber donde estamos clickando con click derecho
        //manera alternativa de indicar si estamos pulsando el botón derecho del ratón
        if (Input.GetButtonDown("Fire2"))
        {
            // se crea una variable que guarda la info del clickado derecho en el canvas
            myPointer = new PointerEventData(EventSystem.current);
            //lista donde se almacena toda la info que devuelve al pulsar el click derecho
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            //se llama de nuevo a la variable para que guarde la posición exacta del ratón
            myPointer.position = Input.mousePosition;
            //muestra toda la info almacenada en la lista
            myRaycaster.Raycast(myPointer, raycastResults);

            //si hemos pulsado click derecho
            if(raycastResults.Count > 0)
            {
                //bucle que recorre la info almacenada en la lista raycastResults
                for(int i = 0; i < raycastResults.Count; i++)
                {
                    //variable que guarda el botón pulsado
                    Button buttonResult = raycastResults[i].gameObject.GetComponent<Button>();

                    //si lo que he clickado es un botón:
                    if (buttonResult)
                    {
                        //variable que guarda el texto hijo del botón que hemos sacado
                        //clickando con el botón derecho que está almacenado en la lista 
                        TextMeshProUGUI changeText = raycastResults[i].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                        //cambiamos la imagen del botón
                        Image changeImage = raycastResults[i].gameObject.GetComponent<Image>();
                        //si se puede interactuar con el botón, cambiamos texto, color, y lo desactivamos
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


    //método que genera el nº de bombas
    private void BombsQuantitySettings()
    {
        bombsAmount = Random.Range(10, (width * height) / 2);

        //condiciones especiales para tableros pequeños
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
