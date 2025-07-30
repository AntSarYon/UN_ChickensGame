using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenStats : MonoBehaviour
{
    //Flags de Estado del Pollito
    [HideInInspector] public bool eatingFlag = false;
    [HideInInspector] public bool drinkingFlag = false;
    [HideInInspector] public bool stressfulFlag = false;
    [HideInInspector] public bool fightingFlag = false;
    [HideInInspector] public bool sleepingFlag = false;

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

    //Estres del pollito
    [HideInInspector] public float estres = 100;
    [Header("Velocidad Cambio de Stats: Estres")]
    [Range(0.00f, 10.00f)] [SerializeField] private float velocidadIncrementoEstres = 1;
    [Range(0.00f, 10.00f)] [SerializeField] private float velocidadReduccionEstres = 3;
    [Range(0.00f, 100.00f)] public float estresParaPelear = 60;

    //Peso del Pollito
    [HideInInspector] public float peso = 100;
    [Header("Velocidad Cambio de Stats: Peso")]
    [Range(0.00f, 10.00f)] [SerializeField] private float velocidadIncrementoPeso = 0.15f;
    [Range(0.00f, 10.00f)] [SerializeField] private float velocidadReduccionPeso = 0.10f;

    //-----------------------------------------------------------------------

    void Start()
    {
        //Traemos los parametros segun se haya ingresado en el Menu Inicial
        velocidadReduccionHP = GameRulesManager.instance.velocidadReduccionHP;

        velocidadIncrementoHambre = GameRulesManager.instance.velocidadIncrementoHambre;
        velocidadReduccionHambre = GameRulesManager.instance.velocidadReduccionHambre;

        velocidadIncrementoEstres = GameRulesManager.instance.velocidadIncrementoEstres;
        velocidadReduccionEstres = GameRulesManager.instance.velocidadReduccionEstres;
        estresParaPelear = GameRulesManager.instance.estresParaPelear;

        velocidadIncrementoPeso = GameRulesManager.instance.velocidadIncrementoPeso;
        velocidadReduccionPeso = GameRulesManager.instance.velocidadReduccionPeso;

        //Seteamos los stats iniciales del pollo
        hp = 100;
        hambre = Random.Range(35.00f, 80.00f);
        estres = Random.Range(35.00f, 80.00f);
        peso = Random.Range(2.00f, 7.00f);

        //Funcion Delegafa del Evento "Orden de Dormir"
        DayStatusManager.instance.OnSleepOrderClicked += OnSleepOrderClickedDelegate;
    }

    //-----------------------------------------------------------------------

    private void OnSleepOrderClickedDelegate(bool sleepOrder)
    {
        //Actualizamos el SleepingFlag en base a si la orden esta activa o no
        sleepingFlag = sleepOrder;
    }

    //-----------------------------------------------------------------------

    void Update()
    {
        //Si el Flag de "Comiendo"; esta Activado
        if (eatingFlag)
        {
            //Reducimos el Stat de Hambre progresivamente
            hambre -= velocidadReduccionHambre * Time.deltaTime;
            hambre = Mathf.Clamp(hambre, 0.00f, 100.00f);

            peso += velocidadIncrementoPeso * Time.deltaTime;
            peso = Mathf.Clamp(peso, 2.00f, 7.00f);
        }
        //Si el Flag de "Comiendo"; esta desactivado
        else
        {
            //Aumentamos el Stat de Hambre progresivamente
            hambre += velocidadIncrementoHambre * Time.deltaTime;
            hambre = Mathf.Clamp(hambre, 0.00f, 100.00f);

            peso -= velocidadReduccionPeso * Time.deltaTime;
            peso = Mathf.Clamp(peso, 2.00f, 7.00f);
        }


        //Si el flag de "peleando" esta activo
        if (fightingFlag)
        {
            //Incrementamos la salud Progresivamente
            hp -= velocidadReduccionHP * Time.deltaTime;
            hp = Mathf.Clamp(hp, 0.00f, 100.00f);
        }

        // Si el Flag de "Durmiendo" esta activo
        if (sleepingFlag)
        {
            //Reducimos el estres Progresivamente
            estres -= velocidadReduccionEstres * Time.deltaTime;
            estres = Mathf.Clamp(estres, 0.00f, 100.00f);
        }
        // Si el Flag de "Durmiendo" esta Desactivo
        else
        {
            //Aumentamos el estres Progresivamente
            estres += velocidadIncrementoEstres * Time.deltaTime;
            estres = Mathf.Clamp(estres, 0.00f, 100.00f);
        }

    }

    //-----------------------------------------------------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Si el objeto con el que colisionamos es otro Pollito
        if (collision.gameObject.CompareTag("Chicken"))
        {
            //Si el nivel de Estres esta por encima del nivel definido para las Peleas...
            if (estres > estresParaPelear)
            {
                //Activamos Flag de "Esta peleando"
                fightingFlag = true;
            }
            //En caso el nivel de estres no esté en el Nivel...
            else
            {
                //Obtenemos los Stats del pollo con el que yhemos chocado
                ChickenStats otherChickenStats = collision.gameObject.GetComponent<ChickenStats>();

                //Revisamos si el Estres del otro Pollo essta en el limite...
                if (otherChickenStats.estres >= estresParaPelear)
                {
                    //De ser el caso...
                    //Activamos Flag de "Esta peleando"
                    fightingFlag = true;
                }
                else
                {
                    //Hacemos que se asigne un nuevo TargetRandom
                    GetComponent<SelfMovementToTarget>().SetNewRandomWaypoint();
                }
            }

        }

        //Si el objeto con el que colisionamos es otro Pollito
        else if (collision.gameObject.CompareTag("Food"))
        {
            //Si tiene hambre...
            if (hambre > 40)
            {
                //Activamos Flag de "Esta comiendeo"
                eatingFlag = true;
            }
            else
            {
                //Seteamos un nuevo target de movimiento random
                GetComponent<SelfMovementToTarget>().SetNewRandomWaypoint();

                //Desactivamos Flag de "Esta comiendeo"
                eatingFlag = false;
            }
            
        }

        //Si el objeto con el que colisionamos es otro Pollito
        else if (collision.gameObject.CompareTag("Water"))
        {
            //Si tiene hambre...
            if (hambre > 40)
            {
                //Activamos Flag de "Esta peleando"
                drinkingFlag = true;
            }
            else
            {
                //Seteamos un nuevo target de movimiento random
                GetComponent<SelfMovementToTarget>().SetNewRandomWaypoint();

                //Desactivamos Flag de "Esta comiendeo"
                drinkingFlag = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Si el objeto con el que colisionamos es otro Pollito
        if (collision.gameObject.CompareTag("Chicken"))
        {
            //Desactivamos Flag de "Esta peleando"
            fightingFlag = false;
        }

        //Si el objeto con el que colisionamos es otro Pollito
        else if (collision.gameObject.CompareTag("Food"))
        {
            //Desactivamos Flag de "Esta peleando"
            eatingFlag = false;
        }

        //Si el objeto con el que colisionamos es otro Pollito
        else if (collision.gameObject.CompareTag("Water"))
        {
            //Desactivamos Flag de "Esta peleando"
            drinkingFlag = false;
        }
    }
}
