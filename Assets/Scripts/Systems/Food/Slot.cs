using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public ChickenController chickenInSlot;

    [HideInInspector] public FoodSlots slotManager;
    [HideInInspector] public Food parentFood;

    // Referencia a corutina "Revisar estatus de pollito"
    private Coroutine cor_checkChickenStatus;

    // -------------------------------------------------------

    void Awake()
    {
        chickenInSlot = null;
    }

    public void CatchChicken(ChickenController newChicken)
    {
        // Asignamos la referecia del Pollito
        chickenInSlot = newChicken;

        GameSoundsController.Instance.PlayBubbleSound();

        //Hacemos que el Poolito almacene referencia a este comedero y slot
        chickenInSlot.Try_AssignFoodSlot(this);

        // Registramos el pollito que ha empezado a comer
        parentFood.RegisterEatingChicken(chickenInSlot);

        //Arrancamos Corutina para mointorear su estado...
        cor_checkChickenStatus = StartCoroutine(CheckChickenStatus());
    }

    // ---------------------------------------------------------------------

    private IEnumerator CheckChickenStatus()
    {
        // De forma constante...
        while (true)
        {
           // Si el pollito se quedo dormido, o ya no esta comiendo
           if (chickenInSlot.bIsSleeping || !chickenInSlot.bIsEating)
           {
                // Lo liberamos del Slot
                ReleaseChicken();
           }

          // Esperamos 0.25 segundos para volver a commprobar...
           yield return new WaitForSeconds(0.25f);
        }
    }

    // ---------------------------------------------------------

    public void ReleaseChicken()
    {
        // Detenemos la corutina de Revision de Status
        StopCoroutine(cor_checkChickenStatus);

        // Quitamos al Pollito de la lista de los que comen
        parentFood.RemoveEatingChicken(chickenInSlot);

        // Liberamos la referencia de Pollito
        chickenInSlot = null;
    }


}
