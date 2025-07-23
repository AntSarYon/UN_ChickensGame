using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region Props

    [SerializeField] private Button btnLightSwitch;
    [SerializeField] private Button btnSleepOrder;
    [SerializeField] private GameObject LightPanel;

    #endregion

    //----------------------------------------------------------------------------------

    #region methods

    void Start()
    {
        //La luz inicia encendida
        TurnOnLight();

        btnLightSwitch.onClick.AddListener(ToggleLight);
        btnSleepOrder.onClick.AddListener(ToggleSleepOrder);
    }

    //----------------------------------------------------------------------------------

    public void TurnOffLight()
    {
        LightPanel.SetActive(true);
    }

    public void TurnOnLight()
    {
        LightPanel.SetActive(false);
    }

    //----------------------------------------------------------------------------------

    public void ToggleLight()
    {
        //Si el Panel esta activo (Luz apagada)
        if (LightPanel.activeSelf)
        {
            //Activamos la Luz (desactivamos el panel)
            TurnOnLight();
        }
        //Caso ontrario
        else
        {
            //Apagamos la luz (Activamos el Panel)
            TurnOffLight();
        }
    }

    //----------------------------------------------------------------------------------

    public void ToggleSleepOrder()
    {
        DayStatusManager.instance.SleepOrderClicked();
    }

    #endregion
}
