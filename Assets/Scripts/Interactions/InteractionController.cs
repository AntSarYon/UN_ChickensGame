using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
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
            // Si el objeto es interactuable
            if (targetObject.GetComponent<Interactable>())
            {
                //Si pulsamos la tecla E...
                if (Input.GetKeyDown(KeyCode.E))
                {
                    //Activamos la interaccion del Objeto Target
                    targetObject.GetComponent<Interactable>().Interact(holdingZone);

                    //Si el Objeto ademas es Pickeable...
                    if (targetObject.GetComponent<PickeableObject>())
                    {
                        // Lo asignamos como objeto sujetado
                        holdedObject = targetObject;

                        //Revisamos que es el objeto que estamos cargando
                        CheckForHoldedObjectTye();
                    }
                }
            }
        }

        //Si hay un Objeto cogido
        else if (holdedObject != null)
        {
            //Si se tiene unn Objeto en frente...
            if (targetObject != null)
            {
                //Solo si el objeto Target No es pickable
                if (!targetObject.GetComponent<PickeableObject>())
                {
                    //Si se oprime la tecla E...
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        //Lanzamos la interaccion (recibe nuestra zona de Agarre, y lo que haya en ella)
                        targetObject.GetComponent<Interactable>().Interact(holdingZone);

                        //Si despues de hacer esto la zona de Agarre queda sin hijos...
                        if (holdingZone.childCount == 0)
                        {
                            //Devolvemos la referencia de objeto agarrado a Null
                            holdedObject = null;
                        }
                    }
                }
                //En caso si haya un objeto al frente, y sea Pickeable...
                else
                {
                    // Gestionamos la accion de Soltar el Objeto
                    ManageObjectRelease();
                }
            }
            //En caso no haya ningun objjeto en frente...
            else
            {
                // Gestionamos la accion de Soltar el Objeto
                ManageObjectRelease();
            }
        }
    }

    private void ManageObjectRelease()
    {

        // Si el payer esta quieto
        if (pController.movementInput == Vector3.zero)
        {
            UIController.Instance.SetInteractionMessage("Soltar");

            if (Input.GetKeyDown(KeyCode.E))
            {
                //Soltamos el Objeto mediante la funcioon de Drop
                holdedObject.GetComponent<PickeableObject>().Drop();

                //Liberamos cualquier referencia dle Objeto.
                holdedObject = null;

                UIController.Instance.HideInteractionMessage();
            }
        }
        //Si el player se esta moviendo
        else
        {
            UIController.Instance.SetInteractionMessage("Arrojar");

            if (Input.GetKeyDown(KeyCode.E))
            {

                holdedObject.GetComponent<PickeableObject>().Throw(pController.movementInput);

                //Liberamos cualquier referencia dle Objeto.
                holdedObject = null;

                UIController.Instance.HideInteractionMessage();
            }
        }

    }

    // -------------------------------------------------------
    // Funcion: Revisar QUE ES el objeto que estamos cargando
    private void CheckForHoldedObjectTye()
    {
        // Si el objeto Holded  es una Bolsa de Comida...
        if (holdedObject.CompareTag("FoodBag"))
        {
            // Activamos el flag de "Cargando comida"
            pController.bisCarryingFood = true;
        }
        // Si el objeto Holded  es un pollo
        else if (holdedObject.CompareTag("Chicken"))
        {
            // Activamos el flag de "Cargando pollo"
            pController.bisCarryingChicken = true;
        }
    }

    // -------------------------------------------------------
    // Funcion: Desactivamos Flags de "cargando objeto"
    private void UpdateDroppedObjectTye()
    {
        // Desactivams todos los flags de "Cargando X"
        pController.bisCarryingFood = false;
        pController.bisCarryingChicken = false;

    }

    // --------------------------------------------
}

