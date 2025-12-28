using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Food : Interactable
{

    [Header("Slider de comida")]
    public Slider mFoodLevelSlider;

    [Header("Corral al que pertenece")]
    public Yard yard;

    [Header("Velocidad de variacion de comida")]
    [Range(1, 5)][SerializeField] private float foodDecreaseSpeed;

    //Lista de GameObjects (Pollos) que estan chocando con la Comida
    private List<GameObject> chickensList = new List<GameObject>();

    [Header("Gestor de Slots")]
    [SerializeField] private FoodSlots slotsManager;

    //Referencia a Componentes
    private Animator mAnimator;

    //--------------------------------------------------------------------------------------

    void Awake()
    {
        //Obtenemos referencia a componentes
        mAnimator = GetComponent<Animator>();

        interactionMessage = "Llenar";

        
    }

    //--------------------------------------------------------------------------------------

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
            /*// Inicializamos contador de Pollitos que SI ESTAN COMIENDO
            // (Puede haber pollitos que estan chocando, pero que no estan comiendo)
            int eatingChicks = 0;

            //Por cada pollito que este chocando
            foreach (GameObject chick in chickensList)
            {
                // Si su flag de "Comiendo" esta activa
                if (chick.GetComponent<ChickenController>().bIsEating)
                {
                    // Se incrementa el contador de pollitos comiendo
                    eatingChicks++;
                }
            }*/

            //Reducimos el valor del Slider, en base a cuantos pollitos estan comiendo, y a la velocidad definida
            //mFoodLevelSlider.value -= eatingChicks * Time.deltaTime * foodDecreaseSpeed;
        }
    }

    // ------------------------------------------------------------

    public void Refill()
    {
        //Reproducimos Animacion
        mAnimator.Play("refill");

        //Llevamnos el valor del Slider al Maximo
        mFoodLevelSlider.value = mFoodLevelSlider.maxValue;
    }

    public void Denegate()
    {
        //Reproducimos Animacion
        mAnimator.Play("denegate");
    }

    // ------------------------------------------------------------

    public override void Interact(Transform holdingZone)
    {
        if (holdingZone.childCount > 0)
        {
            // Si la zona de Agarre tiene una FoodBag como hijo...
            if (holdingZone.GetChild(0).CompareTag("FoodBag"))
            {
                // Obtenemos referencia al Objeto sujetado
                FoodBag foodBag = holdingZone.GetChild(0).GetComponent<FoodBag>();

                //Hacemos un Refill
                Refill();

                //Dropeamos la bolsa
                foodBag.Drop();

                // La desactivamos para que vuelva al Pool
                foodBag.gameObject.SetActive(false);
            }
            else Denegate();
        }

        // Si no tiene nada, o el objeto no es una bolsa de comida
        else Denegate();

    }

    // -------------------------------------------------------------------

    public bool HasFreeSlots()
    {
        // Nos basamos en si los Slots estan Full (retorna negacion)
        return !slotsManager.bFullSlots;
    }

    //--------------------------------------------------------------------------------------

    public void ReleaseChicken(ChickenController chicken)
    {
        //Llamamos a la funcion de Liberacion del Gestor de Sots
        slotsManager.ReleaseChicken(chicken);
    }

}
