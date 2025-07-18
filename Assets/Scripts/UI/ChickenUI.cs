using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenUI : MonoBehaviour
{
    #region Props

    //Panel de esatisticas
    [SerializeField] private GameObject statsPanel;

    // Slider de HealthBar
    [SerializeField] private GameObject HPBar;

    //Icono de Alerta por Pelea
    [SerializeField] private GameObject AlertIcon;


    #endregion

    //------------------------------------------------------------------------------------

    void Start()
    {
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

}
