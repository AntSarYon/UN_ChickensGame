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

    [Header("Botones de Spawn")]
    [SerializeField] private Button btnAddNewChickenRoss;
    [SerializeField] private Button btnAddNewChickenCobb;

    [Header("Controlador de Fade in / out")]
    [SerializeField] private UI_FadeOut UI_fadeOut;

    // Barra de comida (unico, adaptable para cada corral)
    [Header("Medidor de comida")]
    [SerializeField] private Slider FoodSlider;

    [Header("Referencia a Corral")]
    public Yard currentTargetYard;
    [SerializeField] private TextMeshProUGUI txtFoodBar;

    [Header("Barra de Stamina")]
    [SerializeField] private Slider staminaSlider;

    [Header("Textos de interaccion")]
    [SerializeField] private GameObject interactionMessage;
    private string interactionText;

    [Header("Indicador de Temperatura")]
    [SerializeField] private GameObject temperatureUI;

    [Header("Controlador de Delivery Message")]
    [SerializeField] private UI_DeliverMessage UI_AlertMessage;

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
        currentTargetYard = Yard.Instance;
        txtFoodBar.text = $"Comida en Corral:";

        // Agregamos Listener de Agregar nuevo pollito
        btnAddNewChickenRoss.onClick.AddListener(AskForChickenRoss);
        btnAddNewChickenCobb.onClick.AddListener(AskForChickenCobb);

        //Asignamos como Valor maximo del Sliderl el maximo del Corral en turno (podria variar)
        FoodSlider.maxValue = currentTargetYard.totalFoodMaxValue;

        //Asignamos como valor del Slider el nivel de Comida actual en el corral
        FoodSlider.value = currentTargetYard.currentTotalFoodLevel;

        //Ocultamos el mensaje de iteraccion
        HideInteractionMessage();
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

    // ---------------------------------------------------------------------------
    // Funcion: Establece el valor normalizado (0..1) de la barra de stamina

    public void UpdateStamina(float normalized)
    {
        // Si por orden de ejecución el slider aún no está asignado, intentamos encontrarlo ahora
        if (staminaSlider == null)
        {
            staminaSlider = GetComponentInChildren<Slider>();
            if (staminaSlider == null)
            {
                staminaSlider = FindObjectOfType<Slider>();
            }

            if (staminaSlider != null)
            {
                staminaSlider.minValue = 0f;
                staminaSlider.maxValue = 1f;
                staminaSlider.gameObject.SetActive(true);
            }
        }

        if (staminaSlider != null)
        {
            staminaSlider.value = Mathf.Clamp01(normalized);
        }
    }

    // ----------------------------------------------------------------------------

    public void SetInteractionMessage(string interaction)
    {
        //Seteamos el mensaje de interaccion
        interactionText = $"[E] {interaction}";

        //Asignamos el Mensaje de Interaccion a los Txt (norma y sombreado)
        TextMeshProUGUI[] arrTxts = interactionMessage.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI txtComp in arrTxts)
        {
            txtComp.text = interactionText;
        }
    }

    // ----------------------------------------------------------

    public void ShowInteractionMessage()
    {
        //Activamos el mensaje de Iteraccion
        interactionMessage.SetActive(true);
    }

    public void HideInteractionMessage()
    {
        //Activamos el mensaje de Iteraccion
        interactionMessage.SetActive(false);
    }

    // ----------------------------------------------------------

    public void ShowAlertMessage()
    {
        UI_AlertMessage.ShowMessage();
    }


    #endregion
}
