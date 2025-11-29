using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region Props

    public static UIController Instance;

    //Boton de agregar nuevo Pollo
    [SerializeField] private Button btnAddNewChickenRoss;
    [SerializeField] private Button btnAddNewChickenCobb;
    [SerializeField] private Button btnBuyToy;

    // Panel de FadeIn/ Out
    [SerializeField] private UI_FadeOut UI_fadeOut;
    [SerializeField] private GameObject FadeOutPanel;

    // Barra de comida (unico, adaptable para cada corral)
    [Header("Medidores")]
    [SerializeField] private Slider FoodSlider;

    // Corral Objetivo actual
    public Yard currentTargetYard;
    [SerializeField] private TextMeshProUGUI txtYardTitle;

    [SerializeField] private UI_FoodPanel UI_foodPanel;

    #endregion

    // ----------------------------------------------------------------------------------

    #region methods

    void Awake()
    {
        Instance = this;
    }

    // ---------------------------------------------------------------------------------

    void Start()
    {
        //Obtenemos referencia al Corral actual
        currentTargetYard = YardsManager.instance.currentYard;
        txtYardTitle.text = $"Comida en Corral: {currentTargetYard.yardName}";

        // Agregamos Listener de Agregar nuevo pollito
        btnAddNewChickenRoss.onClick.AddListener(AskForChickenRoss);
        btnAddNewChickenCobb.onClick.AddListener(AskForChickenCobb);
        btnBuyToy.onClick.AddListener(AskForToy);

        // Desactivamos el Panel de Comida
        //UI_foodPanel.gameObject.SetActive(false);

        //Asignamos como Valor maximo del Sliderl el maximo del Corral en turno (podria variar)
        FoodSlider.maxValue = currentTargetYard.totalFoodMaxValue;

        //Asignamos como valor del Slider el nivel de Comida actual en el corral
        FoodSlider.value = currentTargetYard.currentTotalFoodLevel;

        YardsManager.instance.OnCurrentYardChanged += OnCurrentYardChangedDelegate;
    }

    // ---------------------------------------------------------------------------------

    private void OnCurrentYardChangedDelegate(Yard newCurrentYard)
    {
        // Actualizamos referencia al Corral actual
        currentTargetYard = newCurrentYard;

        txtYardTitle.text = $"Comida en Corral: {currentTargetYard.yardName}";

        // Actualizamos el valor maximo dle slider
        FoodSlider.maxValue = currentTargetYard.totalFoodMaxValue;
    }

    // -----------------------------------------------------------------------------------

    public void DisplayPanel_with_FoodSelected(Food selectedFood)
    {
        // Enviamos la referencia dle Food al Panel de Food, pues sera quien lo utilice.
        UI_foodPanel.foodReference = selectedFood;

        // Activamos el Panel de Comida...
        UI_foodPanel.ShowPanel();
    }

    // ----------------------------------------------------------------------------------

    void Update()
    {
        //Si ya hay GameOver...
        if (DayStatusManager.Instance.bGameOver)
        {
            UI_fadeOut.Play_FadeInGameOver();
        }

        // Si se tiene una referencia a un Corral actual...
        if (currentTargetYard != null)
        {
            //Asignamos como valor del Slider el nivel de Comida actual en el corral
            FoodSlider.value = currentTargetYard.currentTotalFoodLevel;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            SceneManager.LoadScene("Menu");
        }

    }

    // ----------------------------------------------------------------------------------

    public void AskForFood()
    {
        //Disparamos el Evento de "Pedir mas comida"
        DayStatusManager.Instance.TriggerEvent_FoodRefill();
    }

    // ----------------------------------------------------------------------------------

    public void AskForChickenRoss()
    {
        //Disparamos el Evento de "Pedir mas comida"
        DayStatusManager.Instance.TriggerEvent_GenerateNewChickenRoss();
    }

    public void AskForChickenCobb()
    {
        //Disparamos el Evento de "Pedir mas comida"
        DayStatusManager.Instance.TriggerEvent_GenerateNewChickenCobb();
    }

    // -------------------------------------------------------------------------

    public void AskForToy()
    {
        //Disparamos el Evento de "Pedir mas comida"
        DayStatusManager.Instance.TriggerEvent_ToyBought();
    }


    #endregion
}
