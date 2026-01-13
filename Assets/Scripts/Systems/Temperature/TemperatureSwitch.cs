using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureSwitch : Interactable
{
    // Flag de "Esta encendido"
    [HideInInspector] public bool bIsON;

    [Header("Clips de Sonido")]
    [SerializeField] private AudioClip clipSwitch;

    private AudioSource mAudioSource;

    // --------------------------------------------

    private void Awake()
    {
        //Obtenemos referencia a Audio Source
        mAudioSource = GetComponent<AudioSource>();

        // Inicia Apagado
        bIsON = false;
        interactionMessage = "Encender calefactor";
    }

    // ---------------------------------------------------

    public void TurnOn()
    {
        //Activamos Fag de ´Prendido"
        bIsON = true;

        // Le decimos al Maager de Temp, que aumente la temperatura
        TemperatureManager.Instance.GetWarmer();

        SetInteractioonMessage("Apagar calefactor");

        //Reproducimos sonido de Encendido en Loop
        PlayONSound();
    }

    // ---------------------------------------------------

    public void TurnOff()
    {
        //Desactivamos Flag de ´Prendido"
        bIsON = false;

        // Le decimos al Maager de Temp, que reduzca la temperatura
        TemperatureManager.Instance.GetColder();
        SetInteractioonMessage("Encender calefactor");
    }

    // ---------------------------------------------------

    public override void Interact(Transform holdingZone = null, InteractionController interactionController = null)
    {
        //Reproducimos sonido de Switch
        PlaySwitchSound();

        // Apagamos o prendemos el calefactor segun el caso
        if (bIsON)
            TurnOff();
        else 
            TurnOn();
    }

    // ---------------------------------------------------

    private void PlaySwitchSound()
    {
        //Detenemos el AudioSource
        mAudioSource.Stop();

        //Desactivamos el loop 
        mAudioSource.loop = false;

        //Reproducimos OneShot del Sonido de Switch
        mAudioSource.PlayOneShot(clipSwitch, 0.45f);
    }

    // ------------------------------------------------------

    private void PlayONSound()
    {
        //Reproducimos sonido de Encendido en Loop
        mAudioSource.loop = true;
        mAudioSource.Play();
    }
}
