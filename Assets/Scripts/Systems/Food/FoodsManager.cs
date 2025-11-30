using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodsManager : MonoBehaviour
{
    public static FoodsManager Instance;

    [SerializeField] private Food[] arrFoods = new Food[3];

    // -----------------------------------------------------------

    void Awake()
    {
        Instance = this;
    }

    // -----------------------------------------------------------

    public Transform GetClosestFood(Transform chickenPosition)
    {
        //Por defecto es el primero
        Food closestFood = arrFoods[0];
        float closestDistance = Vector3.Distance(chickenPosition.position, closestFood.transform.position);

        //Por cada comedero existente...
        for (int i = 0; i < arrFoods.Length; i++)
        {
            //Obtencion la distancia del comedero actual al pollito
            Vector3 auxFoodPosition = arrFoods[i].transform.position;
            float auxDistance = Vector3.Distance(chickenPosition.position, auxFoodPosition);

            //Si la distancia de este comedero es menor a la ya seteada...
            if (auxDistance < closestDistance)
            {
                //Asignamos ese comedero
                closestFood = arrFoods[i];
                closestDistance = auxDistance;
            }
        }

        return closestFood.transform;
    }

}
