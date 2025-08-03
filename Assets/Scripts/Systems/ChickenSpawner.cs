using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenSpawner : MonoBehaviour
{
    [Header("Prefab de Pollito")]
    [SerializeField] private GameObject chickenPrefab;

    //------------------------------------------------

    void Start()
    {
        GameManager.Instance.OnGenerateNewChicken += OnGenerateNewChicken;
    }

    private void OnGenerateNewChicken()
    {
        //Instanciamos el Pollito
        GameObject newChicken = Instantiate(chickenPrefab, Vector3.zero, Quaternion.identity);

        //Reproducimos su Animacion de Spawn
        newChicken.GetComponent<Animator>().Play("Spawn");
    }
}
