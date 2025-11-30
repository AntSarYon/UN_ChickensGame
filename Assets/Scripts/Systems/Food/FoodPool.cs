using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPool : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject foodBagPrefab;

    [Header("Transform de Spawn")]
    [SerializeField] private Transform spawnTransform;

    // ---------------------------------------------

    public void SpawnNewFoodBag()
    {
        //Instanciamos la bolsa de Comida
        GameObject newFoodBag = Instantiate(
            foodBagPrefab,
            spawnTransform.position,
            spawnTransform.rotation
        );
    }

    // -------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnNewFoodBag();
        }
    }
}
