using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Food : MonoBehaviour
{
    public Slider mFoodLevelSlider;

    //Referencia a Componentes
    private Collider2D mCollider;

    //Lista de GameObjects (Pollos) que estan chocando con la Comida
    private List<GameObject> chickensList = new List<GameObject>();

    //Velocidad con la que disminuye la comida
    [Range(1,5)] [SerializeField] private float foodDecreaseSpeed;

    //--------------------------------------------------------------------------------------

    void Awake()
    {
        mCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        //Traemos los parametros del RulesManager
        foodDecreaseSpeed = GameRulesManager.instance.foodDecreaseSpeed;
    }

    //--------------------------------------------------------------------------------------

    void Update()
    {
        //Si hay al menos 1 pollo consumiendo comida
        if (chickensList.Count > 0)
        {
            //Reducimos el valor del Slider, en base a cuantos pollos hay, y a la velocidad definida
            mFoodLevelSlider.value -= chickensList.Count * Time.deltaTime * foodDecreaseSpeed;
        }
    }

    //--------------------------------------------------------------------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Si el objeto con el que ha empezado la colision es un Pollo...
        if (collision.gameObject.CompareTag("Chicken"))
        {
            //Agregamos el Pollo a la Lista
            chickensList.Add(collision.gameObject);
        }
    }

    //--------------------------------------------------------------------------------------

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Si el objeto con el que deja de colisionar es un Pollo...
        if (collision.gameObject.CompareTag("Chicken"))
        {
            //Si el pollo estaba en la lista...
            if (chickensList.Contains(collision.gameObject))
            {
                //Lo quitamos de la Lista
                chickensList.Remove(collision.gameObject);
            }
        }
    }
}
