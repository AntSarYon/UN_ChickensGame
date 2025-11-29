using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToySpawner : MonoBehaviour
{
    [Header("Prefab de Juguete")]
    [SerializeField] private GameObject ToyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //Asignamos Delegado al Evento de NuevoJuguete
        DayStatusManager.Instance.OnBuyNewToy += OnBuyNewToyDelegate;

    }

    // ----------------------------------------------

    private void OnBuyNewToyDelegate()
    {
        //Instanciamos el Juguete
        GameObject newToy = Instantiate(
            ToyPrefab,
            YardsManager.instance.currentYard.PosToSpawn,
            new Quaternion(0.216439605f, 0, 0, 0.976296067f)
            );

        //Asignamos al nuevo Pollito su respectivo corral
        newToy.GetComponent<Toy>().assignedYard = YardsManager.instance.currentYard;
    }

}
