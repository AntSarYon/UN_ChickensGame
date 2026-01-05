using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChickenUI : MonoBehaviour
{

    [Header("Globo de Reaccion")]
    [SerializeField] private GameObject imgReactionBallon;
    [SerializeField] private Image imgReaction;

    [Header("Iconos de reaccion")]
    [SerializeField] private Sprite spLike;
    [SerializeField] private Sprite spDisllike;
    [SerializeField] private Sprite spHungry;
    [SerializeField] private Sprite spAngry;
    [SerializeField] private Sprite spFighting;

    [Header("Icono de estimulacion")]
    [SerializeField] private GameObject imgEstimulated;

    [Header("Icono de Suenio")]
    [SerializeField] private GameObject imgSleeping;

    [Header("Slider de Comida")]
    [SerializeField] private Slider hungrySlider;

    //Referencia a Stats del pollo
    private ChickenController chkController;
    private ChickenStats chickenStats;

    //------------------------------------------------------------------------------------

    void Start()
    {
        //Obtenemos referencia a scripts del Pollito Owner de UI
        chickenStats = GetComponentInParent<ChickenStats>();
        chkController = GetComponentInParent<ChickenController>();

        //Iniciamos con todos los Elementos de la UI Desactivados
        HideReaction();
        HideEstimulation();
        HideHungryBar();
    }

    //------------------------------------------------------------------------------------

    void Update()
    {
        // Si el pollito esta durmiendo
        if (chkController.bIsSleeping)
        {
            // Ocultamos cualquier globo de reaccion, asi como la barra de comida
            HideReaction();
            HideHungryBar();
            return;
        }

        // Si esta despierto...
        //Dependiendo del Flag activo, mostraremos un globo de reaccion
        if (chkController.bIsEating)
        {
            HideReaction();
            ShowHungryBar();
            return;
        }

        if (chkController.bIsFighting)
        {
            ShowFighting();
        }
        else if (chkController.bIsAngry)
        {
            ShowAngry();
        }
        else if (chkController.bIsStarving)
        {
            ShowHungry();
        }
        //En cualquier otro caso, ocultamos el globo de reaccion
        else
        {
            HideReaction();
        }

    }


    //------------------------------------------------------------------------------------
    // FUNCIONES: Mostrar / Ocultar Reaccion por Comida

    public void ShowLike()
    {
        //Asignamos ell Sprite de Like
        imgReaction.sprite = spLike;

        //Mostramos el Globo de Reaccion
        imgReactionBallon.SetActive(true);
    }

    public void ShowDislike()
    {
        //Asignamos el Sprite de Dislike
        imgReaction.sprite = spDisllike;

        //Mostramos el Globo de Reaccion
        imgReactionBallon.SetActive(true);
    }

    public void ShowHungry()
    {
        //Asignamos ell Sprite de Like
        imgReaction.sprite = spHungry;

        //Mostramos el Globo de Reaccion
        imgReactionBallon.SetActive(true);
    }

    public void ShowAngry()
    {
        //Asignamos el Sprite de Dislike
        imgReaction.sprite = spAngry;

        //Mostramos el Globo de Reaccion
        imgReactionBallon.SetActive(true);
    }

    public void ShowFighting()
    {
        //Asignamos el Sprite de Dislike
        imgReaction.sprite = spFighting;

        //Mostramos el Globo de Reaccion
        imgReactionBallon.SetActive(true);
    }

    // ---------------------------------------------------------------------

    public void HideReaction()
    {
        //Mostramos el Globo de Reaccion
        imgReactionBallon.SetActive(false);
    }

    //------------------------------------------------------------------------------------
    // FUNCIONES: Mostrar / Ocultar Icono de Estimulacion

    public void ShowEstimulation()
    {
        imgEstimulated.SetActive(true);
    }

    public void HideEstimulation()
    {
        imgEstimulated.SetActive(false);
    }

    // ---------------------------------------------------------------------

    public void ShowHungryBar()
    {
        //Mostramos la Barra de Hambre
        hungrySlider.gameObject.SetActive(true);
    }

    public void SetHugryBarValue(float val)
    {
        hungrySlider.value = (int) val;
    }

    public void HideHungryBar()
    {
        hungrySlider.gameObject.SetActive(false);
    }

}
