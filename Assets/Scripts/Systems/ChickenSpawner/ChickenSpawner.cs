using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenSpawner : MonoBehaviour
{
    [Header("Prefabs de Pollitos")]
    [SerializeField] private GameObject chickenRossPrefab;
    [SerializeField] private GameObject chickenCobbPrefab;

    [Header("Sistema de Eliminación Periódica de Pollos")]

    [Tooltip("Cantidad mínima de pollos que debe tener el corral al momento de llevarse a todos (si hay menos => Game Over)")]
    [SerializeField] private int minChickensRequired = 3;

    [Tooltip("Dinero que recibe el jugador por cada pollo adicional llevado por encima del mínimo")]
    [SerializeField] private float rewardPerExtraChicken = 10f;

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
            Yard.Instance.PosToSpawn,
            new Quaternion(0.216439605f, 0, 0, 0.976296067f)
            );

        //Reproducimos su Animacion de Spawn
        newChicken.GetComponent<Animator>().Play("Spawn");

        //Asignamos al nuevo Pollito su respectivo corral
        newChicken.GetComponent<ChickenController>().assignedYard = Yard.Instance;

        //En base al corral, definimos su limite de movimiento
        SelfMovementToTarget newChickenMovementComp = newChicken.GetComponent<SelfMovementToTarget>();

        // Hacemos que empiece sin target de movimiento
        newChickenMovementComp.target = null;

        // Definimos sus limmites de movimiento
        newChickenMovementComp.maxXDistanceToLeft = Yard.Instance.LeftLimit;
        newChickenMovementComp.maxXDistanceToRight = Yard.Instance.RightLimit;
        newChickenMovementComp.maxZDistanceToBottom = Yard.Instance.BottomLimit;
        newChickenMovementComp.maxZDistanceToTop = Yard.Instance.TopLimit;

        // Seteamos un unevo Punto random para su movimiento
        newChickenMovementComp.SetNewRandomWaypoint();
    }

    // --------------------------------------------------------

    private void OnGenerateNewChickenCobbDelegate()
    {
        //Instanciamos el Pollito
        GameObject newChicken = Instantiate(
            chickenCobbPrefab,
            Yard.Instance.PosToSpawn,
            new Quaternion(0.216439605f, 0, 0, 0.976296067f)
            );

        //Reproducimos su Animacion de Spawn
        newChicken.GetComponent<Animator>().Play("Spawn");

        //Asignamos al nuevo Pollito su respectivo corral
        newChicken.GetComponent<ChickenController>().assignedYard = Yard.Instance;

        //En base al corral, definimos su limite de movimiento
        SelfMovementToTarget newChickenMovementComp = newChicken.GetComponent<SelfMovementToTarget>();
        newChickenMovementComp.maxXDistanceToLeft = Yard.Instance.LeftLimit;
        newChickenMovementComp.maxXDistanceToRight = Yard.Instance.RightLimit;
        newChickenMovementComp.maxZDistanceToBottom = Yard.Instance.BottomLimit;
        newChickenMovementComp.maxZDistanceToTop = Yard.Instance.TopLimit;
    }

    // --------------------------------------------------------
}
