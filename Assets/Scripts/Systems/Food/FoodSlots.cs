using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FoodSlots : MonoBehaviour
{
    private Food parentFood;

    [Header("Slots para Pollito")]
    [SerializeField] private List<Slot> slotsList = new List<Slot>();

    [Header("Lista de Pollitos en el Area")]
    [Tooltip("Esta Lista indica los Pollitos que estan dentro del area de accion del comedero, independientemente de si estan comiendo, o no")]
    [SerializeField] private List<ChickenController> closeChickkensList = new List<ChickenController>();

    // Contador de Slots ocupados
    [HideInInspector] public int takenSlots = 0;

    // Flag de Todos los Slots Full
    [HideInInspector] public bool bFullSlots;

    // Referencia a corutina "Revisar pollitos"
    private Coroutine cor_checkChickens;

    // -------------------------------------------------------------------------

    void Awake()
    {
        //Inicia con 5 slots libres
        takenSlots = 0;

        // Flag de Slots llenos inicia apagado
        bFullSlots = false;
    }

    // -------------------------------------------------------------------------

    void Start()
    {
        //Obtenemos referencia al Food padre
        parentFood = GetComponentInParent<Food>();

        //Por cada Slot del comedero
        foreach (Slot s in slotsList)
        {
            // Le asignamos referencia del SlotManager y al comedero
            s.slotManager = this;
            s.parentFood = this.parentFood;

        }
    }

    // -------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        //Si el polito entra en la zona...
        if (other.CompareTag("Chicken"))
        {
            //Obtenemos referencia del Script del Chicken
            ChickenController chicken = other.GetComponent<ChickenController>();

            //Agregamo el pollito a la lista de Pollitos cercanos
            closeChickkensList.Add(chicken);

            //Si es el primer pollito en ser agregado a la lista
            if (closeChickkensList.Count == 1)
            {
                //Arrancamos la Corutina de Revision de Pollitos en zona
                cor_checkChickens = StartCoroutine(CheckCloseChickensStatus());
            }

        }
    }

    // -----------------------------------------------------------------

    private void OnTriggerExit(Collider other)
    {
        //Si lo que salio de la zona es un Pollito...
        if (other.CompareTag("Chicken"))
        {
            //Obtenemos referencia a su Comp de Pllito
            ChickenController chk = other.GetComponent<ChickenController>();

            //Vaidamos si el pollito esta en la lista de Pollitos cercanos
            if (closeChickkensList.Contains(chk))
            {
                //Removemos la referencia de ese polllito de a lista
                closeChickkensList.Remove(chk);
            }

            //Si a lista queda en 0 (Sin politos cerca)
            if (closeChickkensList.Count == 0)
            {
                //Si la Corutina de revision esta corriendo...
                if (cor_checkChickens != null)
                {
                    //La Detenemos
                    StopCoroutine(cor_checkChickens);
                }
            }
        }
    }

    // -------------------------------------------------------------------------------------------
    // CORUTINA: Revisar si a los Pollitos dentro de la Zona activaron su Flag tras haber entrado

    private IEnumerator CheckCloseChickensStatus()
    {
        while (true)
        {
            //Si el comedero aun tiene comida
            if (parentFood.hasFood)
            {
                //Obtenemos la lista de pollitos cercanos actualizada
                List<ChickenController> updatedList = closeChickkensList;

                // Por cada Pollito cerca al comedero (en lista)
                foreach (ChickenController chk in updatedList)
                {
                    // Si su flag de "Hambre" esta activo, no esta comiendo, NO ESTA DURMIENDO, Y no esta asignado a un comoedero
                    if (chk.bIsStarving && !chk.bIsEating && !chk.bIsSleeping && chk.assignedFoodSlot == null)
                    {
                        // Si hay Slots libres (Flag de Full desactivado)
                        if (!bFullSlots)
                        {
                            // Asignamos el Pollito al Slot libre...
                            AssignChickenToFreeSlot(chk);
                        }

                        //En caso no haya ningun Slot de comida vacio
                        else
                        {
                            //Mantenemos su flag de "Comiendo" desactivado
                            chk.bIsEating = false;

                            // Hacemos que el Pollito tome un nuevo rumbo, en direccion contraria a este comedero
                            if (chk && chk.GetComponent<SelfMovementToTarget>())
                            {
                                chk.GetComponent<SelfMovementToTarget>().SetNewRandomWaypointInOpositeDirection(parentFood.transform.position);
                            }
                        }
                    }

                    // El script del Slot se encargara de determinar cuando debe ser liberado el pollito

                }
            }

            // Volveremos a revisar tras pasado 0.5 segundos
            yield return new WaitForSeconds(0.25f);
        }
    }

    // -----------------------------------------------------------------

    private void AssignChickenToFreeSlot(ChickenController chicken)
    {
        //Buscamos, por cada Slot de la lista
        foreach (Slot slot in slotsList)
        {
            // Si el slot No tiene un pollito asignado
            if (!slot.chickenInSlot)
            {
                // Le asignamos el ChickenControlller
                slot.CatchChicken(chicken);

                // Aumentamos la cantidad de Slots en uso
                takenSlots++;

                // Si el contador de Slots tomados llego a 5
                if (takenSlots == 5)
                {
                    // Se activa el flag de Full
                    bFullSlots = true;
                }
                break;
            }
        }
    }

    // ---------------------------------------------------------
    // Funcionn: Liberar Pollito
    public void ReleaseSlot()
    {
        // Reducimos el contador de Slots ocupados
        takenSlots--;

        // Desactivamos el Flag de Comedero Full
        bFullSlots = false;
    }


}