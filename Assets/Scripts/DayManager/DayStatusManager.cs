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

    //FLAG - Orden para dormir
    [HideInInspector] public bool orderedToSleep;

    //EVENTO - Orden de Dormir Clickeada...
    public UnityAction<bool> OnSleepOrderClicked;

    //Evento - Pollo vendido
    public UnityAction<float> OnChickenSold;

    //Evento - Pollo vendido
    public UnityAction OnFoodRefill;

    //Evento - Pollo vendido
    public UnityAction OnGasRefill;

    //Evento - Pollo vendido
    public UnityAction OnChickenDeath;

    //Evento - Pollo vendido
    public UnityAction OnGenerateNewChicken;

    //Evento - Pollo vendido
    public UnityAction OnGameOver;

    //Evento - Dia Terminado
    public UnityAction OnDayOver;

    // -------------------------------------------------------------------

    void Awake()
    {
        //Asignamos Instancia de clase
        Instance = this;

        //La orden para Dormir inicia desactivada
        orderedToSleep = false;

    }

    // -------------------------------------------------------------------

    void Start()
    {
        //Traemos los datos de la Campa�a actual
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

            //Revisamos si el dinero lleg� a 0 para GameOver
            CheckCashAndGameOver();
        }
    }

    // ---------------------------------------------------------------------
    // Funcion disparadora de Evento: Orden de Dormir clickeada

    public void SleepOrderClicked()
    {
        //Invertimos el valor de la Orden
        orderedToSleep = !orderedToSleep;

        //Invocamos a los delegados,  indicando el estado de la orden
        OnSleepOrderClicked?.Invoke(orderedToSleep);
    }

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

        //Asignamos el nuevo peso
        currentCash = newCash;

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

        //Disparamos el Evento de Refill de Comida
        //enviando el Valor de la Gallina a los Delegados
        OnFoodRefill?.Invoke();

        //Revisamos si el dinero lleg� a 0 para GameOver
        CheckCashAndGameOver();
    }

    public void TriggerEvent_GasRefill()
    {
        currentCash -= 50;

        //Hacemos que el SoundsManager reproduzca sonido de Compra de Recurso
        GameSoundsController.Instance.PlayResourceBoughtSound();

        //Disparamos el Evento de Refill de Comida
        //enviando el Valor de la Gallina a los Delegados
        OnGasRefill?.Invoke();

        //Revisamos si el dinero lleg� a 0 para GameOver
        CheckCashAndGameOver();
    }

    public void TriggerEvent_OnChickenDeath()
    {
        //Incrementamos la cantidad de Pollitos Muertos.
        currentDeathChicken++;

        //Disparamos el Evento de Gallina Muerte para alertar a los delegados 
        OnChickenDeath?.Invoke();
    }

    public void TriggerEvent_GenerateNewChicken()
    {

        //Disminuimos el Dinero en 15
        currentCash -= 15;

        //Disparamos el Evento de Gallina Muerte para alertar a los delegados 
        OnGenerateNewChicken?.Invoke();

        //Reproducimos un Sonido de Spawn de Pollito
        GameSoundsController.Instance.PlayChickenSpawnSound();

        //Revisamos si el dinero lleg� a 0 para GameOver
        CheckCashAndGameOver();
    }

    public void TriggerEvent_GameOver()
    {

        //Reproducimos un Sonido de Spawn de Pollito
        GameSoundsController.Instance.PlayChickenSpawnSound();
    }

    public void TriggerEvent_DayOver()
    {
        // Disparamos evento de Dia Terminado
        OnDayOver?.Invoke();

        //Pasamos al siguiente dia
        CampaignManager.Instance.IncreaseDay();
    }

    private void CheckCashAndGameOver()
    {
        //Si el dinero lleg� a 0...
        if (currentCash <= 0)
        {
            //Activamos Flag de GameOver
            bGameOver = true;
        }
    }
}
