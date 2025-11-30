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
    [SerializeField] private float eliminationIntervalSeconds = 15f;
    [SerializeField] private int chickenCountToRemove = 2;
    [Header("Incremento de pollos llevados")]
    [SerializeField] private int chickensToRemoveIncrement = 1; // Puedes setearlo desde el inspector
    private int currentChickenCountToRemove;
    private float eliminationTimer = 0f;

    [Header("Reglas de Recolección")]
    [Tooltip("Cantidad mínima de pollos que debe tener el corral al momento de llevarse a todos (si hay menos => Game Over)")]
    [SerializeField] private int minChickensRequired = 3;
    [Tooltip("Dinero que recibe el jugador por cada pollo adicional llevado por encima del mínimo")]
    [SerializeField] private float rewardPerExtraChicken = 10f;

    //------------------------------------------------

    void Start()
    {
        DayStatusManager.Instance.OnGenerateNewChickenRoss += OnGenerateNewChickenRossDelegate;
        DayStatusManager.Instance.OnGenerateNewChickenCobb += OnGenerateNewChickenCobbDelegate;
        eliminationTimer = eliminationIntervalSeconds;
        currentChickenCountToRemove = chickenCountToRemove; // Inicializa con el valor base
    }

    // --------------------------------------------------------

    void Update()
    {
        // Actualizamos el timer de eliminación
        eliminationTimer -= Time.deltaTime;

        // Si el timer llega a 0, eliminamos pollos
        if (eliminationTimer <= 0f)
        {
            Debug.Log("currentChickenCountToRemove: "+ currentChickenCountToRemove);
            RemoveAndReplaceChickens();
            eliminationTimer = eliminationIntervalSeconds; // Reiniciamos el timer
        }
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

    // --------------------------------------------------------

    /// <summary>
    /// Lleva (elimina) aleatoriamente una cantidad de pollos del corral actual y trae nuevos del mismo tipo.
    /// Si al momento de llevarlos hay menos de `minChickensRequired` -> Game Over.
    /// </summary>
    private void RemoveAndReplaceChickens()
    {
        // Encontramos todos los pollos en el corral actualmente seleccionado
        ChickenController[] allChickens = FindObjectsOfType<ChickenController>();

        List<ChickenController> chickensInCurrentYard = new List<ChickenController>();
        foreach (ChickenController chicken in allChickens)
        {
            if (chicken.assignedYard == YardsManager.instance.currentYard && chicken.isAlive)
            {
                chickensInCurrentYard.Add(chicken);
            }
        }

        // Si hay menos que el mínimo requerido -> Trigger Game Over
        if (chickensInCurrentYard.Count < minChickensRequired)
        {
            DayStatusManager.Instance.bGameOver = true;
            DayStatusManager.Instance.TriggerEvent_GameOver();
            return;
        }

        // Ajustar la cantidad a eliminar si hay menos pollos
        int toRemove = Mathf.Min(currentChickenCountToRemove, chickensInCurrentYard.Count);

        // Seleccionar aleatoriamente los pollos a eliminar
        List<ChickenController> chickensToRemove = new List<ChickenController>();
        System.Random rng = new System.Random();
        while (chickensToRemove.Count < toRemove)
        {
            int idx = rng.Next(chickensInCurrentYard.Count);
            var selected = chickensInCurrentYard[idx];
            if (!chickensToRemove.Contains(selected))
                chickensToRemove.Add(selected);
        }

        // Contar cuántos de cada tipo se eliminan
        int rossCount = 0;
        int cobbCount = 0;
        foreach (ChickenController ch in chickensToRemove)
        {
            if (ch.type != null && ch.type.typeName == "Ross") rossCount++;
            else cobbCount++;
        }

        // Eliminar los pollos seleccionados
        foreach (ChickenController ch in chickensToRemove)
        {
            Destroy(ch.gameObject);
        }

        // Reemplazar automáticamente los eliminados por nuevos del mismo tipo
        for (int i = 0; i < rossCount; i++)
        {
            OnGenerateNewChickenRossDelegate();
        }
        for (int i = 0; i < cobbCount; i++)
        {
            OnGenerateNewChickenCobbDelegate();
        }

        // Aumentar la cantidad de pollos a llevar la próxima vez
        currentChickenCountToRemove += chickensToRemoveIncrement;
    }
}
