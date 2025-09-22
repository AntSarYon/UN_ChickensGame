using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class YardsManager : MonoBehaviour
{
    //Lista de Corrales
    [SerializeField] private List<Yard> yardsList = new List<Yard>();

    //Referencia al corral actual (enfocado en camara)
    private int currentYardIndex;
    private Yard currentYard;

    // Evento de Cambio de Corral.
    public UnityAction<Yard> OnCurrentYardChanged;

    // ---------------------------------------------------------------------------

    void Awake()
    {
        // Seteamos el 1er elemento de la lista como el index de corral actual
        currentYardIndex = 0;
    }

    // ------------------------------------------------------------------------

    void Start()
    {
        // El 1er corral sera el principal
        currentYard = yardsList[currentYardIndex];
    }

    // -----------------------------------------------------------------------

    public void GoToRightYard()
    {
        // Si el indice esta en el ultimo (mas a laderecha)
        if (currentYardIndex == yardsList.Count-1)
        {
            // No hacemos nada
            return;
        }
        // Caso contrario...

        // Incrementamos el valor del indice
        currentYardIndex++;
        currentYard = yardsList[currentYardIndex];
        
        // Lanzamos Evento para indicar a todos el nuevo corral
        TriggerEvent_CurrentYardChanged(currentYard);

    }

    // -----------------------------------------------------------------------

    public void GoToLeftYard()
    {
        // Si el indice esta en el primero (mas a la izquierda)
        if (currentYardIndex == 0)
        {
            // No hacemos nada
            return;
        }
        // Caso contrario...

        // Reducimos el valor del indice
        currentYardIndex--;
        currentYard = yardsList[currentYardIndex];

        // Lanzamos Evento para indicar a todos el nuevo corral
        TriggerEvent_CurrentYardChanged(currentYard);
    }

    // -----------------------------------------------------------------------

    public void TriggerEvent_CurrentYardChanged(Yard newCurrentYard)
    {
        // Lanzamos el Evento de Cambio de Corral.
        OnCurrentYardChanged?.Invoke(newCurrentYard);
    }
}
