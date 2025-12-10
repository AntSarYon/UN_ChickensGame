using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Necesario si usas TextMeshPro
using System;

public class Yard : MonoBehaviour
{
    public static Yard Instance;

    [Header("Posiciones Base")]
    public Vector3 PosToCamera;
    public Vector3 PosToSpawn;

    [Header("Limites de movimiento")]
    public float RightLimit;
    public float LeftLimit;
    public float TopLimit;
    public float BottomLimit;

    [Header("Nombre del Corral")]
    public string yardName;

    [HideInInspector] public float currentTotalFoodLevel = 0;
    [HideInInspector] public float totalFoodMaxValue = 0;
    [HideInInspector] public float currentChickensCount;

    [Header("Lista de Comederos del Corral")]
    [SerializeField] private List<Food> listFoods = new List<Food>();

    [Header("UI Debug")]
    [SerializeField] private TMP_Text temperaturaDebug; // Arrastra el objeto temperaturaDebug aquí

    // ------------------------------------------------------------------

    void Awake()
    {
        Instance = this;
    }

    // ------------------------------------------------------------------

    void Start()
    {
        Food[] arrAllFoods = FindObjectsByType<Food>(FindObjectsSortMode.None);
        float maxValueForGeneralFoodSlider = 0;

        foreach (var food in arrAllFoods)
        {
            if (food.yard == this)
            {
                maxValueForGeneralFoodSlider += food.mFoodLevelSlider.maxValue;
                listFoods.Add(food);
            }
        }

        totalFoodMaxValue = maxValueForGeneralFoodSlider;
        currentTotalFoodLevel = maxValueForGeneralFoodSlider;

    }

    // --------------------------------------------------------------------

    void Update()
    {

        float newCurrentFoodLevel = 0;
        foreach (Food food in listFoods)
        {
            newCurrentFoodLevel += food.mFoodLevelSlider.value;
        }
        currentTotalFoodLevel = newCurrentFoodLevel;

    }
}