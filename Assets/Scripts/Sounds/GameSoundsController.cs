using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundsController : MonoBehaviour
{
    public static GameSoundsController Instance;

    private AudioSource mAudioSource;

    [SerializeField] private AudioClip resourceBoughtSound;
    [SerializeField] private AudioClip oldMachineCashOutSound;
    [SerializeField] private AudioClip chickenSoldCashOutSound;
    [SerializeField] private AudioClip[] arrChickenSoldScreamsSound = new AudioClip[2];
    [SerializeField] private AudioClip chickenDeathSound;

    [SerializeField] private AudioClip[] arrChickenSpawnSounds = new AudioClip[2];
    [SerializeField] private AudioClip bubbleSound;
    [SerializeField] private AudioClip yardChangeSound;
    [SerializeField] private AudioClip showFoodPanelSound;
    [SerializeField] private AudioClip hideFoodPanelSound;

    [SerializeField] private AudioClip toyPlacementSound;

    //-----------------------------------------------------------------

    void Awake()
    {
        //Controlamos que esta sea la unica instancia de la Clase GameManager
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        //Almacenamos referencia de componentes
        mAudioSource = GetComponent<AudioSource>();
    }

    //---------------------------------------------------------------------------------

    public void PlayChickenSoldSound()
    {
        //Obtenemos indice aleatorio para el grito del pollo
        int screamIndex = Random.Range(0, 2);

        //Reproducimos sonido de Grito de Pollo sefgun el indice
        mAudioSource.PlayOneShot(arrChickenSoldScreamsSound[screamIndex], 0.40f);

        //Reproducimos sonido de Venta de Pollo
        mAudioSource.PlayOneShot(chickenSoldCashOutSound, 0.35f);
    }

    public void PlayResourceBoughtSound()
    {
        //Reproducimos sonido de Recurso Comprado
        mAudioSource.PlayOneShot(resourceBoughtSound, 0.35f);
    }
    public void PlayOldMachineCashOutSound()
    {
        //Reproducimos sonido de Venta de Pollo
        mAudioSource.PlayOneShot(oldMachineCashOutSound, 0.35f);
    }

    public void PlayChickenDeathSound()
    {
        //Reproducimos sonido de Venta de Pollo
        mAudioSource.PlayOneShot(chickenDeathSound, 0.50f);
    }

    public void PlayChickenSpawnSound()
    {
        //Obtenemos indice aleatorio para el grito del pollo
        int screamIndex = Random.Range(0, 2);

        //Reproducimos sonido de Recurso Comprado
        mAudioSource.PlayOneShot(resourceBoughtSound, 0.35f);

        //Reproducimos sonido de Burbuja
        mAudioSource.PlayOneShot(bubbleSound, 0.40f);

        //Reproducimos sonido de Grito de Pollo sefgun el indice
        mAudioSource.PlayOneShot(arrChickenSpawnSounds[screamIndex], 0.65f);
    }

    public void PlayYardChangeSound()
    {
        //Reproducimos sonido de Venta de Pollo
        mAudioSource.PlayOneShot(yardChangeSound, 0.50f);
    }

    public void PlayShowFoodPanelSound()
    {
        //Reproducimos sonido de Venta de Pollo
        mAudioSource.PlayOneShot(showFoodPanelSound, 0.50f);
    }

    // ----------------------------------------------------

    public void PlayHideFoodPanelSound()
    {
        //Reproducimos sonido de Venta de Pollo
        mAudioSource.PlayOneShot(hideFoodPanelSound, 0.50f);
    }

    // -----------------------------------------------------

    public void PlayToyPlacementSound()
    {
        //Reproducimos sonido de Venta de Pollo
        mAudioSource.PlayOneShot(toyPlacementSound, 0.65f);
    }
}
