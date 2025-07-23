using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DayStatusManager : MonoBehaviour
{
    //Instancia de Clase
    public static DayStatusManager instance;

    //FLAG - Orden para dormir
    [HideInInspector] public bool orderedToSleep;

    //EVENTO - Orden de Dormir Clickeada...
    public UnityAction<bool> OnSleepOrderClicked;

    //--------------------------------------------------

    private void Awake()
    {
        //Asignamos Instancia de clase
        instance = this;

        //La orden para Dormir inicia desactivada
        orderedToSleep = false;
    }

    //--------------------------------------------------
    //Funcion disparadora de Evento: Orden de Dormir clickeada

    public void SleepOrderClicked()
    {
        //Invertimos el valor de la Orden
        orderedToSleep = !orderedToSleep;

        Debug.Log($"Se cambio la orden de Dormir a: {orderedToSleep}");

        //Invocamos a los delegados,  indicando el estado de la orden
        OnSleepOrderClicked?.Invoke(orderedToSleep);
    }
}
