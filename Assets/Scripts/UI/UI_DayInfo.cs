using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_DayInfo : MonoBehaviour
{
    [Header("Textos de detalle del dia")]
    [SerializeField] private TextMeshProUGUI txtCampaign;
    [SerializeField] private TextMeshProUGUI txtDay;

    [Header("Texto de Hora")]
    [SerializeField] private TextMeshProUGUI txtTimeClock;

    // -------------------------------------------------------

    void Start()
    {
        txtCampaign.text = $"Jornada {CampaignManager.Instance.CampaignCounter}";
        txtDay.text = $"Dia {CampaignManager.Instance.DayCounter}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
