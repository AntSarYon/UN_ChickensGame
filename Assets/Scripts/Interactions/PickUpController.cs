using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    // Objetos interactuables
    [HideInInspector] public GameObject targetObject;
    [HideInInspector] public GameObject holdedObject;

    // Transform donde sostendremos el Objeto
    public Transform holdingZone;

    //Referencia al Player Controller (Padre)
    private PlayerController pController;

    // -----------------------------------------------------------------------

    void Awake()
    {
        //Obtenemos referencia a la UI que tiene el padre
        pController = GetComponentInParent<PlayerController>();
    }

    // ----------------------------------------------------------------------

    void Update()
    {
        //Si hay un objeto habilitado para coger, y no tenemos ningún objeto cogido
        if (targetObject != null && targetObject.GetComponent<Draggable>().isDraggable == true && holdedObject == null)
        { 
            //Actualizamos y mostramos el mensaje de interaccion
            pController.pUI.SetInteractionMessage("Agarrar");
            pController.pUI.ShowInteractionMessage();

            //Si pulsamos la tecla E...
            if (Input.GetKeyDown(KeyCode.E))
            {
                //Cogemos el objeto habilitado para ello
                holdedObject = targetObject;

                //Desactivamos la opción de isDraggable, dado que ya está cogido.
                holdedObject.GetComponent<Draggable>().Catch(holdingZone);
            }
        }
        //Si hay un Objeto cogido
        else if (holdedObject != null)
        {
            //Actualizamos y mostramos el mensaje de interaccion
            pController.pUI.SetInteractionMessage("Soltar");
            pController.pUI.ShowInteractionMessage();

            //Si pulsamos la tecla E de nuevo...
            if (Input.GetKeyDown(KeyCode.E))
            {
                //Volvemos a activar la opción de isDraggable del Objeto.
                holdedObject.GetComponent<Draggable>().Drop();

                //Asignamos el ObjetoCogido como vacío
                holdedObject = null;
            }
        }
        else
        {
            //Ocultamos el mensaje de interaccion
            pController.pUI.HideInteractionMessage();
        }
    }
}
