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

    private Vector3 originalPos;
    private Vector3 targetPos;
    private float interpolation;
    private float translationSpeed = 5.00f;
    private bool mustMove = false;

    //COMPONENTES
    private Animator mAnimator;

    // ------------------------------------------------------------------------

    void Awake()
    {
        //Obteneomps referencia a componentes
        mAnimator = GetComponent<Animator>();

        // Flag de "debe moverse"
        mustMove = false;
    }

    // ------------------------------------------------------------------------

    void Start()
    {
        //Asignamos Delegados a Eventos

        // Evento de Pollo Vendido
        DayStatusManager.Instance.OnChickenSold += OnChickenSoldDelegate;
        // Evento de Cambio de Corral
        YardsManager.instance.OnCurrentYardChanged += OnCurrentYardChangedDelegate;

        // Variables de depslazamiento
        originalPos = transform.position;
        targetPos = transform.position;

        interpolation = 0;

    }

    // ------------------------------------------------------------------------

    private void OnCurrentYardChangedDelegate(Yard targetYard)
    {
        //Actualizamos la Posicion Target
        targetPos = targetYard.PosToCamera;
        
        // Activamos flag de "debe moverse"
        mustMove = true;
    }

    // ------------------------------------------------------------------------

    private void OnChickenSoldDelegate(float chickenValue)
    {
        //Reproducimos Animacion de CashoutShake
        mAnimator.Play("CashoutShake");
    }

    // ------------------------------------------------------------------------

    void Update()
    {
        // Si el flag de "debe moverse" esta activo...
        if (mustMove)
        {
            //  Mientras el valor de interpolacion no llega al objetivo...
            if (interpolation < 1)
            {
                // Aumentamos el valor de interpolacion
                interpolation += Time.deltaTime * translationSpeed;

                // Actualizamos la posicion en base al valor de interpolacion
                transform.position = Vector3.Lerp(originalPos, targetPos, interpolation);
            }

            // En caso ya se haya llegado a la posicion destino (interp = 1)
            else
            {
                // Desactivamos Flag de "debe moverse"
                mustMove = false;

                // Actualizamos el nuevo punto de origen
                originalPos = targetPos;

                // Devolvemos el valor de interpolacion a 0
                interpolation = 0;
            }
        }
        

        /*
        //Almacenamos la posicion actual
        Vector3 position = transform.position;

        //Si el Mouse esta cerca al Borde (superior) de la Pantalla (segun definimos el margen)
        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            //Actualizamos la referencia a la posicion en base a la velocidad
            position.y += panSpeed * Time.deltaTime;
        }
        //Si el Mouse esta cerca al Borde (inferior) de la Pantalla (segun definimos el margen)
        if (Input.mousePosition.y <= panBorderThickness)
        {
            //Actualizamos la referencia a la posicion en base a la velocidad
            position.y -= panSpeed * Time.deltaTime;
        }
        //Si el Mouse esta cerca al Borde (superior) de la Pantalla (segun definimos el margen)
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            //Actualizamos la referencia a la posicion en base a la velocidad
            position.x += panSpeed * Time.deltaTime;
        }
        //Si el Mouse esta cerca al Borde (superior) de la Pantalla (segun definimos el margen)
        if (Input.mousePosition.x <= panBorderThickness)
        {
            //Actualizamos la referencia a la posicion en base a la velocidad
            position.x -= panSpeed * Time.deltaTime;
        }

        //Limitamos la posicion de la Camara en los ejes solo hasta cierta posicion
        position.x = Mathf.Clamp(position.x, -panLimit.x, panLimit.x);
        position.y = Mathf.Clamp(position.y, -panLimit.y, panLimit.y);

        //Asignamos la nueva posicion
        //transform.position = position;*/
    }
}
