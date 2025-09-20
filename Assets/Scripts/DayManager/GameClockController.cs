using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClockController : MonoBehaviour
{
    public static GameClockController Instance;

    [Header("LightPanel UI")]
    [SerializeField] private Image LightPanelUI;

    //Txt con el texto de la Hora actual
    private string clockString;

    private Color lightPanelColor;

    //Nivel de oscuridad...
    private float currentDarknessLevel = 0.00f;

    private float initialDarknessLevel = 0.00f;
    private float finalDarknessLevel = 0.60f;
    private float interpolation = 0;

    //Tiempo transcurrido
    private float elapsedTime = 0;

    //Tiempo de finalizacion del dia
    private float dayFinishTime = 0;

    // Flag de "Dia en curso"
    private bool bDayIsRunning = false;

    [Header("Tiempo en 1 dia")]
    [SerializeField] private float timeInADay = 86400f; // segundos a considerar

    [Header("Hora de inicio del dia")]
    [Range(0, 24)][SerializeField] private int startingTime = 9;

    [Header("Hora de fin del dia")]
    [Range(0, 24)][SerializeField] private int finishTime = 18;

    [Header("Cuan rápido pasa el tiempo")]
    [Range(1,5)] [SerializeField] private float timeScale = 2f;

    public string ClockString { get => clockString; private set => clockString = value; }

    // -------------------------------------------------------------------------------------------------

    void Awake()
    {
        Instance = this;
    }

    // -------------------------------------------------------------------------------------------------

    void Start()
    {
        //Traemos los parametros del RulesManager
        startingTime = GameRulesManager.instance.startingTime;
        finishTime = GameRulesManager.instance.finishTime;
        timeScale = GameRulesManager.instance.timeScale;

        //Definir Tiempo (Hora) inicial
        elapsedTime = startingTime * 3600f; // (Hora deseada x 3600 segundos)

        //Definir Tiempo (Hora) de Fin del dia
        dayFinishTime = finishTime * 3600f;

        //Almacenamos el color original del Panel de Luz
        lightPanelColor = LightPanelUI.color;

        //Activamos Flag de "Dia esta corriendo"
        bDayIsRunning = true;
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

            //Actualizmaos la UI del tiempo (reloj)
            UpdateClockUI();

            //Actualizmaos el nivel de oscuridad
            UpdateDarknessLevel();

            // Si el tiempo transcurrido lleg{o al final del dia...
            if (elapsedTime >= dayFinishTime)
            {
                //Llamamos al evento de Fin del Dia.
                DayStatusManager.Instance.TriggerEvent_DayOver();

                //Desactivamos Flag de "Dia esta corriendo"
                bDayIsRunning = false;
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    //FUNCION: Actualizar Reloj de la UI

    public void UpdateClockUI()
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

    public void UpdateDarknessLevel()
    {
        //Calculamos la interpolacion constantemente, segun que tan cerca estemos del fin del dia...
        interpolation = elapsedTime / dayFinishTime;

        //Calculamos el nivel de oscuridad actual empleando interpolacion segun el tiempo transcurrido
        currentDarknessLevel = Mathf.Lerp(initialDarknessLevel, finalDarknessLevel, interpolation);

        //Obtenemos el nuevo color (se altera el Alpha en base a al interpolacion)
        Color newDarknessColor = new Color(lightPanelColor.r, lightPanelColor.g, lightPanelColor.b, currentDarknessLevel);
        
        //Asignamos el nuevo color
        LightPanelUI.color = newDarknessColor;
    }

}
