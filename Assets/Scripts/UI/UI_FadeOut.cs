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
        // Actualizamos la información sobre la campaña
        txtCampaignInfo.text = $"Jornada {CampaignManager.Instance.CampaignCounter}";
        txtDayInfo.text = $"Dia #{CampaignManager.Instance.DayCounter}";
    }
}
