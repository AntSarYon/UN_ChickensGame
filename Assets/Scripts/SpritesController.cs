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

    //Vector con la escala original del pollito
    private Vector3 originalScale;
    private Vector3 draggedScale = new Vector3(0.80f, 0.80f, 0.80f);

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

        //Inicia con el flag de "Siendo arrastrado" en false
        isBeingDragged = false;

        //Funcion Delegafa del Evento "Orden de Dormir"
        DayStatusManager.instance.OnSleepOrderClicked += OnSleepOrderClickedDelegate;
    }

    //-----------------------------------------------------------------------

    private void OnSleepOrderClickedDelegate(bool sleepOrder)
    {
        if (sleepOrder)
        {
            mAnimator.Play("Sit");
        }

        //Si se ha recibido al orden de dormir...
        else if (!sleepOrder)
        {
            //Actualizamos el SleepingFlag en base a si la orden esta activa o no
            mAnimator.SetTrigger("WakeUp");
        }
        
    }

    //-----------------------------------------------------------------------------------

    void Update()
    {
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

    //-----------------------------------------------------------------------------------
    // Funciones: Rotar Sprite

    public void LookAtLeft()
    {
        //El sprite se muestra en su sentido orignal
        mSrenderer.flipX = false;
    }

    public void LookAtRight()
    {
        // Voltemos el Sprite (derecha)
        mSrenderer.flipX = true;
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Cuando El Mouse pasa por encima o sale

    private void OnMouseOver()
    {
        mSrenderer.color = draggedColor;

        //Si NO esta siendo arrastrado
        if (!isBeingDragged)
        {
            //Mostramos la UI de informacion del Pollo
            chickenUI.ShowChickenInfo();
        }
    }

    private void OnMouseExit()
    {
        //Asignamos el color de por defecto;
        mSrenderer.color = defaultColor;

        //Desactivamos la UI de informacion del Pollo
        chickenUI.HideChickenInfo();
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Cuando oprimimos el Click

    private void OnMouseDown()
    {
        //Asignamos el color de agarre;
        mSrenderer.color = draggedColor;

        //Le asignamos la escala de Agarre
        transform.localScale = draggedScale;

        //Activamos Flag de "esta siendo arrastrado"
        isBeingDragged = true;

    }

    //-----------------------------------------------------------------------------------
    // Funcion - Mientras se mantenga el Mouse oprimido y se detecta el arrastre...

    private void OnMouseDrag()
    {
        //Desactivamos la UI de informacion del Pollo
        chickenUI.HideChickenInfo();
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Cuando soltamos el Click

    private void OnMouseUp()
    {
        //Asignamos el color de por defecto;
        mSrenderer.color = defaultColor;

        //Le devolvemos la escala orignal
        transform.localScale = originalScale;

        //Desactivamos Flag de "esta siendo arrastrado"
        isBeingDragged = false;
    }

    //-----------------------------------------------------------------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Si chocamos con otra Gallina...
        if (collision.gameObject.CompareTag("Chicken"))
        {
            //Asignamos el color de agarre;
            mSrenderer.color = fightingColor;

            //Mostramos la UI de la pelea
            chickenUI.ShowFightInfo();
        }

        //Si chocamos con un contenedor de Comida o Agua
        else if (collision.gameObject.CompareTag("Food") || collision.gameObject.CompareTag("Water"))
        {
            //Si el objeto colisionado esta hacia la derecha
            if (collision.transform.position.x > transform.position.x)
            {
                LookAtRight();
            }
            else
            {
                LookAtLeft();
            }
        }
        
    }

    //-----------------------------------------------------------------------------------

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Si ha dejado de chocar con otro pollito
        if (collision.gameObject.CompareTag("Chicken"))
        {
            //Asignamos el color de agarre;
            mSrenderer.color = defaultColor;

            //Ocultamos la UI de la pelea
            chickenUI.HideFightInfo();
        }
            
    }

    #endregion

}
