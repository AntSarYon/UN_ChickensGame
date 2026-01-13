using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPool : Interactable
{
    [Header("Prefab")]
    [SerializeField] private GameObject foodBagPrefab;

    // -----------------------------------------------------------

    void Awake()
    {
        interactionMessage = "Coger Bolsa de Alimento";
    }

    public override void Interact(Transform holdingZone = null, InteractionController interactionController = null)
    {
        //Instanciamos la bolsa de Comida
        GameObject newFoodBag = Instantiate(foodBagPrefab, holdingZone.position, holdingZone.rotation);

        // Usamos la funcion Pick para hacer que este sujetado por el Jugador
        newFoodBag.GetComponent<FoodBag>().Pick(holdingZone);

        //Asignamos directamente el objeto
        interactionController.SetHoldedObject(newFoodBag);
    }

}
