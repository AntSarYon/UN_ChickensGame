using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Food : MonoBehaviour
{
    public Slider mFoodLevelSlider;
    public Button btnReloadFood;

    //Referencia a Componentes
    private Collider2D mCollider;
    private Animator mAnimator;

    //Lista de GameObjects (Pollos) que estan chocando con la Comida
    private List<GameObject> chickensList = new List<GameObject>();

    //Velocidad con la que disminuye la comida
    [Range(1,5)] [SerializeField] private float foodDecreaseSpeed;

    //--------------------------------------------------------------------------------------

    void Awake()
    {
        //Obtenemos referencia a componentes
        mCollider = GetComponent<Collider2D>();
        mAnimator = GetComponent<Animator>();
    }

    //--------------------------------------------------------------------------------------

    void Start()
    {
        //Asignamos funcion Delegado para el evento de FoodRefill
        DayStatusManager.Instance.OnFoodRefill += OnFoodRefillDelegate;

        //Traemos los parametros del RulesManager
        foodDecreaseSpeed = GameRulesManager.instance.foodDecreaseSpeed;

        //Agregamos Listener al Boton de Recarga individual
        btnReloadFood.onClick.AddListener(ReloadSpecificFood);

        //Desactivamos el Boton (no se visualizará)
        btnReloadFood.gameObject.SetActive(false);
    }

    //--------------------------------------------------------------------------------------

    public void ReloadSpecificFood()
    {
        //Lanzamos sonido de Gasto...
        GameSoundsController.Instance.PlayResourceBoughtSound();

        //Reproducimos Animacion
        mAnimator.Play("refill");

        //Llevamnos el valor del Slider al Maximo
        mFoodLevelSlider.value = mFoodLevelSlider.maxValue;

        //Reducimos el Cash en 5
        DayStatusManager.Instance.currentCash -= 5;

        //Reproducimos la animacion de Reduccion de Cash
        FindObjectOfType<CashUIController>().OnFoodRefillDelegate();
    }

    //--------------------------------------------------------------------------------------

    private void OnFoodRefillDelegate()
    {
        //Reproducimos Animacion
        mAnimator.Play("refill");

        //Llevamnos el valor del Slider al Maximo
        mFoodLevelSlider.value = mFoodLevelSlider.maxValue;

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

    private void OnMouseOver()
    {
        //Activamos el Boton (no se visualizará)
        btnReloadFood.gameObject.SetActive(true);
        btnReloadFood.interactable = true;
    }

    private void OnMouseDown()
    {
        ReloadSpecificFood();
    }



    private void OnMouseExit()
    {
        //Desactivamos el Boton (no se visualizará)
        btnReloadFood.gameObject.SetActive(false);
    }
}
