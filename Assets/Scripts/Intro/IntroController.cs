using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    [SerializeField] private AudioClip clipChickTrickAppereance;
    [SerializeField] private AudioClip clipGoUp;

    private AudioSource mAudiosSource;

    //--------------------------------------------------------------------

    void Awake()
    {
        //Obtenemos referencia a Componentes
        mAudiosSource = GetComponent<AudioSource>();
    }


    public void PlayChickTrickAppereance()
    {
        mAudiosSource.PlayOneShot(clipChickTrickAppereance, 0.70f);
    }

    public void PlayGoUp()
    {
        mAudiosSource.PlayOneShot(clipGoUp, 0.60f);
    }

    //--------------------------------------------------------------------

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
