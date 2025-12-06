using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DayStatusManager : MonoBehaviour
{
    //Instancia de Clase
    public static DayStatusManager Instance;

    //Cash del jugador...
    [HideInInspector] public float currentCash;

    //Cantidad de Pollitos Muertos
    [HideInInspector] public int currentDeathChicken;

    // Escala de perdida de dinero...
    [HideInInspector] public float cashPassiveOutcome;

    [HideInInspector] public bool bGameOver = false;

    //Evento - Pollo vendido
    public UnityAction<float> OnChickenSold;

    //Evento - Pollo vendido
    public UnityAction OnFoodRefill;

    //Evento - Pollo vendido
    public UnityAction OnChickenDeath;

    //Evento - Pollo vendido
    public UnityAction OnGenerateNewChickenRoss;
    public UnityAction OnGenerateNewChickenCobb;

    public UnityAction OnCashIncrease;
    public UnityAction OnCashReduce;

    public UnityAction OnBuyNewToy;

    //Evento - Pollo vendido
    public UnityAction OnGameOver;

    //Evento - Dia Terminado
    public UnityAction OnDayOver;

    // -------------------------------------------------------------------

    void Awake()
    {
        //Asignamos Instancia de clase
        Instance = this;

    }

    // -------------------------------------------------------------------

    void Start()
    {
        //Traemos los datos de la Campaña actual
        currentCash = CampaignManager.Instance.campaignCurrentCash;
        currentDeathChicken = CampaignManager.Instance.campaignCurrentDeathChicken;
        cashPassiveOutcome = CampaignManager.Instance.campaignCashPassiveOutcome;
    }
    
    // --------------------------------------------------------------------

    void Update()
    {
        // Si el nivel actual NO ES el menu
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            //Reducimos el valoor del Cash constantemente
            currentCash -= cashPassiveOutcome * Time.deltaTime;

            //Revisamos si el dinero llegó a 0 para GameOver
            CheckCashAndGameOver();
        }
    }

    // ---------------------------------------------------------------------
    // Funcion disparadora de Evento: Orden de Dormir clickeada

    public void TriggerEvent_ChickenSold(float chickenValue)
    {
        float newCash = currentCash;

        //Si el Valor del Pollo es 0 (muerto)
        if (chickenValue == 0)
        {
            //El Cash solo aumento en 5
            newCash = currentCash + 5;
        }
        //Caso contrario...
        else
        {
            //El cash aumenta en base a su peso
            newCash = currentCash + (5 * chickenValue);
        }

        //Asignamos el nuevo monto
        currentCash = newCash;

        // Lllamamos a la UI de Cash para que actualice el monto
        CashUIController.instance.PlayIncreaseCash();

        //Disparamos el Evento de Galiina vendida
        //enviando el Valor de la Gallina a los Delegados
        OnChickenSold?.Invoke(chickenValue);
    }

    //-----------------------------------------------------------
    // Funcion de Evento - Food 

    public void TriggerEvent_FoodRefill()
    {
        currentCash -= 30;

        //Hacemos que el SoundsManager reproduzca sonido de Compra de Recurso
        GameSoundsController.Instance.PlayResourceBoughtSound();

        // Lllamamos a la UI de Cash para que actualice el monto
        CashUIController.instance.PlayReduceCash();

        //Disparamos el Evento de Refill de Comida
        //enviando el Valor de la Gallina a los Delegados
        OnFoodRefill?.Invoke();

        //Revisamos si el dinero llegó a 0 para GameOver
        CheckCashAndGameOver();
    }

    public void TriggerEvent_OnChickenDeath()
    {
        //Incrementamos la cantidad de Pollitos Muertos.
        currentDeathChicken++;

        //Disparamos el Evento de Gallina Muerte para alertar a los delegados 
        OnChickenDeath?.Invoke();
    }

    public void TriggerEvent_GenerateNewChickenRoss()
    {

        //Disminuimos el Dinero en 15
        currentCash -= 15;

        // Lllamamos a la UI de Cash para que actualice el monto
        CashUIController.instance.PlayReduceCash();

        //Disparamos el Evento de Gallina Muerte para alertar a los delegados 
        OnGenerateNewChickenRoss?.Invoke();

        //Reproducimos un Sonido de Spawn de Pollito
        GameSoundsController.Instance.PlayChickenSpawnSound();

        //Revisamos si el dinero llegó a 0 para GameOver
        CheckCashAndGameOver();
    }

    public void TriggerEvent_GenerateNewChickenCobb()
    {

        //Disminuimos el Dinero en 15
        currentCash -= 15;

        // Lllamamos a la UI de Cash para que actualice el monto
        CashUIController.instance.PlayReduceCash();

        //Disparamos el Evento de Gallina Muerte para alertar a los delegados 
        OnGenerateNewChickenCobb?.Invoke();

        //Reproducimos un Sonido de Spawn de Pollito
        GameSoundsController.Instance.PlayChickenSpawnSound();

        //Revisamos si el dinero llegó a 0 para GameOver
        CheckCashAndGameOver();
    }

    // -------------------------------------------
    // Disparador de Evento: Juguete comprado 
    public void TriggerEvent_ToyBought()
    {
        //Disminuioms el Valor del Cash...
        currentCash -= 10;

        //Hacemos que el SoundsManager reproduzca sonido de Compra de Recurso
        GameSoundsController.Instance.PlayResourceBoughtSound();

        // Lllamamos a la UI de Cash para que actualice el monto
        CashUIController.instance.PlayReduceCash();

        // Invocamoms a los Delegados (si los hubiera)
        OnBuyNewToy?.Invoke();
    }

    // -------------------------------------------------------------------

    public void TriggerEvent_GameOver()
    {
        //Reproducimos un Sonido de Spawn de Pollito
        GameSoundsController.Instance.PlayChickenSpawnSound();
    }

    private void CheckCashAndGameOver()
    {
        //Si el dinero llegó a 0...
        if (currentCash <= 0)
        {
            //Activamos Flag de GameOver
            bGameOver = true;
        }
    }
}
