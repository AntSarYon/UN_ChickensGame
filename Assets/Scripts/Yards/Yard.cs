using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Necesario si usas TextMeshPro

public class Yard : MonoBehaviour
{
    public Vector3 PosToCamera;
    public Vector3 PosToSpawn;

    public float RightLimit;
    public float LeftLimit;
    public float TopLimit;
    public float BottomLimit;

    public string yardName;

    [HideInInspector] public float currentTotalFoodLevel = 0;
    [HideInInspector] public float totalFoodMaxValue = 0;
    [HideInInspector] public float currentChickensCount;

    [SerializeField] private List<Food> listFoods = new List<Food>();

    // ------------------- TEMPERATURA -------------------
    [Header("Temperatura del Corral")]
    [SerializeField] public float temperature = 25f; // Temperatura inicial
    [SerializeField] private float temperatureDecreaseInterval = 5f; // Cada cuántos segundos cambia
    [SerializeField] private float temperatureDecreaseAmount = 1f;   // Cuánto baja cada vez
    [SerializeField] private float temperatureIncreaseAmount = 1f;   // Cuánto sube cada vez
    private float temperatureTimer = 0f;
    private bool isTemperatureIncreasing = false;

    [Header("UI Debug")]
    [SerializeField] private TMP_Text temperaturaDebug; // Arrastra el objeto temperaturaDebug aquí

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
        temperatureTimer = temperatureDecreaseInterval;
    }

    // --------------------------------------------------------------------

    void Update()
    {
        // Cambiar el sentido de la temperatura al presionar T
        if (Input.GetKeyDown(KeyCode.T))
        {
            isTemperatureIncreasing = !isTemperatureIncreasing;
        }

        if (YardsManager.instance.currentYard == this)
        {
            float newCurrentFoodLevel = 0;
            foreach (Food food in listFoods)
            {
                newCurrentFoodLevel += food.mFoodLevelSlider.value;
            }
            currentTotalFoodLevel = newCurrentFoodLevel;

            temperatureTimer -= Time.deltaTime;
            if (temperatureTimer <= 0f)
            {
                if (isTemperatureIncreasing)
                {
                    temperature += temperatureIncreaseAmount;
                }
                else
                {
                    temperature -= temperatureDecreaseAmount;
                }
                temperatureTimer = temperatureDecreaseInterval;
            }

            if (temperaturaDebug != null)
            {
                temperaturaDebug.text = $"Temperatura: {temperature:0.0}°C";
            }
        }
    }
}