using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSlots : MonoBehaviour
{
    private Food parentFood;

    [Header("Slots para Pollito")]
    [SerializeField] private List<Transform> slotsList = new List<Transform>();

    [Header("Lista de Pollitos en el Area")]
    [Tooltip("Esta Lista indica los Pollitos que estan dentro del area de accion del comedero, independientemente de si estan comiendo, o no")]
    [SerializeField] private List<ChickenController> closeChickkensList = new List<ChickenController>();

    // Diccionario de Slots y pollitos
    private Dictionary<Transform, ChickenController> dicSlotsChicks = new Dictionary<Transform, ChickenController>();

    // Contador de Slots ocupados
    [HideInInspector] public int takenSlots = 0;

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
        foreach (Transform t in slotsList)
        {
            // Creamos una llave en el Diccionario, con valor vacio
            dicSlotsChicks.Add(t, null);
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

            //Si es e primer pollito en ser agregado a la lista
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
                    // Si su flag de "Hambre" esta activo, NO ESTA DURMIENDO, Y no esta asignado a un Slot
                    if (chk.bIsStarving && !chk.bIsSleeping && !dicSlotsChicks.ContainsValue(chk))
                    {
                        // Si hay Slots libres (Flag de Full desactivado)
                        if (!bFullSlots)
                        {
                            // Asignamos el Pollito al Slot libre...
                            AssignChickenToFreeSlot(chk);

                            // Registramos el pollito que ha empezado a comer
                            parentFood.RegisterEatingChicken(chk);
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
                    // Si el pollito esta asignado a un Slot pero no esta comiendo (Se quedo dormido, o ya se lleno)
                    else if (dicSlotsChicks.ContainsValue(chk) && chk.bIsSleeping)// !chk.bIsEating)
                    {
                        // Quitamos al Pollito de la lista de los que comen
                        parentFood.RemoveEatingChicken(chk);

                        // Hacemos que el pollito abandone la comida (y libere el Slot)
                        chk.Try_AbandonFood();
                    }
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
        foreach (Transform slot in slotsList)
        {
            // Si el slot tiene un Vallue Nulo
            if (dicSlotsChicks[slot] == null)
            {
                // Le asignamos el ChickenControlller
                dicSlotsChicks[slot] = chicken;

                GameSoundsController.Instance.PlayBubbleSound();

                //Hacemos que el Poolito almacene referencia a este comedero y slot
                chicken.Try_AssignFood(parentFood, slot);

                // Aumentamos la cantidad de Slots en uso
                takenSlots++;

                //Actualizamos el contador visual
                parentFood.UpdateSlotsUICounter();

                // Si el contador de Slots libres lleg[o a 0
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
    public void ReleaseChicken(ChickenController chickenToFree)
    {
        //Si el pollito a liberar SI ESTA como valor en el diccionario
        if (dicSlotsChicks.ContainsValue(chickenToFree))
        {
            Transform auxSlot = null;

            // Por cada elemento del diccionario
            foreach(KeyValuePair<Transform, ChickenController> kvp in dicSlotsChicks)
            {
                // Si el pollito es valor...
                if (kvp.Value == (chickenToFree))
                {
                    // almacenamos su llave
                    auxSlot = kvp.Key;
                    break;
                }                
            }

            //Si se allmaceno una referencia a llave...
            if (auxSlot != null)
            {
                // Remomvemos su elemento del Diccionario
                dicSlotsChicks.Remove(auxSlot);

                // Y volvemos a agregarlo, pero con Valor Nulo
                dicSlotsChicks.Add(auxSlot, null);

                // Reducimos el contador de Slots ocupados
                takenSlots--;

                //Actualizamos el contador visual
                parentFood.UpdateSlotsUICounter();

                // Desactivamos el Flag de Comedero Full
                bFullSlots = false;
            }
        }
    }
}