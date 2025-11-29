using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    //Flag para identifixar si puede ser Pickeado
    [HideInInspector] public bool isDraggable;

    //Componente RigidBody
    private Rigidbody mRb;

    //-----------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------

    #region Methods

    void Awake()
    {
        // Inicia siendo Draggable
        isDraggable = true;

        //Obtencion de Componentes
        mRb = GetComponent<Rigidbody>();
    }
    
    //-----------------------------------------------------------------------------------
    // Funcion - Agarrar a la Gallina

    public void Catch(Transform parent)
    {
        isDraggable = false;

        //Activamos Flag de "siendo arrastrado" en el Controller principal
        GetComponent<ChickenController>().isBeingDragged = true;

        //Lo posicionamos en el mismo lugar que la zona de interacción
        transform.position = parent.position;

        //Emparentamos el objeto con la Zona de Interacción
        transform.SetParent(parent);

        //Le desactivamos la gravedad
        mRb.useGravity = false;

        //Lo marcamos como Kinemático para que las Físicas no le afecten.
        mRb.isKinematic = true;

        // Entramos a animacion de Drag
        GetComponent<SpritesController>().EnterDragAnimation();

        //Capturamos la posicion Offset del Mouse restandole a la posicon del objeto la posicion del mouse, obteniendo la diferencia
        //mousePositionOffset = gameObject.transform.position - MousePositionn3D.Instance.GetMouseWorldPosition();

    }

    //-----------------------------------------------------------------------------------
    // Funcion - Mover a la Gallina

   /* public void MovePosition()
    {
        //La posicion será la del mouse; añadiendole a distancia del suelo
        transform.position = MousePositionn3D.Instance.GetMouseWorldPosition() + mousePositionOffset; //+ (Vector3.up * 0.5f);
    }*/

    //-----------------------------------------------------------------------------------
    // Funcion - Soltar (Liberar) a la Gallina

    public void Drop()
    {
        isDraggable = true;

        //Desactivamos Flag de "siendo arrastrado" en el Controller principal
        GetComponent<ChickenController>().isBeingDragged = false;

        // Devolvemos la animacion a la normalidad
        GetComponent<SpritesController>().SetSpriteBackToNormal();
        

        //Desemparentamos el objeto con la Zona de Interacción
        transform.SetParent(null);

        //Le Activamos la gravedad
        mRb.useGravity = true;

        //Lo desmarcamos como Kinemático para que las Físicas si le afecten.
        mRb.isKinematic = false;

        //Ponemos la velocidad en 0
        //mRb.velocity = Vector3.zero;
    }

    #endregion



}
