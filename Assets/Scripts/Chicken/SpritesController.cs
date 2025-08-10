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

    //Flag para reconocer si esta siendo arrastrado
    [SerializeField] private bool isBeingDragged;

    //UI del pollito
    [SerializeField] private ChickenUI chickenUI;

    #endregion

    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

    #region Components

    private SpriteRenderer mSrenderer;
    private Rigidbody2D mRigidbody;
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
        mRigidbody = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        mChickenStats = GetComponent<ChickenStats>();
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

        //Funcion Delegafa del Evento "Orden de Dormir"
        DayStatusManager.instance.OnSleepOrderClicked += OnSleepOrderClickedDelegate;
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
        if (mRigidbody.velocity != Vector2.zero)
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

    public void EnterStarvingAnim()
    {
        //Hacemos que el Pollito este de rojo
        mSrenderer.color = starvingColor;
        
    }

    public void ExitStarvingAnim()
    {
        //Hacemos que el Pollito este normal
        mSrenderer.color = defaultColor;
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
        //Activamos trigger de Muerte
        mAnimator.SetTrigger("Die");

        //Regresamos su Color a la normalidad
        mSrenderer.color = defaultColor;

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
        chickenUI.ShowFightInfo();
    }

    //-----------------------------------------------------------------------------------

    public void ExitFightAnim()
    {
        //Asignamos el color de agarre;
        mSrenderer.color = defaultColor;

        //Ocultamos la UI de la pelea
        chickenUI.HideFightInfo();
    }

    #endregion

}
