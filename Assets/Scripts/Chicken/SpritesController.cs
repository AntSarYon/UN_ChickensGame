using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpritesController : MonoBehaviour
{
    #region Props

    //Colores para cuando se interactua con el pollo
    private Color defaultColor;
    private Color targetedColor = Color.gray;

    //UI del pollito
    [SerializeField] private ChickenUI chickenUI;


    #endregion

    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

    #region Components

    private SpriteRenderer mSrenderer;
    private Rigidbody mRigidbody;
    private Animator mAnimator;
    private ChickenController chickController;

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

        // Obtenemos referencia al Chicken Controller (principal)
        chickController = GetComponent<ChickenController>();
    }

    //-----------------------------------------------------------------------------------

    void Start()
    {
        //Almacenamos el color original del Pollito
        defaultColor = mSrenderer.color;

        //Asignacion de color Gris para cuando esta siendo sujetado
        targetedColor = Color.gray;

    }

    // -------------------------------------------------------------

    void Update()
    {
        //Controlamos la animacion de Caminar
        ManageWalkingAnim();
    }

    //-----------------------------------------------------------------------------------
    // FUNCION: Conntrolar animacion de dormir

    public void SetSleeping(bool sleep)
    {
        //Dependiendo del parametro ingresado, activamos / desactivamos la animacion de Sleep
        if (sleep)
        {
            mAnimator.SetTrigger("GoToSleep");
            mAnimator.SetBool("Sleeping", true);
        }
        else
        {
            mAnimator.SetTrigger("WakeUp");
            mAnimator.SetBool("Sleeping", false);
        }
    }

    public void SetEating(bool eat)
    {
        //Dependiendo del parametro ingresado, activamos / desactivamos la animacion de Sleep
        if (eat)
        {
            mAnimator.SetBool("IsEating", true);
        }
        else
        {
            mAnimator.SetBool("IsEating", false);
        }
    }

    //-----------------------------------------------------------------------------------

    public void ManageWalkingAnim()
    {
        //Si el Flag de "caminando" esta activo
        if (chickController.bIsWalking)
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

        //Si el Flag de "caminando" esta Desactivado
        else
        {
            //Desactivamos flag de animacion 'Is Walking'
            mAnimator.SetBool("IsWalking", false);
        }
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
        //Regresamos su Color a la normalidad
        SetDefaultColor();

        //Activamos trigger de Muerte
        mAnimator.SetTrigger("Die");
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Controlar Animacion de Targeted

    public void SetTargetedColor()
    {
        //Asignamos el color de agarre;
        mSrenderer.color = targetedColor;
    }

    //-----------------------------------------------------------------------------------

    public void SetDefaultColor()
    {
        //Asignamos el color de por defecto;
        mSrenderer.color = defaultColor;
    }

    // -------------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider collision)
    {
        //Si el Triger al que entramos es la zona de interacci�n
        if (collision.tag == "PlayerInteractionZone")
        {
            //Controlamos la animacion de cuando se hace Hover
            SetTargetedColor();
        }
    }

    // ------------------------------------------------------------------------------------

    private void OnTriggerExit(Collider collision)
    {

        //Si el Triger del que salimos es la zona de interacci�n
        if (collision.tag == "PlayerInteractionZone")
        {
            //Controlamos la animacion de cuando se hace Hover
            SetDefaultColor();

        }
    }

    #endregion

}
