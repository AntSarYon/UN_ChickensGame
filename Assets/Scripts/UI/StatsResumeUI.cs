using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsResumeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtPesoPrmedio;
    [SerializeField] private TextMeshProUGUI txtMortalidad;
    [SerializeField] private TextMeshProUGUI txtPoblacion;

    private float poblacion;
    private float pollitosMuertos;
    private float tasaMortalidad;
    private float pesoPromedio;

    private List<ChickenStats> listChickensStats = new List<ChickenStats>();

    private float checkTime;
    private float timer;

    //----------------------------------------------------------

    void Awake()
    {
        //La revision ocurrirá cada 3 segundos
        checkTime = 3;
        timer = 0;
    }

    //----------------------------------------------------------

    void Start()
    {
        GameManager.Instance.OnChickenDeath += OnChickenDeathDelegate;
        GameManager.Instance.OnGenerateNewChicken += OnGenerateNewChickenDelegate;


        ChickenStats[] arrChickensStats = FindObjectsByType<ChickenStats>(FindObjectsSortMode.None);

        //Por cada Pollito en el Arreglo...
        foreach (ChickenStats ch in arrChickensStats)
        {
            //Lo Agregamos a la Lista...
            listChickensStats.Add(ch);
        }

        // El contador de Poblacion empieza con el numero inicial de Gallinas en la Lista
        poblacion = listChickensStats.Count;
        pollitosMuertos = 0;

        //La tasa de mortalidad empieza en 0
        tasaMortalidad = 0;

        CalcularPesoPromedio();

        UpdateStats();

    }

    //----------------------------------------------------------------------------

    void Update()
    {
        //Incrementamos el tiempo constantemente.
        timer += Time.deltaTime;

        //Si el timer llega al tiempo de Check
        if (timer >= checkTime)
        {
            //Calculamos el Peso
            CalcularPesoPromedio();

            //Actualizmaos los stats
            UpdateStats();

            //Timer regresa a 0
            timer = 0;
        }

        
    }

    //----------------------------------------------------------------------------

    private void OnGenerateNewChickenDelegate()
    {
        //Aumentamos la Poblacion
        poblacion++;

        //Calculamos la nueva Mortalidad
        CalcularMortalidad();

        //Calculamos el nuev Peso Promedio
        CalcularPesoPromedio();

        // Actualizamos los Stats
        UpdateStats();
    }

    //----------------------------------------------------------------------------

    private void OnChickenDeathDelegate()
    {
        //Incrementamos la cantidad de Pllitos muertos...
        pollitosMuertos++;

        //Calculamos la nueva Mortalidad
        CalcularMortalidad();

        UpdateStats();
    }

    //----------------------------------------------------------------------------
    // Funcion: Actualizar resumen de Stats

    public void UpdateStats()
    {
        //Actualizamos los Textos resumen de Stats
        txtPesoPrmedio.text = "Peso Promedio: " + pesoPromedio.ToString("F2");
        txtPoblacion.text = "Población: " + poblacion.ToString();
        txtMortalidad.text = "Tasa de Mortalidad: " + tasaMortalidad.ToString("F2") + "%";

    }

    //----------------------------------------------------------------------------
    // Funcion: Calcular Peso Promedio

    public void CalcularPesoPromedio()
    {
        //Inicializamos var de PesoTotal
        float pesoTotal = 0;

        //Por cada stat de Pollito en la Lista
        foreach (ChickenStats ch in listChickensStats)
        {
            //Incrementamos el Peso
            pesoTotal += ch.peso;
        }

        //Obtenemos el peso promedio dividiendo
        pesoPromedio = (pesoTotal / poblacion);
    }

    //----------------------------------------------------------------------------
    // Funcion: Calcular Mortalidad

    public void CalcularMortalidad()
    {
        tasaMortalidad = pollitosMuertos / poblacion * 100.00f;
        Debug.Log(tasaMortalidad);
        Debug.Log(poblacion);
    }

    
}
