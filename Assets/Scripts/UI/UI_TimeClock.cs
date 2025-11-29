using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_TimeClock : MonoBehaviour
{
    // Texto de Hora
    [SerializeField] private TextMeshProUGUI txtHour;

    void Start()
    {
        
    }

    // --------------------------------------------------------------

    void Update()
    {
        txtHour.text = GameClockController.Instance.GetTimeString();
    }
}
