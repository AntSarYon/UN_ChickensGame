using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClockController : MonoBehaviour
{
    public static GameClockController Instance;

    //Txt con el texto de la Hora actual
    private string clockString;

    [SerializeField] private Light DayLight;

    // Vectores RGBA de Luz objetivo
    private Vector4 morningDayLight = Vector4.one;
    private Vector4 AfternoonDayLight = new Color(1.00f, 0.78f, 0.57f, 1.00f);
    private Vector4 EveningDayLight = new Color(0.42f, 0.47f, 0.95f, 1.00f);

    // Vectores para interpolacion de Luz ...
    private Vector4 initialDayLight;
    private Vector4 currentDayLight;
    private Vector4 targetDayLight;

    //Tiempo para completar el cambio de iluminacion
    private float targetChangeTime;

    private float interpolation = 0;

    //Tiempo transcurrido
    private float elapsedTime = 0;

    //Tiempo de finalizacion del dia
    private float dayFinishTime = 0;

    // Flag de "Dia en curso"
    private bool bDayIsRunning = false;

    [Header("Tiempo en 1 dia")]
    private float timeInADay = 24 * 3600f; // 24 Horas (* segundos)

    [Header("Hora de inicio del dia")]
    [Range(0, 24)][SerializeField] private int startingTime = 10;

    [Header("Hora de fin del dia")]
    [Range(0, 24)][SerializeField] private int finishTime = 18;

    [Header("Cuan rápido pasa el tiempo")]
    [Range(1,10)] [SerializeField] private float timeScale = 2.00f;

    public string ClockString { get => clockString; private set => clockString = value; }

    // -------------------------------------------------------------------------------------------------

    void Awake()
    {
        Instance = this;

        initialDayLight = Vector4.one;
        currentDayLight = Vector4.one;
        targetDayLight = Vector4.one;
    }

    // -------------------------------------------------------------------------------------------------

    void Start()
    {
        //Definir Tiempo (Hora) inicial
        elapsedTime = startingTime * 3600f; // (Hora deseada x 3600 segundos)

        //Definir Tiempo (Hora) de Fin del dia 
        dayFinishTime = finishTime * 3600f; // (Hora deseada x 3600 segundos)

        //Activamos Flag de "Dia esta corriendo"
        bDayIsRunning = true;

        //Seteamos el Color Inicial de la Luz a Blanco
        DayLight.color = new Color(
            morningDayLight.x,
            morningDayLight.y,
            morningDayLight.z,
            morningDayLight.w
            );
    }

    //-------------------------------------------------------------------------------------------------

    void Update()
    {
        //Si el dia esta corriendo...
        if (bDayIsRunning)
        {
            //El tiempo transcurrido se incrementa progresivamente; considerando la escala del tiempo
            elapsedTime += Time.deltaTime * timeScale * 50;

            // Si el tiempo ranscurrido llega al total establecido por el dia; se reiniciará
            elapsedTime %= timeInADay;

            // Si ya son pasadas las 5pm...
            if (elapsedTime >= 17 * 3600f)
            {
                //Seteamos la luz inicial como la de a tarde
                initialDayLight = AfternoonDayLight;

                //Asignams commo Luz Target la de Evening
                targetDayLight = EveningDayLight;

                //Definimos el tiempo en que el cambio debe ser completado...
                targetChangeTime = 18 * 3600f;
            }
            // Si ya son pasadas las 3pm...
            else if (elapsedTime >= 15 * 3600f)
            {

                //Seteamos la luz inicial como la de la manana
                initialDayLight = morningDayLight;

                //Asignams commo Luz Target la de Evening
                targetDayLight = AfternoonDayLight;

                //Definimos el tiempo en que el cambio debe ser completado...
                targetChangeTime = 17 * 3600f;
            }

            //Actualizmaos la UI del tiempo (reloj)
            UpdateClockString();

            //Actualizmaos el color de la luz del sol
            UpdateDayLightColor();

            // Si el tiempo transcurrido lleg{o al final del dia...
            if (elapsedTime >= dayFinishTime)
            {
                // Disparamos Logica de fin del dia...
                DayStatusManager.Instance.TriggerEvent_GameOver();

                //Desactivamos Flag de "Dia esta corriendo"
                bDayIsRunning = false;
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    //FUNCION: Actualizar String de Tiempo

    public void UpdateClockString()
    {
        //Calculamos el valoer de Hora. minuto y segundos, en base a Tiempo Transcurrido
        int hours = Mathf.FloorToInt(elapsedTime / 3600f);
        int minutes = Mathf.FloorToInt((elapsedTime - hours *3600f) / 60f);
        //int seconds = Mathf.FloorToInt((elapsedTime - hours * 3600f) - (minutes * 60f));

        //Creamos el String con los datos de tiempo
        //string clockString = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        //Creamos el String con los datos de tiempo
        clockString = string.Format("{0:00}:{1:00}", hours, minutes);
    }

    //-------------------------------------------------------------------------------------------------
    // FUNCION: Actualizar Nivel de Oscuridad
    public void UpdateDayLightColor()
    {
        //Calculamos la interpolacion constantemente, segun que tan cerca estemos de la siguiente hora clave...
        interpolation = elapsedTime / targetChangeTime;

        //Calculamos el nivel de oscuridad actual empleando interpolacion segun el tiempo transcurrido
        currentDayLight = Vector4.Lerp(initialDayLight, targetDayLight, interpolation);

        Debug.Log(currentDayLight);

        //Actualizams el color de la Luz...
        DayLight.color = currentDayLight;


        //Obtenemos el nuevo color (se altera el Alpha en base a al interpolacion)
        //Color newDarknessColor = new Color(lightPanelColor.r, lightPanelColor.g, lightPanelColor.b, currentDarknessLevel);
        
        //Asignamos el nuevo color
        //LightPanelUI.color = newDarknessColor;
    }

    // --------------------------------------------------------

    public string GetTimeString()
    {
        return clockString;
    }

}
