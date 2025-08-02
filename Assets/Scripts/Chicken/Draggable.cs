using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    //Flag, esta siendo sujetado
    [HideInInspector] public bool bIsBeingDragged;

    //Posicion Offset del mouse respecto al centro del objeto.
    private Vector3 mousePositionOffset;

    //Componente RigidBody
    private Rigidbody2D mRb;

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    #region Methods

    void Awake()
    {
        //Obtencion de Componentes
        mRb = GetComponent<Rigidbody2D>();
    }

    //-----------------------------------------------------------------------------------
    // Funcion GETTER - Posicion (en el mundo) del Mouse

    private Vector3 GetMouseInWorldPosition()
    {
        //Retornamos las coordenadas en el Mundo que coinciden con la posicion del Mouse
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Agarrar a la Gallina

    public void Catch()
    {
        //Capturamos la posicion Offset del Mouse restandole a la posicon del objeto la posicion del mouse, obteniendo la diferencia
        mousePositionOffset = gameObject.transform.position - GetMouseInWorldPosition();

        //Activamos el flag "Siendo sujetado"
        bIsBeingDragged = true;
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Mover a la Gallina

    public void MovePosition()
    {
        //La posicion será la del mouse; añadiendole el Offset capturado
        transform.position = GetMouseInWorldPosition() + mousePositionOffset;
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Soltar (Liberar) a la Gallina

    public void Drop()
    {
        //Si estaba siendo sujetado
        if (bIsBeingDragged)
        {
            //Ponemos la velocidad en 0
            mRb.velocity = Vector2.zero;

            //Activamos el flag "Siendo sujetado"
            bIsBeingDragged = false;
        }
        
    }

    #endregion



}
