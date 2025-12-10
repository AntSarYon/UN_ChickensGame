using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Temperature : MonoBehaviour
{
    [SerializeField] private Slider barTemp;
    [SerializeField] private TextMeshProUGUI txtTemp;

    // ---------------------------------------------------

    void Start()
    {
        // Seteamos el valor minimo y Maximo del Slider de temperatura
        barTemp.minValue = 0;
        barTemp.maxValue = 41;

        TemperatureManager.Instance.OnTemperatureChanged += OnTemperatureChangedDelegate;
    }

    // ---------------------------------------------------------
    // Funcion Delegada de Cambio de temperatura
    private void OnTemperatureChangedDelegate(float newTemp)
    {
        // Asignamos valor de temperatura a los elementos de UI
        barTemp.value = newTemp;
        txtTemp.text = $"{newTemp}' C";
    }

    // --------------------------------------------------

    private void OnDestroy()
    {
        TemperatureManager.Instance.OnTemperatureChanged -= OnTemperatureChangedDelegate;
    }

}
