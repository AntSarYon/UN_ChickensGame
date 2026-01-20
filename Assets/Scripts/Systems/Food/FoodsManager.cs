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
        bool finalHasFood = false;

        //Por cada comedero existente...
        for (int i = 0; i < arrFoods.Length; i++)
        {
            // Si el comedero aun tiene comida, y aun tiene Slots ibres...
            if (arrFoods[i].hasFood && arrFoods[i].HasFreeSlots())
            {
                //Obtencion la distancia del comedero actual al pollito
                Vector3 auxFoodPosition = arrFoods[i].transform.position;
                float auxDistance = Vector3.Distance(chickenPosition.position, auxFoodPosition);

                //Si la distancia de este comedero es menor o igual a la ya seteada
                if ((auxDistance <= closestDistance))
                {
                    //Asignamos ese comedero
                    closestFood = arrFoods[i];
                    closestDistance = auxDistance;
                    finalHasFood = true;
                }
            }

            //Si el comedero no tiene slots libres, o comida... ni siquiera entra en consideracion
        }

        //Si el flag de "Ultimo no tiene comida" No esta activo"
        if (!finalHasFood)
        {
            //Retornamos null, para indicar que no hay Comedero libre...
            return null;
        }

        //Retorna el Transform del Comedero mas cercano
        return closestFood.transform;
    }

}
