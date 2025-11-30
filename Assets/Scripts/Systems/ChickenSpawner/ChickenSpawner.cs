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
    }

    // --------------------------------------------------------

    void Update()
    {
        // Actualizamos el timer de eliminación
        eliminationTimer -= Time.deltaTime;

        // Si el timer llega a 0, eliminamos pollos
        if (eliminationTimer <= 0f)
        {
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
    /// <summary>
    /// Lleva (elimina) a todos los pollos del corral actual y trae nuevos. 
    /// Si al momento de llevarlos hay menos de `minChickensRequired` -> Game Over.
    /// Si se llevaron más que el mínimo, recompensa al jugador por cada extra.
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

        int totalRemoved = chickensInCurrentYard.Count;

        // Si hay menos que el mínimo requerido -> Trigger Game Over
        if (totalRemoved < minChickensRequired)
        {
            // Marca GameOver en el DayStatusManager y dispara el evento
            DayStatusManager.Instance.bGameOver = true;
            DayStatusManager.Instance.TriggerEvent_GameOver();
            return;
        }

        // Calculamos recompensa por extras
        int extras = Mathf.Max(0, totalRemoved - minChickensRequired);
        if (extras > 0)
        {
            float reward = extras * rewardPerExtraChicken;
            DayStatusManager.Instance.currentCash += reward;
            CashUIController.instance.PlayIncreaseCash();
        }

        // Guardamos la cantidad de cada tipo (para reproducir la misma mezcla)
        int rossCount = 0;
        int cobbCount = 0;
        foreach (ChickenController ch in chickensInCurrentYard)
        {
            if (ch.type != null && ch.type.typeName == "Ross") rossCount++;
            else cobbCount++;
        }

        // Eliminamos todos los pollos del corral
        foreach (ChickenController ch in chickensInCurrentYard)
        {
            Destroy(ch.gameObject);
        }

        // Traemos nuevos: re-spawneamos la misma cantidad por tipo
        for (int i = 0; i < rossCount; i++)
        {
            OnGenerateNewChickenRossDelegate();
        }
        for (int i = 0; i < cobbCount; i++)
        {
            OnGenerateNewChickenCobbDelegate();
        }
    }
}
