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

    // -------------------------------------------------------------------------------------------
    // CORUTINA: Revisar si a los Pollitos dentro de la Zona activaron su Flag tras haber entrado

    private IEnumerator CheckCloseChickensStatus()
    {
        while (true)
        {
            // Por cada Pollito cerca al comedero (en lista)
            foreach (ChickenController chk in closeChickkensList)
            {
                // Si su flag de "Hambre" esta activo - Y no esta asignado a un Slot
                if (chk.bIsStarving && !dicSlotsChicks.ContainsValue(chk))
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
                        // Hacemos que el Pollito tome un nuevo rumbo
                        chk.bIsEating = false;
                        chk.bIsWalking = true;

                        chk.GetComponent<SelfMovementToTarget>().SetNewRandomWaypoint();
                    }
                }
            }

            // Volveremos a revisar tras pasado 0.5 segundos
            yield return new WaitForSeconds(0.5f);
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

            //Si el Pollito tiene hambre
            if (chicken.bIsStarving)
            {
                //Si los Slots NO estan llenos
                if (!bFullSlots)
                {
                    AssignChickenToFreeSlot(chicken);
                }

                //En caso no haya ningun Slot de comida vacio
                else
                {
                    // Hacemos que el Pollito tome un nuevo rumbo
                    chicken.bIsEating = false;
                    chicken.bIsWalking = true;

                    chicken.GetComponent<SelfMovementToTarget>().SetNewRandomWaypoint();
                }
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

                //Hacemos que el Poolito almacene referencia a este comedero
                chicken.Try_AssignFood(parentFood, slot);

                // Reducimos la cantidad de Slots libres
                takenSlots++;

                Debug.Log("Se ha agregado el Pollito al Slot");

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
            // Por cada registro del diccionario
            foreach (var item in dicSlotsChicks)
            {
                // Si el pollito es valor...
                if (item.Value == chickenToFree)
                {
                    //Actualizamos a referencia de Valor a Null
                    dicSlotsChicks[item.Key] = null;

                    // Reducimos el contador de Slots ocupados
                    takenSlots--;

                    // Desactivamos el Flag de Comedero Full
                    bFullSlots = false;
                }
            }
        }

        //Vaidamos si el pollito esta en la lista de Pollitos cercanos
        if (closeChickkensList.Contains(chickenToFree))
        {
            //Removemos la referencia de ese polllito de a lista
            closeChickkensList.Remove(chickenToFree);
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