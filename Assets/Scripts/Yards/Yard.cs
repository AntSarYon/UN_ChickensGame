using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yard : MonoBehaviour
{
    // Posicion que debera tener la camara cuando este sobre este corral
    public Vector3 PosToCamera;

    // Nivel de comida (total) actual
    [HideInInspector] public float currentTotalFoodLevel = 0;

    // Maximo nivel psoible de Comida (total)
    [HideInInspector] public float totalFoodMaxValue = 0;

    // Cantidad de Pollitos en Este corral
    [HideInInspector] public float currentChickensCount;

    // Array con los comederod Del Corral
    [SerializeField] private Food[] arrFoods;
    

    // ------------------------------------------------------------------

    void Start()
    {
        //Obtenemos Array con todos los Food de la escena
        arrFoods = FindObjectsByType<Food>(FindObjectsSortMode.None);

        //Definimos var para calcular el Valor maximo posible de comida (total)
        float maxValueForGeneralFoodSlider = 0;

        //Por cada Food en el corral
        foreach (Food food in arrFoods)
        {
            //Incrementamos el valor maximo
            maxValueForGeneralFoodSlider += food.mFoodLevelSlider.maxValue;
        }

        //Asignamos el Valor maximo obtenido como el maximo del Slider de comida
        totalFoodMaxValue = maxValueForGeneralFoodSlider;

        //Asignamos que el nivel de Comida actual es el maximo (Asi inicia siempre)
        currentTotalFoodLevel = maxValueForGeneralFoodSlider;
    }

    // --------------------------------------------------------------------

    void Update()
    {
        //Var temporal para el nuevo total de comida actual
        float newCurrentTotalFoodLevel = 0;


    }
}
