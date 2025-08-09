using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MenuController : MonoBehaviour
{
    [Header("Parameters Panel")]
    [SerializeField] private GameObject parametersPanel;
    [SerializeField] private Button btnParameters;
    [SerializeField] private Button btnCloseParametersPanel;

    [Header("Boton de Jugar")]
    [SerializeField] private Button btnPlay;

    //-------------------------------------------------------

    void Start()
    {
        btnCloseParametersPanel.onClick.AddListener(CloseParametersPanel);
        btnParameters.onClick.AddListener(OpenParametersPanel);

        CloseParametersPanel();
    }

    //-------------------------------------------------------

    public void OpenParametersPanel()
    {
        //Desactivamos el Panel de Parametros...
        parametersPanel.SetActive(true);
    }

    public void CloseParametersPanel()
    {
        //Desactivamos el Panel de Parametros...
        parametersPanel.SetActive(false);
    }
}
