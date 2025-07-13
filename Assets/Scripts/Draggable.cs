using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{

    #region Props

    //Posicion Offset del mouse respecto al centro del objeto.
    private Vector3 mousePositionOffset;

    #endregion

    //-----------------------------------------------------------------------------------

    #region Methods

    //-----------------------------------------------------------------------------------
    // Funcion GETTER - Posicion (en el mundo) del Mouse

    private Vector3 GetMouseInWorldPosition()
    {
        //Retornamos las coordenadas en el Mundo que coinciden con la posicion del Mouse
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    //-----------------------------------------------------------------------------------
    // Funci{on - Al hacer Click sobre el Objeto (con collider)

    private void OnMouseDown()
    {
        //Capturamos la posicion Offset del Mouse restandole a la posicon del objeto la posicion del mouse, obteniendo la diferencia
        mousePositionOffset = gameObject.transform.position - GetMouseInWorldPosition();
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Mientras se mantenga el Mouse oprimido...

    private void OnMouseDrag()
    {
        //La posicion será la del mouse; añadiendole el Offset capturado
        transform.position = GetMouseInWorldPosition() + mousePositionOffset;
    }

    #endregion

    //----------------------------------------------------------


}
