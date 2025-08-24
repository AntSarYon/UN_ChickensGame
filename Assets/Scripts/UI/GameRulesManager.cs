using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRulesManager : MonoBehaviour
{
    public static GameRulesManager instance;

    //Salud del Pollito
    [HideInInspector] public float hp = 100;
    [Header("Velocidad Cambio de Stats: HP")]
    [Range(0.00f, 10.00f)] public float velocidadIncrementoHP = 0;
    [Range(0.00f, 10.00f)] public float velocidadReduccionHP = 5;

    //Hambre del Pollito
    [HideInInspector] public float hambre = 100;
    [Header("Velocidad Cambio de Stats: Hambre")]
    [Range(0.00f, 10.00f)] public float velocidadIncrementoHambre = 2f;
    [Range(0.00f, 10.00f)] public float velocidadReduccionHambre = 4;

    //Estres del pollito
    [HideInInspector] public float estres = 100;
    [Header("Velocidad Cambio de Stats: Estres")]
    [Range(0.00f, 10.00f)] public float velocidadIncrementoEstres = 1;
    [Range(0.00f, 10.00f)] public float velocidadReduccionEstres = 3;
    [Range(0.00f, 100.00f)] public float estresParaPelear = 65;

    //Peso del Pollito
    [HideInInspector] public float peso = 100;
    [Header("Velocidad Cambio de Stats: Peso")]
    [Range(0.00f, 10.00f)] public float velocidadIncrementoPeso = 0.15f;
    [Range(0.00f, 10.00f)] public float velocidadReduccionPeso = 0.10f;

    //Velocidad con la que disminuye la comida
    [Range(1, 5)] public float foodDecreaseSpeed = 3;

    [Header("Hora de inicio del dia")]
    [Range(0, 24)] public int startingTime = 9;

    [Header("Hora de inicio del dia")]
    [Range(0, 24)] public int finishTime = 18;

    [Header("Cuan rápido pasa el tiempo")]
    [Range(1, 5)] public float timeScale = 2.00f;

    //Flag de Parametros guardados
    [HideInInspector] public bool nuevosParametrosGuardados = false;

    //-----------------------------------------------------------

    void Awake()
    {
        // Inicialmente No hay parametros (nuevos) guardados
        nuevosParametrosGuardados = false;

        //Manejamos unica instancia del Singleton
        ManageInstance();
        
    }

    //-----------------------------------------------------------

    private void ManageInstance()
    {
        //Si no hay instancia
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

}
