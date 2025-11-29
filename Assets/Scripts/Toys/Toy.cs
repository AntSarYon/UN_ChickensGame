using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toy : MonoBehaviour
{
    // Referencia al Corral donde estara este Juguete
    [HideInInspector] public Yard assignedYard;

    // Flag de "ha sido coloocado"
    private bool bHasBeenPlaced = false;

    // ---------------------------------------------------------

    private void OnEnable()
    {
        bHasBeenPlaced = false;
    }

    // ---------------------------------------------------------

    // Update is called once per frame
    void Update()
    {
        //Si el Juguete aun no ha sido colocado
        if (!bHasBeenPlaced)
        {
            // Lo movemos segun la posicion del Mouse (Considerando su distancia respecto al suelo)
            transform.position = MousePositionn3D.Instance.GetMouseWorldPosition() + (Vector3.up * 0.75f) - (Vector3.forward * 0.5f);
        }
    }

    private void OnMouseDown()
    {
        // Reproducimos Sonido de Posicionamiento de Juguete
        GameSoundsController.Instance.PlayToyPlacementSound();

        //Activamos el Flag de "Ha sido colocado"
        bHasBeenPlaced = true;
    }
}
