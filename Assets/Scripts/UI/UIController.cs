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

    [SerializeField] private Button btnLightSwitch;
    [SerializeField] private Button btnSleepOrder;
    [SerializeField] private Button btnMoreFoodOrder;
    [SerializeField] private Button btnMoreGasOrder;
    [SerializeField] private Button btnAddNewChicken;
    [SerializeField] private GameObject LightPanel;

    [SerializeField] private UI_FadeOut UI_fadeOut;


    [Header("Medidores")]
    [SerializeField] private Slider FoodSlider;
    [SerializeField] private Slider GasSlider;
    [SerializeField] private Slider TemperatureSlider;

    [SerializeField] private Food[] arrFoods;

    [SerializeField] private GameObject FadeOutPanel;

    //Nivel de comida actual
    private float currentFoodLevel;

    #endregion

    // ----------------------------------------------------------------------------------

    #region methods

    void Start()
    {
        //La luz inicia encendida
        TurnOnLight();

        btnLightSwitch.onClick.AddListener(ToggleLight);
        btnSleepOrder.onClick.AddListener(ToggleSleepOrder);
        btnMoreFoodOrder.onClick.AddListener(AskForFood);
        btnMoreGasOrder.onClick.AddListener(AskForGas);
        btnAddNewChicken.onClick.AddListener(AskForChicken);

        //Obtenemos Array con todos los Food
        arrFoods = FindObjectsByType<Food>(FindObjectsSortMode.None);

        //Definimos var para el Valor maximo del Slider de comida global...
        float maxValueForGeneralFoodSlider = 0;

        //Por cada Food
        foreach (Food food in arrFoods)
        {
            //Incrementamos el valor maximo
            maxValueForGeneralFoodSlider += food.mFoodLevelSlider.maxValue;
        }

        //Asignamos el Valor maximo obtenido como el maximo del Slider de comida
        FoodSlider.maxValue = maxValueForGeneralFoodSlider;

        //Asignamos que el nivel de Comida actual es el maximo
        currentFoodLevel = maxValueForGeneralFoodSlider;

        DayStatusManager.Instance.OnDayOver += OnDayOverDelegate;
    }

    // ----------------------------------------------------------------------------------
    // FUNCION DELEGADA - DIA TERMINADO

    private void OnDayOverDelegate()
    {
        // Hacemos que la UI de FadeOut muestre la animacion correspondiente
        UI_fadeOut.Play_FadeInDayOver();
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

        //Por cada Food
        foreach (Food food in arrFoods)
        {
            //Acumulamos su cantida de comida disponible
            newCurrentFoodLevel += food.mFoodLevelSlider.value;
        }

        //Actualizamos la variable de "nivel de Comida actual"
        currentFoodLevel = newCurrentFoodLevel;

        //Asignamos el valor de Comida en el Slider
        FoodSlider.value = currentFoodLevel;

        if (Input.GetKeyDown(KeyCode.M))
        {
            SceneManager.LoadScene("Menu");
        }

        //El Gas (y la temperatura) se recue constantemente...
        GasSlider.value -= 2.75f * Time.deltaTime;
        
        // Igualamos el valor de la temperatura al del Gas (por ahora)
        TemperatureSlider.value= GasSlider.value;
    }

    //----------------------------------------------------------------------------------

    public void TurnOffLight()
    {
        LightPanel.SetActive(true);
    }

    public void TurnOnLight()
    {
        LightPanel.SetActive(false);
    }

    //----------------------------------------------------------------------------------

    public void ToggleLight()
    {
        //Si el Panel esta activo (Luz apagada)
        if (LightPanel.activeSelf)
        {
            //Activamos la Luz (desactivamos el panel)
            TurnOnLight();
        }
        //Caso ontrario
        else
        {
            //Apagamos la luz (Activamos el Panel)
            TurnOffLight();
        }
    }

    //----------------------------------------------------------------------------------

    public void ToggleSleepOrder()
    {
        DayStatusManager.Instance.SleepOrderClicked();
    }

    // ----------------------------------------------------------------------------------

    public void AskForFood()
    {
        //Disparamos el Evento de "Pedir mas comida"
        DayStatusManager.Instance.TriggerEvent_FoodRefill();
    }

    public void AskForGas()
    {
        //Disparamos el Evento de "Pedir mas comida"
        DayStatusManager.Instance.TriggerEvent_GasRefill();

        //Llevamos el valor del Slider al maximo
        GasSlider.value = GasSlider.maxValue;
    }

    public void AskForChicken()
    {
        //Disparamos el Evento de "Pedir mas comida"
        DayStatusManager.Instance.TriggerEvent_GenerateNewChicken();
    }


    #endregion
}
