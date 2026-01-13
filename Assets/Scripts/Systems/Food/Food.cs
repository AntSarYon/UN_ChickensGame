using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Food : Interactable
{
    [Header("Slots")]
    [SerializeField] private FoodSlots slotsManager;
    public TextMeshProUGUI txtSlotsCounter1;
    public TextMeshProUGUI txtSlotsCounter2;

    [Header("Slider de comida")]
    public Slider mFoodLevelSlider;

    [Header("Corral al que pertenece")]
    public Yard yard;

    [Header("Velocidad de variacion de comida")]
    [Range(1, 5)][SerializeField] private float foodDecreaseSpeed;

    //Lista de GameObjects (Pollos) que estan chocando con la Comida
    private List<ChickenController> eatingChickensList = new List<ChickenController>();

    public bool hasFood;
    

    //Referencia a Componentes
    private Animator mAnimator;

    //--------------------------------------------------------------------------------------

    void Awake()
    {
        //Obtenemos referencia a componentes
        mAnimator = GetComponent<Animator>();

        interactionMessage = "Llenar";

        hasFood = true;
    }

    //--------------------------------------------------------------------------------------

    void Start()
    {
        //Traemos los parametros del RulesManager
        foodDecreaseSpeed = GameRulesManager.instance.foodDecreaseSpeed;

        //Actualizamos el contador de Slots
        UpdateSlotsUICounter();
    }

    //--------------------------------------------------------------------------------------

    void Update()
    {
        //Manejamos el consumo de alimento
        ManageFoodConsumption();
    }

    // ---------------------------------------------------------------
    // FUNCION: Manejar el consumo de Alimento segun slots ocupados
    public void ManageFoodConsumption()
    {
        //Si hay al menos 1 pollito comiendo, y aun hay comida
        if (eatingChickensList.Count > 0 && hasFood)
        {
            //Reducimos el valor del Slider, en base a cuantos slots estan siendo usados por pollitos, y la velocidad definida
            mFoodLevelSlider.value -= eatingChickensList.Count * Time.deltaTime * foodDecreaseSpeed;

            if (mFoodLevelSlider.value <= 0)
            {
                //Desactivamos flag de "tiene comida"
                hasFood = false;

                // Liberamos a todos los pollitos que hayan estado comiendo
                foreach (ChickenController chicken in eatingChickensList)
                {
                    // Liberamos al Pollito del Slot - True para decirle que debe alejarse
                    chicken.Try_AbandonFoodSlot();
                }

            }
        }
    }

    // ------------------------------------------------------------
    // FUNCION: Rellenar Comedero
    public void Refill()
    {
        //Reproducimos Animacion
        mAnimator.Play("refill");

        //Llevamnos el valor del Slider al Maximo
        mFoodLevelSlider.value = mFoodLevelSlider.maxValue;

        //Activamos flag de "tiene comida"
        hasFood = true;
    }

    // ------------------------------------------------------------
    // FUNCION: Denegar Interaccion

    public void Denegate()
    {
        //Reproducimos Animacion
        mAnimator.Play("denegate");
    }

    // ------------------------------------------------------------

    public override void Interact(Transform holdingZone, InteractionController interactionController = null)
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

    // -------------------------------------------------------------------s

    public void UpdateSlotsUICounter()
    {
        //Actualizamos el contador de Slots
        txtSlotsCounter1.text = $"{slotsManager.takenSlots}/{5}";
        txtSlotsCounter2.text = $"{slotsManager.takenSlots}/{5}";
    }

    // -------------------------------------------------------------------

    public bool HasFreeSlots()
    {
        // Nos basamos en si los Slots estan Full (retorna negacion)
        return !slotsManager.bFullSlots;
    }


    // --------------------------------------------------------------------------------------

    public void RegisterEatingChicken(ChickenController chicken)
    {
        eatingChickensList.Add(chicken);

        UpdateSlotsUICounter();
    }

    public void RemoveEatingChicken(ChickenController chicken)
    {
        eatingChickensList.Remove(chicken);

        UpdateSlotsUICounter();
    }

}
