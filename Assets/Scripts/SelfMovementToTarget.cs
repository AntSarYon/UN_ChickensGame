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

    #endregion

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    #region Methods

    void Awake()
    {
        //Obtencion de componentges
        mRb = GetComponent<Rigidbody2D>();
    }

    //-----------------------------------------------------------------------------------

    void Start()
    {
        //Definimos un punto de destino aleatorio
        SetNewRandomWaypoint();

        /*
        //Obtenemos el Transform del Chicken principal
        target = GameObject.Find("Chicken").transform;
        */
    }

    //-----------------------------------------------------------------------------------

    void Update()
    {
        //Si hay un Target definido
        if (target)
        {
            //Obtenemos direccion de movimiento (normalizada) hacia el target
            moveDirection = (target.position - transform.position).normalized;

            /*
            // Rotacion del Sprite hacia la direccion (NO SE USAR�, PERO QUIZAS EN UN FUTURO...)

            //Obtencion de Angulo de rotacion en base a la Contangente de la Direccion; y pasando el calculo de radiales a Grados.
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;

            //Asignamos la Rotacion al RigidBody
            mRb.rotation = angle;
            */
        }

        //Si no hay un Target definido...
        else
        {
            //Obtenemos direccion de movimiento (normalizada) hacia el random waypoint
            moveDirection = (randomWaypoint - transform.position).normalized;

            //Nos desplazamos hacia el Waypoint.

            


        }
    }

    //-----------------------------------------------------------------------------------

    void FixedUpdate()
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

    //-----------------------------------------------------------------------------------
    // FUNCION - Definir nuevo destino aleatorio (SIN TARGET)

    public void SetNewRandomWaypoint()
    {
        //Definimos un nuevo Destino
        randomWaypoint = new Vector2(Random.Range(-maxXDistance, maxXDistance+1), Random.Range(-maxYDistance, maxYDistance + 1));
    }

    #endregion


}
