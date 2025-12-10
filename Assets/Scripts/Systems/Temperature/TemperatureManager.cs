using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TemperatureManager : MonoBehaviour
{
    public static TemperatureManager Instance;

    [SerializeField] public float temperature = 25f; // Temperatura inicial
    [SerializeField] private float temperatureDecreaseInterval = 5f; // Cada cu�ntos segundos cambia
    [SerializeField] private float temperatureDecreaseAmount = 1f;   // Cu�nto baja cada vez
    [SerializeField] private float temperatureIncreaseAmount = 1f;   // Cu�nto sube cada vez
    
    private float temperatureTimer = 0f;
    private bool isTemperatureIncreasing = false;

    // Evento para notificar cambios de temperatura
    public UnityAction<float> OnTemperatureChanged;

    // --------------------------------------------------------------------------------------------

    void Awake()
    {
        // Asignamos INstancia
        Instance = this;
    }

    // ---------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        //Se inicializa el Timer de temperatura
        temperatureTimer = temperatureDecreaseInterval;
    }

    // ------------------------------------------------------------

    void Update()
    {
        //Disminuimos el Timer de Temperatura
        temperatureTimer -= Time.deltaTime;

        //Si el Timer llega a 0
        if (temperatureTimer <= 0f)
        {
            //Si la temperatura esta subiendo
            if (isTemperatureIncreasing)
            {
                //Incrementamos la temperatura segun la variable definida.
                temperature += temperatureIncreaseAmount;
                temperature = Mathf.Clamp(temperature, 0, 41);
            }
            //En caso este baando
            else
            {
                // Reducimos la temperatura segun variabe definida
                temperature -= temperatureDecreaseAmount;
                temperature = Mathf.Clamp(temperature, 0, 41);
            }

            //Reiniciamos el Timer al valr inicial
            temperatureTimer = temperatureDecreaseInterval;

            // Notificar a los suscriptores que la temperatura cambió
            TriggerEvent_TemperatureChanged();
        }
    }

    // --------------------------------------------------------------------------

    private void TriggerEvent_TemperatureChanged()
    {
        OnTemperatureChanged?.Invoke(temperature);
    }

    // ------------------------------------------------------------------------

    public void GetWarmer()
    {
        //Activamos Flag de "Temperatura en aumento"
        isTemperatureIncreasing = true;
    }

    public void GetColder()
    {
        //Desactivamos Flag de "Temperatura en aumento"
        isTemperatureIncreasing = false;
    }
}
