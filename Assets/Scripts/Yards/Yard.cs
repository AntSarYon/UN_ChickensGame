using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yard : MonoBehaviour
{
    // Posicion que debera tener la camara cuando este sobre este corral
    public Vector3 PosToCamera;
    public Vector3 PosToSpawn;

    //Rango y distancia para destino aleatorio
    public float RightLimit;
    public float LeftLimit;
    public float TopLimit;
    public float BottomLimit;

    public string yardName;

    // Nivel de comida (total) actual
    [HideInInspector] public float currentTotalFoodLevel = 0;

    // Maximo nivel psoible de Comida (total)
    [HideInInspector] public float totalFoodMaxValue = 0;

    // Cantidad de Pollitos en Este corral
    [HideInInspector] public float currentChickensCount;

    // Array con los comederod Del Corral
    [SerializeField] private List<Food> listFoods = new List<Food>();
    

    // ------------------------------------------------------------------

    void Start()
    {
        //Obtenemos Array con todos los Food de la escena
        Food[] arrAllFoods = FindObjectsByType<Food>(FindObjectsSortMode.None);

        //Definimos var para calcular el Valor maximo posible de comida (total)
        float maxValueForGeneralFoodSlider = 0;

        // Por cada Food en el Array
        foreach (var food in arrAllFoods)
        {
            // A aquellos que pertencen a este Yard
            if (food.yard == this)
            {
                // Los consideramos para obtener el valor maximo de Food del Corral
                maxValueForGeneralFoodSlider += food.mFoodLevelSlider.maxValue;

                // Agregamos el Food a la Lista de Foods de este corral
                listFoods.Add(food);
            }
        }

        //Asignamos el Valor maximo obtenido como el maximo del Slider de comida
        totalFoodMaxValue = maxValueForGeneralFoodSlider;

        //Asignamos que el nivel de Comida actual es el maximo (Asi inicia siempre)
        currentTotalFoodLevel = maxValueForGeneralFoodSlider;
    }

    // --------------------------------------------------------------------

    void Update()
    {
        // Solo si el Corral actual es el nuestro, hacemos el calculo
        if (YardsManager.instance.currentYard == this)
        {
            //Var temporal para el nuevo total de comida actual en el corral...
            float newCurrentFoodLevel = 0;

            //Por cada Food
            foreach (Food food in listFoods)
            {
                //Acumulamos su cantida de comida disponible
                newCurrentFoodLevel += food.mFoodLevelSlider.value;
            }

            //Actualizamos la variable de "nivel de Comida actual"
            currentTotalFoodLevel = newCurrentFoodLevel;
        }
    }
}
