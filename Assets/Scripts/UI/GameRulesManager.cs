using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameRulesManager : MonoBehaviour
{
    public static GameRulesManager instance;

    //Hambre del Pollito
    [HideInInspector] public float hambre = 100;
    [Header("Velocidad Cambio de Stats: Hambre")]
    [Range(0.00f, 10.00f)] public float velocidadIncrementoHambre = 2f;
    [Range(0.00f, 10.00f)] public float velocidadReduccionHambre = 4;

    //felicidad del pollito
    [HideInInspector] public float felicidad = 100;
    [Header("Velocidad Cambio de Stats: felicidad")]
    [Range(0.00f, 10.00f)] public float velocidadIncrementofelicidad = 1;
    [Range(0.00f, 10.00f)] public float velocidadReduccionfelicidad = 3;

    //Peso del Pollito
    [HideInInspector] public float peso = 100;
    [Header("Velocidad Cambio de Stats: Peso")]
    [Range(0.00f, 10.00f)] public float velocidadIncrementoPeso = 0.15f;
    [Range(0.00f, 10.00f)] public float velocidadReduccionPeso = 0.10f;

    //Velocidad con la que disminuye la comida
    [Range(1, 5)] public float foodDecreaseSpeed = 3;

    //Flag de Parametros guardados
    [HideInInspector] public bool nuevosParametrosGuardados = false;

    // Precios de Ingredientes
    [Range(1, 50)] public int precioHarina = 10;
    [Range(1, 50)] public int precioMaiz = 10;
    [Range(1, 50)] public int precioSoya = 10;
    [Range(1, 50)] public int precioGusanos = 10;

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
