using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfMovementToTarget : MonoBehaviour
{
    #region Props

    //Velocidad de movimiento
    [SerializeField] private float moveSpeed;
    private float speedMultiplier;

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
    private ChickenStats mChickenStats;
    private ChickenController mChickenController;

    #endregion

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    #region Methods

    void Awake()
    {
        //Obtencion de componentes
        mRb = GetComponent<Rigidbody2D>();

        //Inicializamos el multiplicador de velocidad en 1
        speedMultiplier = 1;
    }

    //-----------------------------------------------------------------------------------

    void Update()
    {
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

    public void StopMoving()
    {
        //AZsignamos 0 velocidad
        mRb.velocity = Vector2.zero;

        return;
    }

    //------------------------------------------------------------------------------------------------------
    //FUNCION: Moverse hacia el objetivo de Movimiento (independientemente de si es Target o RandomWaypoint)

    public void MoveToTarget()
    {
        //Asignamos Velocidad y direccion en base a los calculos anteriores sobre el destino (Target o Waypoint)
        mRb.velocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed * speedMultiplier;
    }

    //------------------------------------------------------------------------------------------------------
    //FUNCION: Revisa si se necesita un nuevo RandomWaypoint (ya llegó al anterior)

    public void CheckIfNeedNewRandomWaypoint()
    {
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
        //Obtenemos nuevas coordenadas Random...
        float newRandomX = Random.Range(-maxXDistance, maxXDistance + 1);
        float newRandomY = Random.Range(-maxYDistance, maxYDistance + 1);

        //Definimos un nuevo Destino
        randomWaypoint = new Vector2(newRandomX, newRandomY);
    }

    public void SetNewRandomWaypointToRight(float leftXLimit)
    {
        //Obtenemos nuevas coordenadas Random, considirando la limitante de X
        float newRandomX = Random.Range(leftXLimit+0.5f, maxXDistance + 1);
        float newRandomY = Random.Range(-maxYDistance, maxYDistance + 1);

        //Definimos un nuevo Destino
        randomWaypoint = new Vector2(newRandomX, newRandomY);
    }

    public void SetNewRandomWaypointToLeft(float rightLimitX)
    {
        //Obtenemos nuevas coordenadas Random, considirando la limitante de X
        float newRandomX = Random.Range(-maxXDistance, rightLimitX+0.5f);
        float newRandomY = Random.Range(-maxYDistance, maxYDistance + 1);

        //Definimos un nuevo Destino
        randomWaypoint = new Vector2(newRandomX, newRandomY);
    }

    //-----------------------------------------------------------------------------------
    // FUNCION - Modificamos la velocidad de movimiento temporalmente
    public void MultiplySpeedTemporary(float timeForRun)
    {
        //Modificamos el multiplicador de velocidad a 3
        speedMultiplier = 3.25f;

        //Devolveremos la velocidad a la normalidad tras haber pasado X segundos.
        Invoke(nameof(SetSpeedMultiplierBackToNormal), timeForRun);
    }

    //-----------------------------------------------------------------------------------
    // FUNCION - Devolver el multiplicador de velocidad a 1 
    public void SetSpeedMultiplierBackToNormal()
    {
        //Devolvemos el multiplicador de velocidad a 1
        speedMultiplier = 1;
    }

    #endregion


}
