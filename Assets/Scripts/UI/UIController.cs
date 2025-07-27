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
    [SerializeField] private GameObject LightPanel;

    [Header("Medidores")]
    [SerializeField] private Slider FoodSlider;
    [SerializeField] private Slider GasSlider;

    [SerializeField] private Food[] arrFoods;

    //Nivel de comida actual
    private float currentFoodLevel;

    [SerializeField] private TextMeshProUGUI txtMoreFood;

    #endregion

    //----------------------------------------------------------------------------------

    #region methods

    void Start()
    {
        //La luz inicia encendida
        TurnOnLight();

        btnLightSwitch.onClick.AddListener(ToggleLight);
        btnSleepOrder.onClick.AddListener(ToggleSleepOrder);
        btnMoreFoodOrder.onClick.AddListener(AskForFood);

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

        //Desactivamos el mensaje de "Comida solicitada"
        txtMoreFood.gameObject.SetActive(false);

    }

    void Update()
    {
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
        DayStatusManager.instance.SleepOrderClicked();
    }

    public void AskForFood()
    {
        //Hacemos que el boton no sea interactuable...
        btnMoreFoodOrder.interactable = false;

        //Sctivamos el Mensaje...
        txtMoreFood.gameObject.SetActive(true);

        //Desactivamos el mensaje tras 4 segundos
        Invoke(nameof(HideFoodMessage), 4);
    }

    public void HideFoodMessage()
    {
        txtMoreFood.gameObject.SetActive(false);
    }

    #endregion
}
