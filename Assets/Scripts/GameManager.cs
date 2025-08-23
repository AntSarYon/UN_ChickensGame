using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Instancia de Clase
    public static GameManager Instance;

    //Cash del jugador...
    [HideInInspector] public float currentCash;

    // Escala de perdida de dinero...
    private float cashPassiveOutcome;

    //Cantidad de Pollitos Muertos
    [HideInInspector] public int currentDeathChicken;

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

    [HideInInspector] public bool bGameOver = false;

    //---------------------------------------------------
    void Awake()
    {
        /*
        //Controlamos que esta sea la unica instancia de la Clase GameManager
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            
            DontDestroyOnLoad(this.gameObject);
        }
        */
        Instance = this;

        //El dinero empieza en 250$
        currentCash = 300.00f;

        //Empezamos con 0 pollitos muertos
        currentDeathChicken = 0;

        //Iniciamos el valor de Salida Pasiva del Cash
        cashPassiveOutcome = 1.5f;
    }

    void Start()
    {
        
    }


    //-----------------------------------------------------------
    // Funcion de Evento - ChickenSold

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

        //Revisamos si el dinero llegó a 0 para GameOver
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

    public void TriggerEvent_GenerateNewChicken()
    {

        //Disminuimos el Dinero en 15
        currentCash -= 15;

        //Disparamos el Evento de Gallina Muerte para alertar a los delegados 
        OnGenerateNewChicken?.Invoke();

        //Reproducimos un Sonido de Spawn de Pollito
        GameSoundsController.Instance.PlayChickenSpawnSound();

        //Revisamos si el dinero llegó a 0 para GameOver
        CheckCashAndGameOver();
    }

    void Update()
    {
        //Si el nivel actual NO ES el menu
       if (SceneManager.GetActiveScene().name != "Menu")
       {
            //Reducimos el valoor del Cash constantemente
            currentCash -= cashPassiveOutcome * Time.deltaTime;

            //Revisamos si el dinero llegó a 0 para GameOver
            CheckCashAndGameOver();
        }
    }

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
