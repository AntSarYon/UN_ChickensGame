using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChickenUI : MonoBehaviour
{
    #region Props

    [Header("Globo de Reaccion (por comida)")]
    [SerializeField] private GameObject imgReactionBallon;
    [SerializeField] private Image imgReaction;
    [SerializeField] private Sprite spLike;
    [SerializeField] private Sprite spDisllike;

    [Header("Icono de estimulacion")]
    [SerializeField] private GameObject imgEstimulated;

    //Referencia a Stats del pollo
    private ChickenStats chickenStats;

    #endregion

    //------------------------------------------------------------------------------------

    void Start()
    {
        //Obtenemos los Stats del Pollito Owner de esta UI
        chickenStats = GetComponentInParent<ChickenStats>();

        //Iniciamos con todos los Elementos de la UI Desactivados
        HideReaction();
        HideEstimulation();
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

}
