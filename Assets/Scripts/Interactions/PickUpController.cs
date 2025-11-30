using System;
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
        if (targetObject != null && holdedObject == null)
        {
            if (targetObject.GetComponent<Draggable>() != null)
            {
                if (targetObject.GetComponent<Draggable>().isDraggable)
                {
                    //Actualizamos y mostramos el mensaje de interaccion
                    pController.pUI.SetInteractionMessage("Agarrar");
                    pController.pUI.ShowInteractionMessage();

                    //Si pulsamos la tecla E...
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        //Tomamos el Target como objeto sostenido
                        holdedObject = targetObject;

                        //Desactivamos la opción de isDraggable, dado que ya está cogido.
                        holdedObject.GetComponent<Draggable>().Catch(holdingZone);
                    }
                        
                }
            }
            else if (targetObject.GetComponent<PickeableObject>() != null)
            {
                if (targetObject.GetComponent<PickeableObject>().isPickeable)
                {
                    //Actualizamos y mostramos el mensaje de interaccion
                    pController.pUI.SetInteractionMessage("Agarrar");
                    pController.pUI.ShowInteractionMessage();

                    //Si pulsamos la tecla E...
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        holdedObject = targetObject;

                        //Desactivamos la opción de IsPickeable, dado que ya está cogido.
                        holdedObject.GetComponent<PickeableObject>().isPickeable = false;

                        //Lo posicionamos en el mismo lugar que la zona de interacción
                        holdedObject.transform.position = holdingZone.position;

                        //Emparentamos el objeto con la Zona de Interacción
                        holdedObject.transform.SetParent(holdingZone);

                        //Le desactivamos la gravedad
                        holdedObject.GetComponent<Rigidbody>().useGravity = false;

                        //Lo marcamos como Kinemático para que las Físicas no le afecten.
                        holdedObject.GetComponent<Rigidbody>().isKinematic = true;

                        // Si el objeto Holded  es una Bolsa de Comida...
                        if (holdedObject.CompareTag("FoodBag"))
                        {
                            // Activamos el flag de "Cargando comida"
                            pController.bisCarryingFood = true;
                        }
                    }
                }
            }
        }
        
        //Si hay un Objeto cogido
        else if (holdedObject != null)
        {
            //Actualizamos y mostramos el mensaje de interaccion
            pController.pUI.SetInteractionMessage("Soltar");

            // Si el flag de "Cargando comida" está activo, y estamos frente a un comedero
            if (pController.bisCarryingFood)
            {
                //Si el objeto en manos es una bolsa de comida, y estamos apuntando a aun comedero
                if (targetObject != null && targetObject.CompareTag("Food"))
                {
                    //Actualizamos y mostramos el mensaje de interaccion
                    pController.pUI.SetInteractionMessage("LLENAR");
                }
            }
            
            // Muestra mensaje de interaccion
            pController.pUI.ShowInteractionMessage();

            //Si pulsamos la tecla E de nuevo...
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (holdedObject.GetComponent<Draggable>() != null)
                {
                    //Volvemos a activar la opción de isDraggable del Objeto.
                    holdedObject.GetComponent<Draggable>().Drop();
                    //holdedObject.GetComponent<Rigidbody>().AddForce(pController.movementInput * 10, ForceMode.Impulse);

                    // Asignamos el ObjetoCogido como vacío
                    holdedObject = null;
                }

                else if (holdedObject.GetComponent<PickeableObject>() != null)
                {
                    //Volvemos a activar la opción de IsPickeable del Objeto.
                    holdedObject.GetComponent<PickeableObject>().isPickeable = true;

                    //Desemparentamos el objeto con la Zona de Interacción
                    holdedObject.transform.SetParent(null);

                    //Le Activamos la gravedad
                    holdedObject.GetComponent<Rigidbody>().useGravity = true;

                    //Lo desmarcamos como Kinemático para que las Físicas si le afecten.
                    holdedObject.GetComponent<Rigidbody>().isKinematic = false;

                    //Si el objeto en manos es una bolsa de comida
                    if (holdedObject.CompareTag("FoodBag"))
                    {
                        if (targetObject != null)
                        {
                            if (targetObject.CompareTag("Food"))
                            {
                                //Almacenamos el bag en una variable
                                GameObject bag = holdedObject;

                                // Asignamos el ObjetoCogido como vacío
                                holdedObject = null;

                                //Desactivamos la bolsa
                                bag.SetActive(false);
                            }
                        }

                        //Desactivamos Flag de "Cargando comida"
                        pController.bisCarryingFood = false;
                    }

                    // Asignamos el ObjetoCogido como vacío
                    holdedObject = null;

                    //Actualizamos y mostramos el mensaje de interaccion
                    pController.pUI.HideInteractionMessage();
                }

                //Actualizamos y mostramos el mensaje de interaccion
                pController.pUI.HideInteractionMessage();
            }
        }
        else pController.pUI.HideInteractionMessage();
        

        
    }
}
