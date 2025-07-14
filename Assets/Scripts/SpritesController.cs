using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesController : MonoBehaviour
{
    #region Props

    //Colores para cuando se interactua con el pollo
    private Color defaultColor;
    private Color draggedColor = Color.gray;

    //Vector con la escala original del pollito
    private Vector3 originalScale;
    private Vector3 draggedScale = new Vector3(0.80f, 0.80f, 0.80f);

    #endregion

    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

    #region Components

    private SpriteRenderer mSrenderer;

    #endregion

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    #region Methods 

    void Awake()
    {
        //Obtenemos componentes
        mSrenderer = GetComponent<SpriteRenderer>();
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
        //Asignamos el color de agarre;
        mSrenderer.color = defaultColor;

        //Le devolvemos la escala orignal
        transform.localScale = originalScale;
    }

    #endregion

}
