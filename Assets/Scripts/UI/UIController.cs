using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region Props

    //Boton de agregar nuevo Pollo
    [SerializeField] private Button btnAddNewChicken;

    // Panel de FadeIn/ Out
    [SerializeField] private UI_FadeOut UI_fadeOut;
    [SerializeField] private GameObject FadeOutPanel;

    // Barra de comida (unico, adaptable para cada corral)
    [Header("Medidores")]
    [SerializeField] private Slider FoodSlider;

    

    // Corral Objetivo actual
    private Yard currentTargetYard;

    //Nivel de comida del Corral actual
    private float currentYardFoodLevel;

    #endregion

    // ----------------------------------------------------------------------------------

    #region methods

    void Start()
    {
        // Agregamos Listener de Agregar nuevo pollito
        btnAddNewChicken.onClick.AddListener(AskForChicken);

        //Asignamos como Valor maximo del Sliderl el maximo del Corral en turno (podria variar)
        FoodSlider.maxValue = currentTargetYard.totalFoodMaxValue;

        //Asignamos como valor del Slider el nivel de Comida actual en el corral
        FoodSlider.value = currentTargetYard.currentTotalFoodLevel;
    }

    // ----------------------------------------------------------------------------------

    void Update()
    {
        //Si ya hay GameOver...
        if (DayStatusManager.Instance.bGameOver)
        {
            UI_fadeOut.Play_FadeInGameOver();
        }

        //Var temporal para el nuevo total de comida actual
        float newCurrentFoodLevel = 0;
        /*
        //Por cada Food
        foreach (Food food in arrFoods)
        {
            //Acumulamos su cantida de comida disponible
            newCurrentFoodLevel += food.mFoodLevelSlider.value;
        }

        //Actualizamos la variable de "nivel de Comida actual"
        currentFoodLevel = newCurrentFoodLevel;

        //Asignamos el valor de Comida en el Slider
        FoodSlider.value = currentFoodLevel;*/

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

    public void AskForChicken()
    {
        //Disparamos el Evento de "Pedir mas comida"
        DayStatusManager.Instance.TriggerEvent_GenerateNewChicken();
    }


    #endregion
}
