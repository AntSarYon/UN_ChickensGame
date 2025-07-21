using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameClockController : MonoBehaviour
{
    [Header("Clock UI")]
    [SerializeField] private TextMeshProUGUI txtDayClock;

    //Tiempo transcurrido
    private float elapsedTime = 0;

    [Header("Tiempo en 1 dia")]
    [SerializeField] private float timeInADay = 86400f; // segundos a considerar

    [Header("Hora de inicio del dia")]
    [Range(0, 24)][SerializeField] private int startingTime = 12;

    [Header("Cuan rápido pasa el tiempo")]
    [Range(1,100)] [SerializeField] private float timeScale = 2.00f;

    //-------------------------------------------------------------------------------------------------

    void Start()
    {
        //Definir Tiempo (Hora) inicial
        elapsedTime = startingTime * 3600f; // (Hora deseada x 3600 segundos)

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
    }

    //-------------------------------------------------------------------------------------------------

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

    public void CheckTime()
    {
        /*
        if (elapsedTime >= )
        {

        }
        */
    }
}
