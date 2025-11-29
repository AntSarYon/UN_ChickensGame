using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ChickenStats : MonoBehaviour
{

    //Salud del Pollito
    [HideInInspector] public float hp = 100;
    [Header("Velocidad Cambio de Stats: HP")]
    [Range(0.00f, 10.00f)][SerializeField] private float velocidadIncrementoHP = 0;
    [Range(0.00f, 10.00f)][SerializeField] private float velocidadReduccionHP = 10;

    //Hambre del Pollito
    [HideInInspector] public float hambre = 100;
    [Header("Velocidad Cambio de Stats: Hambre")]
    [Range(0.00f, 10.00f)] [SerializeField] private float velocidadIncrementoHambre = 2.75f;
    [Range(0.00f, 10.00f)] [SerializeField] private float velocidadReduccionHambre = 4;

    //felicidad del pollito
    [HideInInspector] public float felicidad = 100;
    [Header("Velocidad Cambio de Stats: felicidad")]
    [Range(0.00f, 10.00f)] [SerializeField] private float velocidadIncrementofelicidad = 1;
    [Range(0.00f, 10.00f)] [SerializeField] private float velocidadReduccionfelicidad = 3;
    [Range(0.00f, 100.00f)] public float felicidadParaPelear = 60;

    //Peso del Pollito
    [HideInInspector] public float peso = 100;
    [Header("Velocidad Cambio de Stats: Peso")]
    [Range(0.00f, 10.00f)] [SerializeField] private float velocidadIncrementoPeso = 0.15f;
    private float multiplicadorIncrementoPesoSegunfelicidad = 1;
    [Range(0.00f, 10.00f)] [SerializeField] private float velocidadReduccionPeso = 0.10f;

    //-----------------------------------------------------------------------

    void Start()
    {
        //Traemos los parametros segun se haya ingresado en el Menu Inicial
        velocidadIncrementoHambre = GameRulesManager.instance.velocidadIncrementoHambre;
        velocidadReduccionHambre = GameRulesManager.instance.velocidadReduccionHambre;

        velocidadIncrementofelicidad = GameRulesManager.instance.velocidadIncrementofelicidad;
        velocidadReduccionfelicidad = GameRulesManager.instance.velocidadReduccionfelicidad;

        velocidadIncrementoPeso = GameRulesManager.instance.velocidadIncrementoPeso;
        velocidadReduccionPeso = GameRulesManager.instance.velocidadReduccionPeso;
        multiplicadorIncrementoPesoSegunfelicidad = 1;

        //Seteamos los stats iniciales del pollo
        hp = 100;
        hambre = Random.Range(35.00f, 80.00f);
        felicidad = Random.Range(35.00f, 80.00f);
        peso = 1; // El Peso empieza en 1 siempre // Random.Range(1.00f, 7.00f);

    }

    //-----------------------------------------------------------------------
    // FUNCION: Manejo de Stats segun estados...

    public void ManageStats_HambreYPeso(bool eatingFlag, bool isbeingDragged)
    {
        if (eatingFlag)
        {
            //Reducimos el Stat de Hambre progresivamente
            hambre -= velocidadReduccionHambre * Time.deltaTime;
            hambre = Mathf.Clamp(hambre, 0.00f, 100.00f);

            //Si esta Estimulado...
            if (GetComponent<ChickenController>().isEstimulated)
            {
                peso += velocidadIncrementoPeso * Time.deltaTime * multiplicadorIncrementoPesoSegunfelicidad * 2;
            }
            else if (GetComponent<ChickenController>().isDisgusted) 
            {
                //peso += velocidadIncrementoPeso * Time.deltaTime * multiplicadorIncrementoPesoSegunfelicidad;
            }
            //Si no hay ningun efecto...
            else
            {
                peso += velocidadIncrementoPeso * Time.deltaTime * multiplicadorIncrementoPesoSegunfelicidad;
            }
                

            peso = Mathf.Clamp(peso, 1.00f, 7.00f);
        }
        //Si el Flag de "Comiendo"; esta desactivado
        else
        {
            //Si esta siendo sujetado...
            if (isbeingDragged)
            {
                //Aumentamos el Stat de Hambre progresivamente (más rapido)
                hambre += velocidadIncrementoHambre * 1.75f * Time.deltaTime;
            }
            else
            {
                //Aumentamos el Stat de Hambre progresivamente
                hambre += velocidadIncrementoHambre * Time.deltaTime;
            }
            

            hambre = Mathf.Clamp(hambre, 0.00f, 100.00f);

            peso -= velocidadReduccionPeso * Time.deltaTime;
            peso = Mathf.Clamp(peso, 1.00f, 7.00f);
        }
    }

    //-----------------------------------------------------------------------

    public void ManageStats_HP(bool starvingFlag)
    {
        //Si el flag de "peleando" esta activo
        if (starvingFlag)
        {
            //Incrementamos la salud Progresivamente
            hp -= velocidadReduccionHP * Time.deltaTime;
            hp = Mathf.Clamp(hp, 0.00f, 100.00f);
        }
    }

    //-----------------------------------------------------------------------

    public void ManageStats_felicidad(bool eatingFlag, bool isBeingDragged)
    {
        // Si el Flag de "Durmiendo" o "Comiendo" esta activo
        if (eatingFlag)
        {
            //Reducimos el felicidad Progresivamente
            felicidad -= velocidadReduccionfelicidad * Time.deltaTime;
            felicidad = Mathf.Clamp(felicidad, 0.00f, 100.00f);

            //Actualizamos el indice de incremento de Peso por felicidad
            UpdateIncrementoPesoPorfelicidad();


        }
        // Si el Flag de "Durmiendo" y "Comiendo" estan Desactivados
        else
        {
            if (isBeingDragged)
            {
                //Aumentamos el felicidad Progresivamente (más rapido)
                felicidad += velocidadIncrementofelicidad * 1.75f * Time.deltaTime;
            }
            else
            {
                //Aumentamos el felicidad Progresivamente
                felicidad += velocidadIncrementofelicidad * Time.deltaTime;
            }

            //Limitamos el felicidad dentro de un rango hasta 100
            felicidad = Mathf.Clamp(felicidad, 0.00f, 100.00f);

            //Actualizamos el indice de incremento de Peso por felicidad
            UpdateIncrementoPesoPorfelicidad();
        }
    }

    //-----------------------------------------------------------------------

    private void UpdateIncrementoPesoPorfelicidad()
    {
        // Mientras mas elevado el felicidad, menos engordará al comer
        if (felicidad <= 20.00f)
        {
            multiplicadorIncrementoPesoSegunfelicidad = 3;
        }
        else if (felicidad <= 50.00f)
        {
            multiplicadorIncrementoPesoSegunfelicidad = 2;
        }
        else if (felicidad <= 80.00f)
        {
            multiplicadorIncrementoPesoSegunfelicidad = 1;
        }
        else
        {
            multiplicadorIncrementoPesoSegunfelicidad = 0.5f;
        }
    }

}
