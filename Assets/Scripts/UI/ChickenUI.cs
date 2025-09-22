using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChickenUI : MonoBehaviour
{
    #region Props

    //Panel de esatisticas
    [SerializeField] private GameObject statsPanel;

    // Slider de HealthBar
    [SerializeField] private GameObject HPBar;
    private Slider HPBarSlider;

    //Icono de Alerta por Pelea
    [SerializeField] private GameObject AlertIcon;

    //Referencia a Stats del pollo
    private ChickenStats chickenStats;

    [Header("Stats Bars")]
    [SerializeField] private Slider statsHpBar;
    [SerializeField] private Slider statsHambreBar;
    [SerializeField] private Slider statsfelicidadBar;
    [SerializeField] private Slider statsPesoBar;


    #endregion

    //------------------------------------------------------------------------------------

    void Start()
    {
        //Obtenemos los Stats del Pollito Owner de esta UI
        chickenStats = GetComponentInParent<ChickenStats>();

        //Obtenemos referencia al Slider de HP Principal (fuera de Stats)
        HPBarSlider = HPBar.GetComponent<Slider>();

        //Obtenemos referencia a las Barras de Stadisticas
        statsHpBar = statsPanel.transform.Find("HP").Find("HPSlider").GetComponent<Slider>();
        statsHambreBar = statsPanel.transform.Find("Hambre").Find("HambreSlider").GetComponent<Slider>();
        statsfelicidadBar = statsPanel.transform.Find("felicidad").Find("felicidadSlider").GetComponent<Slider>();
        statsPesoBar = statsPanel.transform.Find("Peso").Find("PesoSlider").GetComponent<Slider>();

        //Iniciamos con todos los Elementos de la UI Desactivados
        statsPanel.SetActive(false);
        HPBar.SetActive(false);
        AlertIcon.SetActive(false);
    }

    //------------------------------------------------------------------------------------
    // FUNCION: Mostrar informacion de Pollito

    public void ShowChickenInfo()
    {
        //Mostramos el Panel de estadisticas y el HealthBar
        statsPanel.SetActive(true);
        HPBar.SetActive(true);
    }

    public void HideChickenInfo()
    {
        //Mostramos el Panel de estadisticas y el HealthBar
        statsPanel.SetActive(false);
        HPBar.SetActive(false);
    }

    //------------------------------------------------------------------------------------
    // FUNCION: Mostrar elementos de Pelea del Pollito

    public void ShowFightInfo()
    {
        //Mostramos el Panel de estadisticas y el HealthBar
        AlertIcon.SetActive(true);
        HPBar.SetActive(true);
    }

    public void HideFightInfo()
    {
        //Mostramos el Panel de estadisticas y el HealthBar
        AlertIcon.SetActive(false);
        HPBar.SetActive(false);
    }

    void Update()
    {
        //Actualizamos valor de los Slider en base a los Stats
        HPBarSlider.value = chickenStats.hp;
        statsHpBar.value = chickenStats.hp;

        statsHambreBar.value = chickenStats.hambre;
        statsfelicidadBar.value = chickenStats.felicidad;
        statsPesoBar.value = chickenStats.peso;

    }

}
