using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfMovement : MonoBehaviour
{

    #region Props

    //Velocidad de movimiento
    [SerializeField] private float moveSpeed;

    //Transform del destino
    private Transform target;

    //Direccion del movimiento
    private Vector2 moveDirection;

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
        //Obtenemos el Transform del Chicken principal
        target = GameObject.Find("Chicken").transform;
    }

    //-----------------------------------------------------------------------------------

    void Update()
    {
        //Si hay un Target definido
        if (target)
        {
            //Obtenemos direccion de movimiento hacia el target
            //(normalizada; pues solo me interesa la direccion; no la distancia)
            moveDirection = (target.position - transform.position).normalized;

            /*
            // Rotacion del Sprite hacia la direccion (NO SE USARÁ, PERO QUIZAS EN UN FUTURO...)

            //Obtencion de Angulo de rotacion en base a la Contangente de la Direccion; y pasando el calculo de radiales a Grados.
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;

            //Asignamos la Rotacion al RigidBody
            mRb.rotation = angle;
            */
        }
    }

    //-----------------------------------------------------------------------------------

    void FixedUpdate()
    {
        //Asignamos Velocidad y direccion en base a los calculos anteriores
        mRb.velocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
    }

    #endregion


}
