using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    //Instancia de Clase
    public static GameManager Instance;

    //Cash del jugador...
    [HideInInspector] public int currentCash;

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
    //public UnityAction<int> OnChickenSold;

    //---------------------------------------------------
    void Awake()
    {
        //Controlamos que esta sea la unica instancia de la Clase GameManager
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        //El dinero empieza en 250$
        currentCash = 250;

        //Empezamos con 0 pollitos muertos
        currentDeathChicken = 0;
    }

    void Start()
    {
        
    }


    //-----------------------------------------------------------
    // Funcion de Evento - ChickenSold

    public void TriggerEvent_ChickenSold(float chickenValue)
    {
        float newCash = currentCash + (5 * chickenValue);

        currentCash = (int)newCash;

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
    }

    public void TriggerEvent_GasRefill()
    {
        currentCash -= 50;

        //Hacemos que el SoundsManager reproduzca sonido de Compra de Recurso
        GameSoundsController.Instance.PlayResourceBoughtSound();

        //Disparamos el Evento de Refill de Comida
        //enviando el Valor de la Gallina a los Delegados
        OnGasRefill?.Invoke();
    }

    public void TriggerEvent_OnChickenDeath()
    {
        //Incrementamos la cantidad de Pollitos Muertos.
        currentDeathChicken++;

        //Disparamos el Evento de Refill de Comida
        //enviando el Valor de la Gallina a los Delegados
        OnChickenDeath?.Invoke();
    }
}
