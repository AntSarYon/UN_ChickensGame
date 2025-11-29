using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickeableObject : MonoBehaviour
{
    
    public bool isPickeable;

    void Awake()
    {
        isPickeable = true;
    }

    // -----------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        //Si el Triger al que entramos es la zona de interacción
        if (other.tag == "PlayerInteractionZone")
        {
            //Obtenemos el PickupController del Player (Padre del Triger)
            //para asignarle que este será el Objeto a coger.
            other.GetComponentInParent<PickUpController>().targetObject = this.gameObject;
        }
    }

    //--------------------------------------------------------------------------------------

    private void OnTriggerExit(Collider other)
    {
        //Si el Triger del que salimos es la zona de interacción
        if (other.tag == "PlayerInteractionZone")
        {
            //Si el objeto target es este mismo...
            if(other.GetComponentInParent<PickUpController>().targetObject == this.gameObject)

            //Obtenemos el PickupController del Player (Padre del Triger)
            //para indicar que ya no habrá ningun Objeto Asignado.
            other.GetComponentInParent<PickUpController>().targetObject = null;
        }
    }
}
