using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string interactionMessage = "";

    // ---------------------------------------------------------------------

    protected void SetInteractioonMessage(string newMessage)
    {
        interactionMessage = newMessage;
    }

    // ------------------------------------------------------------------

    protected void OnTriggerEnter(Collider other)
    {
        //Si el Triger al que entramos es la zona de interacción
        if (other.tag == "PlayerInteractionZone")
        {
            //Obtenemos el PickupController del Player (Padre del Triger)
            //para asignarle que este será el Objeto a coger.
            other.GetComponentInParent<InteractionController>().targetObject = this.gameObject;

            // Si el player no esta cargando nada...
            if (other.GetComponentInParent<InteractionController>().holdedObject == null)
            {
                //Mostramos el Mensaje de Interaccion de este Interactuable
                UIController.Instance.SetInteractionMessage(interactionMessage);
                UIController.Instance.ShowInteractionMessage();
            }
        }
    }

    // ----------------------------------------------------------------------

    protected void OnTriggerExit(Collider other)
    {
        //Si el Triger del que salimos es la zona de interacción
        if (other.tag == "PlayerInteractionZone")
        {
            //Si el objeto target es este mismo...
            if (other.GetComponentInParent<InteractionController>().targetObject == this.gameObject)
            {
                //Obtenemos el PickupController del Player (Padre del Triger)
                //para indicar que ya no habrá ningun Objeto Asignado.
                other.GetComponentInParent<InteractionController>().targetObject = null;

                // Si el player no esta cargando nada...
                if (other.GetComponentInParent<InteractionController>().holdedObject == null)
                {
                    // Hacemos que la UI oculte mensaje de interaccion
                    UIController.Instance.HideInteractionMessage();
                }
            }
        }
    }

    // ---------------------------------------------------------------------------

    private void OnTriggerStay(Collider other)
    {
        //Si el Triger del que salimos es la zona de interacción
        if (other.tag == "PlayerInteractionZone")
        {
            //Si el objeto target es este mismo...
            if (other.GetComponentInParent<InteractionController>().targetObject == this.gameObject)
            {
                //Manntenemos el texto de inbteraccion den la UI actualizado
                UIController.Instance.SetInteractionMessage(interactionMessage);
            }
        }
    }

    // -------------------------------------------------
    // Metodo de Interaccion (Abstracto para sobreescritura)

    public abstract void Interact(Transform holdingZone = null, InteractionController interactionController = null);
}
