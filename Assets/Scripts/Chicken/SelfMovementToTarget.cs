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
    private Vector3 moveDirection;

    //Transform del target
    [HideInInspector] public Transform target;

    // Desino aleatorio (Para cuando no haya Target)
    private Vector3 randomWaypoint;

    //Rango y distancia para destino aleatorio
    [SerializeField] private float minRange;
    public float maxXDistanceToRight;
    public float maxXDistanceToLeft;
    public float maxZDistanceToTop;
    public float maxZDistanceToBottom;

    #endregion

    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

    #region Components

    //Componente RigidBody
    private Rigidbody mRb;
    private ChickenStats mChickenStats;
    private ChickenController mChickenController;

    #endregion

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    #region Methods

    void Awake()
    {
        //Obtencion de componentes
        mRb = GetComponent<Rigidbody>();

        //Inicializamos el multiplicador de velocidad en 1
        speedMultiplier = 1;
    }

    // -----------------------------------------------------------------------------------

    void Start()
    {
        //Tratamos de obtener el Corral al que pertenece este pollito
        Yard currentYard = GetComponent<ChickenController>().assignedYard;

        //En caso de ya empezar en un Corral (Pollo no spawneado),
        if (currentYard != null)
        {
            //asignamos los limites de movimiento de este Corral
            maxXDistanceToLeft = currentYard.LeftLimit;
            maxXDistanceToRight = currentYard.RightLimit;
            maxZDistanceToBottom = currentYard.BottomLimit;
            maxZDistanceToTop = currentYard.TopLimit;
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
        mRb.velocity = new Vector3(
            0,
            mRb.velocity.y,
            0
            );

        return;
    }

    //------------------------------------------------------------------------------------------------------
    //FUNCION: Moverse hacia el objetivo de Movimiento (independientemente de si es Target o RandomWaypoint)

    public void MoveToTarget()
    {
        //Asignamos Velocidad y direccion en base a los calculos anteriores sobre el destino (Target o Waypoint)
        mRb.velocity = new Vector3(
            moveDirection.x * moveSpeed * speedMultiplier, 
            mRb.velocity.y, 
            moveDirection.z * moveSpeed * speedMultiplier
            );
    }

    //------------------------------------------------------------------------------------------------------
    //FUNCION: Revisa si se necesita un nuevo RandomWaypoint (ya lleg� al anterior)

    public void CheckIfNeedNewRandomWaypoint()
    {
        //Si no hay un Target (movimiento aleatorio)
        if (!target)
        {
            //Si la distancia entre el Pollito y el Waypoint esta dentro del rango minimo definido;
            if (Vector3.Distance(transform.position, randomWaypoint) < minRange)
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
        float newRandomX = Random.Range(maxXDistanceToLeft, maxXDistanceToRight + 1);
        float newRandomZ = Random.Range(maxZDistanceToBottom, maxZDistanceToTop + 1);

        //Definimos un nuevo Destino
        randomWaypoint = new Vector3(newRandomX, 0.5f, newRandomZ);
    }

    public void SetNewRandomWaypointToRight(float leftXLimit)
    {
        //Obtenemos nuevas coordenadas Random, considirando la limitante de X
        float newRandomX = Random.Range(leftXLimit+0.5f, maxXDistanceToRight + 1);
        float newRandomZ = Random.Range(maxZDistanceToBottom, maxZDistanceToTop + 1);

        //Definimos un nuevo Destino
        randomWaypoint = new Vector3(newRandomX, 0.5f, newRandomZ);
    }

    public void SetNewRandomWaypointToLeft(float rightLimitX)
    {
        //Obtenemos nuevas coordenadas Random, considirando la limitante de X
        float newRandomX = Random.Range(maxXDistanceToLeft, rightLimitX+0.5f);
        float newRandomZ = Random.Range(maxZDistanceToBottom, maxZDistanceToTop + 1);

        //Definimos un nuevo Destino
        randomWaypoint = new Vector3(newRandomX, 0.5f, newRandomZ);
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

    //-----------------------------------------------------------------------------------
    // FUNCION - Escapar en una dirección específica (para cuando el jugador aplaude)
    public void EscapeInDirection(Vector3 escapeDirection, float escapeDistance = 8f)
    {
        //Normalizamos la dirección
        escapeDirection = escapeDirection.normalized;

        //Calculamos el punto hacia donde escapar
        Vector3 escapePoint = transform.position + escapeDirection * escapeDistance;

        //Limitamos el punto a los límites del corral
        escapePoint.x = Mathf.Clamp(escapePoint.x, maxXDistanceToLeft, maxXDistanceToRight);
        escapePoint.z = Mathf.Clamp(escapePoint.z, maxZDistanceToBottom, maxZDistanceToTop);

        //Seteamos ese punto como nuestro waypoint
        randomWaypoint = escapePoint;

        //Asignamos la dirección de movimiento hacia ese punto
        moveDirection = (randomWaypoint - transform.position).normalized;
    }

    //-----------------------------------------------------------------------------------
    // FUNCION - Mover hacia un punto específico (usada para agruparse por temperatura)
    public void MoveToPoint(Vector3 point)
    {
        // Clamp point a límites del corral
        Vector3 clamped = point;
        clamped.x = Mathf.Clamp(clamped.x, maxXDistanceToLeft, maxXDistanceToRight);
        clamped.z = Mathf.Clamp(clamped.z, maxZDistanceToBottom, maxZDistanceToTop);

        randomWaypoint = clamped;
        moveDirection = (randomWaypoint - transform.position).normalized;
    }

    #endregion


}
