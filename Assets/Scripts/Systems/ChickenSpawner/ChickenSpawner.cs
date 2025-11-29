using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenSpawner : MonoBehaviour
{
    [Header("Prefabs de Pollitos")]
    [SerializeField] private GameObject chickenRossPrefab;
    [SerializeField] private GameObject chickenCobbPrefab;

    //------------------------------------------------

    void Start()
    {
        DayStatusManager.Instance.OnGenerateNewChickenRoss += OnGenerateNewChickenRossDelegate;
        DayStatusManager.Instance.OnGenerateNewChickenCobb += OnGenerateNewChickenCobbDelegate;
    }

    // --------------------------------------------------------


    private void OnGenerateNewChickenRossDelegate()
    {
        //Instanciamos el Pollito
        GameObject newChicken = Instantiate(
            chickenRossPrefab, 
            YardsManager.instance.currentYard.PosToSpawn, 
            new Quaternion(0.216439605f, 0, 0, 0.976296067f)
            );

        //Reproducimos su Animacion de Spawn
        newChicken.GetComponent<Animator>().Play("Spawn");

        //Asignamos al nuevo Pollito su respectivo corral
        newChicken.GetComponent<ChickenController>().assignedYard = YardsManager.instance.currentYard;

        //En base al corral, definimos su limite de movimiento
        SelfMovementToTarget newChickenMovementComp = newChicken.GetComponent<SelfMovementToTarget>();
        newChickenMovementComp.maxXDistanceToLeft = YardsManager.instance.currentYard.LeftLimit;
        newChickenMovementComp.maxXDistanceToRight = YardsManager.instance.currentYard.RightLimit;
        newChickenMovementComp.maxZDistanceToBottom = YardsManager.instance.currentYard.BottomLimit;
        newChickenMovementComp.maxZDistanceToTop = YardsManager.instance.currentYard.TopLimit;

    }

    private void OnGenerateNewChickenCobbDelegate()
    {
        //Instanciamos el Pollito
        GameObject newChicken = Instantiate(
            chickenCobbPrefab,
            YardsManager.instance.currentYard.PosToSpawn,
            new Quaternion(0.216439605f, 0, 0, 0.976296067f)
            );

        //Reproducimos su Animacion de Spawn
        newChicken.GetComponent<Animator>().Play("Spawn");

        //Asignamos al nuevo Pollito su respectivo corral
        newChicken.GetComponent<ChickenController>().assignedYard = YardsManager.instance.currentYard;

        //En base al corral, definimos su limite de movimiento
        SelfMovementToTarget newChickenMovementComp = newChicken.GetComponent<SelfMovementToTarget>();
        newChickenMovementComp.maxXDistanceToLeft = YardsManager.instance.currentYard.LeftLimit;
        newChickenMovementComp.maxXDistanceToRight = YardsManager.instance.currentYard.RightLimit;
        newChickenMovementComp.maxZDistanceToBottom = YardsManager.instance.currentYard.BottomLimit;
        newChickenMovementComp.maxZDistanceToTop = YardsManager.instance.currentYard.TopLimit;

    }
}
