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

    #endregion

    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

    #region Components

    private SpriteRenderer mSrenderer;
    private Rigidbody2D mRigidbody;

    #endregion

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    #region Methods 

    void Awake()
    {
        //Obtenemos componentes
        mSrenderer = GetComponent<SpriteRenderer>();
        mRigidbody = GetComponent<Rigidbody2D>();
    }

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
        //Si el movimiento se esta dando hacia la derecha...
        if (mRigidbody.velocity.x > 0)
        {
            LookAtRight();
        }
        //Si el movimiento se esta dando hacia la izquierda...
        else
        {
            LookAtLeft();
        }
    }

    //-----------------------------------------------------------------------------------

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
    // Funcion - Cuando oprimimos el Click

    private void OnMouseDown()
    {
        //Asignamos el color de agarre;
        mSrenderer.color = draggedColor;

        //Le asignamos la escala de Agarre
        transform.localScale = draggedScale;
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Mientras se mantenga el Mouse oprimido y se detecta el arrastre...

    private void OnMouseDrag()
    {

    }

    //-----------------------------------------------------------------------------------
    // Funcion - Cuando soltamos el Click

    private void OnMouseUp()
    {
        //Asignamos el color de por defecto;
        mSrenderer.color = defaultColor;

        //Le devolvemos la escala orignal
        transform.localScale = originalScale;
    }

    //-----------------------------------------------------------------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Chicken"))
        {
            //Asignamos el color de agarre;
            mSrenderer.color = fightingColor;
        }
        
    }

    //-----------------------------------------------------------------------------------

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Asignamos el color de agarre;
        mSrenderer.color = defaultColor;
    }

    #endregion

}
