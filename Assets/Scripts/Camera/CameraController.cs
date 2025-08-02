using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Velocidad desplazamiento Camara")]
    [SerializeField] private float panSpeed;

    [Header("Margen para desplazamiento")]
    [SerializeField] private float panBorderThickness;

    [Header("Limites De Pantalla")]
    [SerializeField] private Vector2 panLimit;

    //COMPONENTES
    private Animator mAnimator;

    // ------------------------------------------------------------------------

    void Awake()
    {
        //Obteneomps referencia a componentes
        mAnimator = GetComponent<Animator>();
    }

    // ------------------------------------------------------------------------

    void Start()
    {
        //Asignamos Delegado al Evento de Pollo Vendido
        GameManager.Instance.OnChickenSold += OnChickenSoldDelegate;
    }

    // ------------------------------------------------------------------------

    private void OnChickenSoldDelegate(int chickenValue)
    {
        //Reproducimos Animacion de CashoutShake
        mAnimator.Play("CashoutShake");
    }

    // ------------------------------------------------------------------------

    void Update()
    {
        //Almacenamos la posicion actual
        Vector3 position = transform.position;

        //Si el Mouse esta cerca al Borde (superior) de la Pantalla (segun definimos el margen)
        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            Debug.Log("Mouse en el limite");
            //Actualizamos la referencia a la posicion en base a la velocidad
            position.y += panSpeed * Time.deltaTime;
        }
        //Si el Mouse esta cerca al Borde (inferior) de la Pantalla (segun definimos el margen)
        if (Input.mousePosition.y <= panBorderThickness)
        {
            Debug.Log("Mouse en el limite");
            //Actualizamos la referencia a la posicion en base a la velocidad
            position.y -= panSpeed * Time.deltaTime;
        }
        //Si el Mouse esta cerca al Borde (superior) de la Pantalla (segun definimos el margen)
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            Debug.Log("Mouse en el limite");
            //Actualizamos la referencia a la posicion en base a la velocidad
            position.x += panSpeed * Time.deltaTime;
        }
        //Si el Mouse esta cerca al Borde (superior) de la Pantalla (segun definimos el margen)
        if (Input.mousePosition.x <= panBorderThickness)
        {
            Debug.Log("Mouse en el limite");
            //Actualizamos la referencia a la posicion en base a la velocidad
            position.x -= panSpeed * Time.deltaTime;
        }

        //Limitamos la posicion de la Camara en los ejes solo hasta cierta posicion
        position.x = Mathf.Clamp(position.x, -panLimit.x, panLimit.x);
        position.y = Mathf.Clamp(position.y, -panLimit.y, panLimit.y);

        //Asignamos la nueva posicion
        transform.position = position;
    }
}
