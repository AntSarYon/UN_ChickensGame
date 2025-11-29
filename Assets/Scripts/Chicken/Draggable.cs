using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    //Posicion Offset del mouse respecto al centro del objeto.
    private Vector3 mousePositionOffset;

    //Componente RigidBody
    private Rigidbody mRb;

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    #region Methods

    void Awake()
    {
        //Obtencion de Componentes
        mRb = GetComponent<Rigidbody>();
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Agarrar a la Gallina

    public void Catch()
    {
        //Capturamos la posicion Offset del Mouse restandole a la posicon del objeto la posicion del mouse, obteniendo la diferencia
        mousePositionOffset = gameObject.transform.position - MousePositionn3D.Instance.GetMouseWorldPosition();

    }

    //-----------------------------------------------------------------------------------
    // Funcion - Mover a la Gallina

    public void MovePosition()
    {
        //La posicion será la del mouse; añadiendole a distancia del suelo
        transform.position = MousePositionn3D.Instance.GetMouseWorldPosition() + mousePositionOffset; //+ (Vector3.up * 0.5f);
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Soltar (Liberar) a la Gallina

    public void Drop()
    {
        //Ponemos la velocidad en 0
        mRb.velocity = Vector3.zero;
    }

    #endregion



}
