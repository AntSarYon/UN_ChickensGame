using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_FadeOut : MonoBehaviour
{
    [Header("Textos de campania")]
    [SerializeField] private TextMeshProUGUI txtCampaignInfo;
    [SerializeField] private TextMeshProUGUI txtDayInfo;

    // Componentes
    private Animator mAnimator;

    // --------------------------------------------------------------------

    void Awake()
    {
        //Obteneoms referencia al Animator
        mAnimator = GetComponent<Animator>();
    }

    // --------------------------------------------------------------------

    void Start()
    {
        // Actualizamos la información sobre la campaña
        txtCampaignInfo.text = $"Jornada {CampaignManager.Instance.CampaignCounter}";
        txtDayInfo.text = $"Dia #{CampaignManager.Instance.DayCounter}";
    }

    // --------------------------------------------------------------------

    public void Play_FadeInGameOver()
    {
        mAnimator.Play("FadeIn_GameOver");
    }
    public void Play_FadeInDayOver()
    {
        mAnimator.Play("FadeIn_DayOver");
    }

    // --------------------------------------------------------------------

    public void GoToNextLevel()
    {
        SceneManager.LoadScene("Level1");
    }
}
