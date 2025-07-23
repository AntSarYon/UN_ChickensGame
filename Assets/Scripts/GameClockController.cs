using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameClockController : MonoBehaviour
{
    [Header("Clock UI")]
    [SerializeField] private TextMeshProUGUI txtDayClock;

    [Header("LightPanel UI")]
    [SerializeField] private Image LightPanelUI;

    private Color lightPanelColor;

    //Nivel de oscuridad...
    private float currentDarknessLevel = 0.00f;

    private float initialDarknessLevel = 0.00f;
    private float finalDarknessLevel = 0.60f;
    private float interpolation = 0;

    //Tiempo transcurrido
    private float elapsedTime = 0;

    [Header("Tiempo en 1 dia")]
    [SerializeField] private float timeInADay = 86400f; // segundos a considerar

    [Header("Hora de inicio del dia")]
    [Range(0, 24)][SerializeField] private int startingTime = 9;

    [Header("Cuan rápido pasa el tiempo")]
    [Range(1,100)] [SerializeField] private float timeScale = 2.00f;

    //-------------------------------------------------------------------------------------------------

    void Start()
    {
        //Definir Tiempo (Hora) inicial
        elapsedTime = startingTime * 3600f; // (Hora deseada x 3600 segundos)

        //Almacenamos el color original del Panel de Luz
        lightPanelColor = LightPanelUI.color;
    }

    //-------------------------------------------------------------------------------------------------

    void Update()
    {
        //El tiempo transcurrido se incrementa progresivamente; considerando la escala del tiempo
        elapsedTime += Time.deltaTime * timeScale;

        

        // Si el tiempo ranscurrido llega al total establecido por el dia; se reiniciará
        elapsedTime %= timeInADay;

        //Actualizmaos la UI del tiempo (reloj)
        UpdateClockUI();

        //Actualizmaos el nivel de oscuridad
        UpdateDarknessLevel();
    }

    //-------------------------------------------------------------------------------------------------
    //FUNCION: Actualizar Reloj de la UI

    public void UpdateClockUI()
    {
        //Calculamos el valoer de Hora. minuto y segundos, en base a Tiempo Transcurrido
        int hours = Mathf.FloorToInt(elapsedTime / 3600f);
        int minutes = Mathf.FloorToInt((elapsedTime - hours *3600f) / 60f);
        int seconds = Mathf.FloorToInt((elapsedTime - hours * 3600f) - (minutes * 60f));

        //Creamos el String con los datos de tiempo
        string clockString = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

        //Asignamos el String creado al elemento de UI
        txtDayClock.text = clockString;
    }

    //-------------------------------------------------------------------------------------------------
    // FUNCION: Actualizar Nivel de Oscuridad

    public void UpdateDarknessLevel()
    {
        //Calculamos la interpolacion constantemente, segun que tan cerca estemos del fin del dia...
        interpolation = elapsedTime / timeInADay;

        //Calculamos el nivel de oscuridad actual empleando interpolacion segun el tiempo transcurrido
        currentDarknessLevel = Mathf.Lerp(initialDarknessLevel, finalDarknessLevel, interpolation);

        //Obtenemos el nuevo color (se altera el Alpha en base a al interpolacion)
        Color newDarknessColor = new Color(lightPanelColor.r, lightPanelColor.g, lightPanelColor.b, currentDarknessLevel);
        
        //Asignamos el nuevo color
        LightPanelUI.color = newDarknessColor;
    }

    public void CheckTime()
    {
        /*
        if (elapsedTime >= )
        {

        }
        */
    }
}
