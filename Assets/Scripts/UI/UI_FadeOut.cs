using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_FadeOut : MonoBehaviour
{
    [Header("Textos de campania")]
    [SerializeField] private TextMeshProUGUI txtCampaignInfo;
    [SerializeField] private TextMeshProUGUI txtDayInfo;

    // --------------------------------------------------------------------

    void Start()
    {
        // Actualizamos la informaci�n sobre la campa�a
        txtCampaignInfo.text = $"Jornada {CampaignManager.Instance.CampaignCounter}";
        txtDayInfo.text = $"Dia #{CampaignManager.Instance.DayCounter}";
    }
}
