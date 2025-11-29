using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesController : MonoBehaviour
{
    #region Props

    //Colores para cuando se interactua con el pollo
    private Color defaultColor;
    private Color draggedColor = Color.gray;
    private Color fightingColor = Color.red;
    private Color starvingColor = Color.red;

    //Vector con la escala original del pollito
    private Vector3 originalScale;
    private Vector3 draggedScale = new Vector3(1.35f, 1.35f, 1.35f);

    //UI del pollito
    [SerializeField] private ChickenUI chickenUI;

    // Flag Modo starving
    private bool bStarvingModeOn;

    private float starvingInterpolation = 0;
    private float starvingSpeed = 2;
    // Flag de "Debe volverse volviendose rojo"
    private bool mustTurnRed = false;

    #endregion

    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

    #region Components

    private SpriteRenderer mSrenderer;
    private Rigidbody mRigidbody;
    private Animator mAnimator;
    private ChickenStats mChickenStats;

    #endregion

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    #region Methods 

    void Awake()
    {
        //Obtenemos componentes
        mSrenderer = GetComponent<SpriteRenderer>();
        mRigidbody = GetComponent<Rigidbody>();
        mAnimator = GetComponent<Animator>();
        mChickenStats = GetComponent<ChickenStats>();

        // El flag de "modo starving" empieza en false
        bStarvingModeOn = false;
        starvingInterpolation = 0;
        starvingSpeed = 2;
        mustTurnRed = false;
    }

    //-----------------------------------------------------------------------------------

    void Start()
    {
        //Almacenamos el color original del Pollito
        defaultColor = mSrenderer.color;

        //Asignacion de color Gris para cuando esta siendo sujetado
        draggedColor = Color.gray;

        //Almacenamos la escala original del pollito
        originalScale = transform.localScale;

    }

    void Update()
    {
        // Controlamos (si se requiere) la animacion de starving
        ControlStarvingAnim();
                
    }

    //----------------------------------------------------------------------------------------
    // FUNCION: Controlar la Animacion de Starving

    private void ControlStarvingAnim()
    {
        //Si el "Modo Starving" esta activo...
        if (bStarvingModeOn)
        {
            //Si el flag de "Debe cambiar a Rojo" esta activo
            if (mustTurnRed)
            {
                // Si la interpolacion aun es menor a 1 (no llega a rojo)
                if (starvingInterpolation < 1)
                {
                    //Incrementamos el valor de interpolacion
                    starvingInterpolation += Time.deltaTime * starvingSpeed;
                }
                //En caso ya haya llegado a Rojo...
                else
                {
                    //Desactivamos Flag de "Debe hacerse rojo"
                    mustTurnRed = false;
                }
            }
            // En caso el flag este desactivado
            else
            {
                // Si el valor de interpolacion aun es mayor que 0
                if (starvingInterpolation > 0)
                {
                    // Reducimos el valor de interpolacion
                    starvingInterpolation -= Time.deltaTime * starvingSpeed;
                }
                // En caso ya este en 0
                else
                {
                    // Activamos Flag de "Debe hacerse rojo"
                    mustTurnRed = true;
                }
            }

            //Actualizamos el Color seg{un corresponda
            mSrenderer.color = Color.Lerp(defaultColor, starvingColor, starvingInterpolation);
        }

        //En caso No estar en "Modo Starving"
        else
        {
            //El valor de interpolacion vuelve a 0
            starvingInterpolation = 0;

            //Activamos Flag de "Debe converitrse en Rojo" para cuando se requiera nuevamente
            mustTurnRed = true;
        }
    }

    //----------------------------------------------------------------------------------------
    // FUNCION DELEGADA: Dependiendo de la Orden de dormir; se activa o desactiva la Animacion

    private void OnSleepOrderClickedDelegate(bool sleepOrder)
    {
        if (sleepOrder)
        {
            mAnimator.SetTrigger("GoToSleep");
            mAnimator.SetBool("Sleeping", true);
        }

        //Si se ha recibido al orden de dormir...
        else if (!sleepOrder)
        {
            //Actualizamos el SleepingFlag en base a si la orden esta activa o no
            mAnimator.SetTrigger("WakeUp");
            mAnimator.SetBool("Sleeping", false);
        }
        
    }

    //-----------------------------------------------------------------------------------

    public void ManageWalkingAnim()
    {
        //Si el Pollito tiene Velocidad en su RB
        if (!GetComponent<ChickenController>().isBeingDragged && mRigidbody.velocity != Vector3.zero)
        {
            //Activams flag de animacion 'Is Walking'
            mAnimator.SetBool("IsWalking", true);

            //Si el movimiento se esta dando hacia la derecha...
            if (mRigidbody.velocity.x > 0)
            {
                LookAtRight();
            }
            //Si el movimiento se esta dando hacia la izquierda...
            else if (mRigidbody.velocity.x < 0)
            {
                LookAtLeft();
            }
        }

        //En caso de que la Velocidad si sea igual a 0...
        else
        {
            //Desactivamos flag de animacion 'Is Walking'
            mAnimator.SetBool("IsWalking", false);
        }
    }

    //-----------------------------------------------------------------------------------

    public void EnableStarvingAnim()
    {
        // Activamos el flag de Modo Starving
        bStarvingModeOn = true;
    }

    public void DisableStarvingAnim()
    {
        // Desactivamos el flag de Modo Starving
        bStarvingModeOn = false;
    }

    //-----------------------------------------------------------------------------------
    // FUNCION: Mirar en Direccion de un Target

    public void LookAtTarget(Vector3 viewTarget)
    {
        // Si el objetivo esta hacia la derecha
        if (viewTarget.x > transform.position.x)
        {
            LookAtRight();
        }

        //Si el Objetivo esta hacia la izquierda
        else
        {
            LookAtLeft();
        }
    }

    //-----------------------------------------------------------------------------------
    // Funciones: Rotar Sprite

    public void LookAtLeft()
    {
        //El sprite se muestra en su sentido orignal
        mSrenderer.flipX = true;
    }

    public void LookAtRight()
    {
        // Voltemos el Sprite (derecha)
        mSrenderer.flipX = false;
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Reproducir Muerte

    public void PlayDeath()
    {
        // Desactivamos la Animacion de "Starving"
        DisableStarvingAnim();

        //Regresamos su Color a la normalidad
        mSrenderer.color = defaultColor;

        //Activamos trigger de Muerte
        mAnimator.SetTrigger("Die");

        //Desactivamos la UI de informacion del Pollo
        chickenUI.HideChickenInfo();
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Controlar Animacion de Dragged 

    public void EnterHoverAnimation()
    {
        //Cambiamos su color al de Dragged...
        mSrenderer.color = draggedColor;
    }

    public void ExitHoverAnimation()
    {
        //Asignamos el color de por defecto;
        mSrenderer.color = defaultColor;
    }


    //-----------------------------------------------------------------------------------
    // Funcion - Cuando oprimimos el Click

    public void EnterDragAnimation()
    {
        //Asignamos el color de agarre;
        mSrenderer.color = draggedColor;

        //Le asignamos la escala de Agarre
        transform.localScale = draggedScale;
    }

    //-----------------------------------------------------------------------------------

    public void SetSpriteBackToNormal()
    {
        //Asignamos el color de por defecto;
        mSrenderer.color = defaultColor;

        //Le devolvemos la escala orignal
        transform.localScale = originalScale;
    }

    //-----------------------------------------------------------------------------------

    public void EnterFightAnim()
    {
        //Asignamos el color de agarre;
        mSrenderer.color = fightingColor;

        //Mostramos la UI de la pelea
        //chickenUI.ShowFightInfo();
    }

    //-----------------------------------------------------------------------------------

    public void ExitFightAnim()
    {
        //Asignamos el color de agarre;
        mSrenderer.color = defaultColor;

        //Ocultamos la UI de la pelea
        //chickenUI.HideFightInfo();
    }

    #endregion

}
