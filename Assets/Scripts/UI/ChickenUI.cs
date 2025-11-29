using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChickenUI : MonoBehaviour
{
    #region Props

    [Header("Panel de Stats")]
    [SerializeField] private GameObject statsPanel;

    [Header("Datos del Pollito")]
    [SerializeField] private TextMeshProUGUI txtNombre;
    [SerializeField] private TextMeshProUGUI txtTipo;

    [Header("Barras de Stats")]
    [SerializeField] private Slider statsHambreBar;
    [SerializeField] private Slider statsFelicidadBar;
    [SerializeField] private Slider statsPesoBar;

    [Header("Iconos de Aimento")]
    [SerializeField] private GameObject icoHarina;
    [SerializeField] private GameObject icoSoja;
    [SerializeField] private GameObject icoGusanos;
    [SerializeField] private GameObject icoMaiz;

    [Header("Textos de info")]
    [SerializeField] private TextMeshProUGUI txtGustos;
    [SerializeField] private TextMeshProUGUI txtPasiva;

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
        HideChickenInfo();
        HideReaction();
        HideEstimulation();
    }

    //------------------------------------------------------------------------------------
    // FUNCION: Setear Preferencia de Ingredientes

    public void SetIngredientsPreference(bool likeHarina, bool likeSoja, bool likeGusanos, bool likeMaiz)
    {
        if (likeHarina) icoHarina.SetActive(true);
        else icoHarina.SetActive(false);

        if (likeSoja) icoSoja.SetActive(true);
        else icoSoja.SetActive(false);

        if (likeGusanos) icoGusanos.SetActive(true);
        else icoGusanos.SetActive(false);

        if (likeMaiz) icoMaiz.SetActive(true);
        else icoMaiz.SetActive(false);
    }

    //------------------------------------------------------------------------------------
    // FUNCION: Setear informacion extra

    public void SetInfo(string name, string type, string gustos, string pasiva)
    {
        //Mostramos el Panel de estadisticas y el HealthBar
        txtNombre.text = $"{name}";
        txtTipo.text = $"Tipo: {type}";
        txtGustos.text = $"Le gusta: {gustos}";
        txtPasiva.text = $"Pasiva: {pasiva}";
    }

    //------------------------------------------------------------------------------------
    // FUNCION: Mostrar informacion de Pollito

    public void ShowChickenInfo()
    {
        //Mostramos el Panel de estadisticas y el HealthBar
        statsPanel.SetActive(true);
    }

    public void HideChickenInfo()
    {
        //Mostramos el Panel de estadisticas y el HealthBar
        statsPanel.SetActive(false);
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

    // ------------------------------------------------------------------------------------

    void Update()
    {
        //Actualizamos frecuentemente el valor de los Slider en base a los Stats
        statsHambreBar.value = chickenStats.hambre;
        statsFelicidadBar.value = chickenStats.felicidad;
        statsPesoBar.value = chickenStats.peso;

    }

}
