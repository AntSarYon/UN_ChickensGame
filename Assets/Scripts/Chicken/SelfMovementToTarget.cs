using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfMovementToTarget : MonoBehaviour
{
    #region Props

    //Velocidad de movimiento
    [SerializeField] private float moveSpeed;

    //Direccion del movimiento
    private Vector2 moveDirection;

    //Transform del target
    private Transform target;

    // Desino aleatorio (Para cuando no haya Target)
    private Vector3 randomWaypoint;

    //Rango y distancia para destino aleatorio
    [SerializeField] private float minRange;
    [SerializeField] private float maxXDistance;
    [SerializeField] private float maxYDistance;

    #endregion

    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

    #region Components

    //Componente RigidBody
    private Rigidbody2D mRb;
    private SpritesController mSpritesController;
    private ChickenStats mChickenStats;

    #endregion

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    #region Methods

    void Awake()
    {
        //Obtencion de componentes
        mRb = GetComponent<Rigidbody2D>();
        mSpritesController = GetComponent<SpritesController>();
        mChickenStats = GetComponent<ChickenStats>();
    }

    //-----------------------------------------------------------------------------------

    void Update()
    {
        //Si el Pollito está comiendo, Bebiendo, Durmiendo, o Peleando
        if (mChickenStats.eatingFlag || mChickenStats.drinkingFlag || mChickenStats.sleepingFlag || mChickenStats.fightingFlag)
        {
            //Si el Stat de hambre esta por debajo de 40
            if (mChickenStats.hambre < 40)
            {
                //Desactivamos los Flags de Comiendo y Bebiendo
                mChickenStats.eatingFlag = false;
                mChickenStats.drinkingFlag = false;
            }

            //No hace nada
            return;
        }
        //En caso no est{e haciendo ninguna de esas acciones...
        else
        {
            //Seteamos una direccion para su movimiento
            SetMovementDirection();
        }



        if (Input.GetKeyDown(KeyCode.F))
        {
            target = GameObject.Find("Food").transform;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            target = GameObject.Find("Water").transform;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            target = null;
            //Asignamos un nuevo target aleatorio
            SetNewRandomWaypoint();
        }

    }

    //-----------------------------------------------------------------------------------

    public void SetMovementDirection()
    {
        //Si tiene un Target
        if (target)
        {
            //Obtenemos direccion de movimiento (normalizada) hacia el target
            moveDirection = (target.position - transform.position).normalized;
        }

        //Si no hay un Target definido...
        else
        {
            //Obtenemos direccion de movimiento (normalizada) hacia el random waypoint
            moveDirection = (randomWaypoint - transform.position).normalized;
        }
    }

    //-----------------------------------------------------------------------------------

    void FixedUpdate()
    {
        //Si el Pollito está comiendo, Bebiendo, Durmiendo, o Peleando
        if (mChickenStats.eatingFlag || mChickenStats.drinkingFlag || mChickenStats.sleepingFlag || mChickenStats.fightingFlag)
        {
            //Anulamos la velocidad del pollo
            mRb.velocity = Vector2.zero;

            //No hace nada
            return;
        }
        //En caso no est{e haciendo ninguna de esas acciones...
        else
        {
            //Asignamos Velocidad y direccion en base a los calculos anteriores sobre el destino (Target o Waypoint)
            mRb.velocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;

            //Si no hay un Target (movimiento aleatorio)
            if (!target)
            {
                //Si la distancia entre el Pollito y el Waypoint esta dentro del rango minimo definido;
                if (Vector2.Distance(transform.position, randomWaypoint) < minRange)
                {
                    //cambiamos de Waypoint para que el pollito siga moviendose
                    SetNewRandomWaypoint();
                }
            }
        }

    }

    //-----------------------------------------------------------------------------------
    // FUNCION - Definir nuevo destino aleatorio (SIN TARGET)

    public void SetNewRandomWaypoint()
    {
        //Definimos un nuevo Destino
        randomWaypoint = new Vector2(Random.Range(-maxXDistance, maxXDistance+1), Random.Range(-maxYDistance, maxYDistance + 1));
    }

    #endregion


}
