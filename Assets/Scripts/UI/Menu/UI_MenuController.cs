using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MenuController : MonoBehaviour
{
    [Header("Parameters Panel")]
    [SerializeField] private GameObject parametersPanel;
    [SerializeField] private Button btnParameters;
    [SerializeField] private Button btnCloseParametersPanel;

    [Header("Botones de Jugar")]
    [SerializeField] private Button btnPlayNewCmpaign;
    [SerializeField] private Button btnPlayContinueCampaign;

    //-------------------------------------------------------

    void Start()
    {
        btnCloseParametersPanel.onClick.AddListener(CloseParametersPanel);
        btnParameters.onClick.AddListener(OpenParametersPanel);

        btnPlayNewCmpaign.onClick.AddListener(StartNewGame);
        btnPlayContinueCampaign.onClick.AddListener(ContinueGame);
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

    //-------------------------------------------------------

    public void StartNewGame()
    {
        // Reiniciamos a la primera campaña
        //CampaignManager.Instance.RestartToFirstCampaign();

        // Cargamos la escena del juego
        SceneManager.LoadScene("Level1");
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene("Level1");
    }
}
