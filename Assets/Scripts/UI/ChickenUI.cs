using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    [Header("Textos debug de flags")]
    [SerializeField] private TextMeshProUGUI txtAsignado;
    [SerializeField] private TextMeshProUGUI txtDurmiendo;
    [SerializeField] private TextMeshProUGUI txtComiendo;
    [SerializeField] private TextMeshProUGUI txtHambre;
    [SerializeField] private TextMeshProUGUI txtPeleando;
    [SerializeField] private TextMeshProUGUI txtEnojado;
    [SerializeField] private TextMeshProUGUI txtCaminando;

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
        if (chkController.assignedFoodSlot) txtAsignado.gameObject.SetActive(true);
        else txtAsignado.gameObject.SetActive(false);

        if (chkController.bIsSleeping) txtDurmiendo.gameObject.SetActive(true);
        else txtDurmiendo.gameObject.SetActive(false);

        if (chkController.bIsEating) txtComiendo.gameObject.SetActive(true);
        else txtComiendo.gameObject.SetActive(false);

        if (chkController.bIsStarving) txtHambre.gameObject.SetActive(true);
        else txtHambre.gameObject.SetActive(false);

        if (chkController.bIsFighting) txtPeleando.gameObject.SetActive(true);
        else txtPeleando.gameObject.SetActive(false);

        if (chkController.bIsAngry) txtEnojado.gameObject.SetActive(true);
        else txtEnojado.gameObject.SetActive(false);

        if (chkController.bIsWalking) txtCaminando.gameObject.SetActive(true);
        else txtCaminando.gameObject.SetActive(false);

        // Si el pollito esta durmiendo
        if (chkController.bIsSleeping)
        {
            // Ocultamos cualquier globo de reaccion, asi como la barra de comida
            HideReaction();
            if (chkController.bIsStarving)
            {
                ShowHungryBar();
            }
            else
            {
                HideHungryBar();
            }

        }
        // Si esta despierto...
        else
        {
            
            //Dependiendo del Flag activo, mostraremos un globo de reaccion
            if (chkController.bIsFighting)
            {
                ShowFighting();
                HideHungryBar();
            }
            else if (chkController.bIsStarving)
            {
                ShowHungry();
                ShowHungryBar();
            }
            else if (chkController.bIsEating)
            {
                HideReaction();
                ShowHungryBar();
                return;
            }
            //En cualquier otro caso, ocultamos el globo de reaccion
            else
            {
                HideReaction();
                HideHungryBar();
            }
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
        hungrySlider.value = val;
    }

    public void HideHungryBar()
    {
        hungrySlider.gameObject.SetActive(false);
    }

}
