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

    public void ManageStarvingAnim()
    {
        //Si el flag de "Starving" esta activo...
        if (mChickenStats.starvingFlag)
        {
            //Hacemos que el Pollito este de rojo
            mSrenderer.color = starvingColor;
        }
        //else
        //{
        //    //Hacemos que el Pollito este normal
        //    mSrenderer.color = defaultColor;
        //}
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

    public void ManageHoverAnimation()
    {
        //Cambiamos su color al de Dragged...
        mSrenderer.color = draggedColor;

        Debug.Log("Color cambiado a gris");
    }

    public void ManageUnhoverAnimation()
    {
        //Asignamos el color de por defecto;
        mSrenderer.color = defaultColor;
    }


    //-----------------------------------------------------------------------------------
    // Funcion - Cuando oprimimos el Click

    public void ManageDragAnimation()
    {
        //Asignamos el color de agarre;
        mSrenderer.color = draggedColor;
        Debug.Log("Color cambiado a gris");

        //Le asignamos la escala de Agarre
        transform.localScale = draggedScale;
    }

    public void ManageUndragAnimation()
    {
        //Asignamos el color de por defecto;
        mSrenderer.color = defaultColor;

        //Le devolvemos la escala orignal
        transform.localScale = originalScale;
    }


    //-----------------------------------------------------------------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GetComponent<ChickenController>().isAlive)
        {
            //Si chocamos con otra Gallina...
            if (collision.gameObject.CompareTag("Chicken"))
            {
                //Si los Stats del Pollo indican que esta estresado...
                if (mChickenStats.estres > mChickenStats.estresParaPelear)
                {
                    //Asignamos el color de agarre;
                    mSrenderer.color = fightingColor;

                    //Mostramos la UI de la pelea
                    chickenUI.ShowFightInfo();
                }

                //En caso el nivel de estres no esté en el Nivel...
                else
                {
                    //Obtenemos los Stats del pollo con el que yhemos chocado
                    ChickenStats otherChickenStats = collision.gameObject.GetComponent<ChickenStats>();

                    //Revisamos si el Estres del otro Pollo essta en el limite...
                    if (otherChickenStats.estres >= otherChickenStats.estresParaPelear)
                    {
                        //De ser el caso...
                        //Asignamos el color de agarre;
                        mSrenderer.color = fightingColor;

                        //Mostramos la UI de la pelea
                        chickenUI.ShowFightInfo();
                    }
                }

            }

            //Si chocamos con un contenedor de Comida o Agua
            else if (collision.gameObject.CompareTag("Food") || collision.gameObject.CompareTag("Water"))
            {
                //Si empieza a comer o tomar agua, su color se normaliza
                mSrenderer.color = defaultColor;

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
    }

    //-----------------------------------------------------------------------------------

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (GetComponent<ChickenController>().isAlive)
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
        
            
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Food") || collision.gameObject.CompareTag("Water"))
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

    #endregion

}
