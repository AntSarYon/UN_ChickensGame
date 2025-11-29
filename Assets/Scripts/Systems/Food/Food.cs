using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Food : MonoBehaviour
{
    [Header("Panel (UI) de Ingredientes")]
    [SerializeField] private GameObject IngredientsPanel;

    // Dimensiones del Panel
    private float panelTotalWidth;
    private float panelHeight;
    private float panelIngredientWidth;

    [SerializeField] private GameObject infoMaiz;
    [SerializeField] private GameObject infoSoya;
    [SerializeField] private GameObject infoHarina;
    [SerializeField] private GameObject infoGusanos;

    //Porcentajes de Ingredientes
    [HideInInspector] public float perMaiz = 0.25f;
    [HideInInspector] public float perSoya = 0.25f;
    [HideInInspector] public float perHarina = 0.25f;
    [HideInInspector] public float perGusanos = 0.25f;

    // Lista de Ingredientes del Comedero
    [HideInInspector] public List<Ingredient> ingredientsList = new List<Ingredient>();

    [Header("Slider de comida")]
    public Slider mFoodLevelSlider;

    [Header("Corral al que pertenece")]
    public Yard yard;

    [Header("Velocidad de variacion de comida")]
    [Range(1,5)] [SerializeField] private float foodDecreaseSpeed;

    //Lista de GameObjects (Pollos) que estan chocando con la Comida
    private List<GameObject> chickensList = new List<GameObject>();

    //Referencia a Componentes
    private Collider2D mCollider;
    private Animator mAnimator;

    //--------------------------------------------------------------------------------------

    void Awake()
    {
        //Obtenemos referencia a componentes
        mCollider = GetComponent<Collider2D>();
        mAnimator = GetComponent<Animator>();

        //Inicialmente se tiene una comp de todos los ingredientes
        perMaiz = 0.25f;
        perSoya = 0.25f;
        perHarina = 0.25f;
        perGusanos = 0.25f;

        // Agregamos todos los Ingredientes a la
        // Lista de ingredientes actuales
        ingredientsList.Add(Ingredient.Maiz);
        ingredientsList.Add(Ingredient.Harina);
        ingredientsList.Add(Ingredient.Soya);
        ingredientsList.Add(Ingredient.Gusanos);

    }

    //--------------------------------------------------------------------------------------

    void Start()
    {
        //Traemos los parametros del RulesManager
        foodDecreaseSpeed = GameRulesManager.instance.foodDecreaseSpeed;

        //Obtenemos el ancho original del Panel (Rect)
        panelTotalWidth = IngredientsPanel.GetComponent<RectTransform>().rect.width;
        panelHeight = IngredientsPanel.GetComponent<RectTransform>().rect.height;

        // Calculamos el Ancho necesario x ingrediente.
        panelIngredientWidth = panelTotalWidth / 4;

        // El panel de Ingredientes siempre empezara desactivado
        IngredientsPanel.SetActive(false);

        //Inicialmente se tiene una comp de todos los ingredientes
        infoMaiz.SetActive(true);
        infoSoya.SetActive(true);
        infoHarina.SetActive(true);
        infoGusanos.SetActive(true);
    }

    //--------------------------------------------------------------------------------------

    public void OpenFoodPanel()
    {

        //Reproducimos Animacion
        mAnimator.Play("refill");

        //Hacemos que la UI muestre el Panel de Comida seleccionada,
        //indicando que esta instancia sera la que se muestre
        UIController.Instance.DisplayPanel_with_FoodSelected(this);

        //Reducimos el Cash en 5
        //DayStatusManager.Instance.currentCash -= 5;

        //Reproducimos la animacion de Reduccion de Cash
        //FindObjectOfType<CashUIController>().OnFoodRefillDelegate();
    }

    //--------------------------------------------------------------------------------------

    void Update()
    {
        //Si hay al menos 1 pollo consumiendo comida
        if (chickensList.Count > 0)
        {
            // Inicializamos contador de Pollitos que SI ESTAN COMIENDO
            // (Puede haber pollitos que estan chocando, pero que no estan comiendo)
            int eatingChicks = 0;

            //Por cada pollito que este chocando
            foreach(GameObject chick in chickensList)
            {
                // Si su flag de "Comiendo" esta activa
                if (chick.GetComponent<ChickenController>().eatingFlag)
                {
                    // Se incrementa el contador de pollitos comiendo
                    eatingChicks++;
                }
            }

            //Reducimos el valor del Slider, en base a cuantos pollitos estan comiendo, y a la velocidad definida
            mFoodLevelSlider.value -= eatingChicks * Time.deltaTime * foodDecreaseSpeed;
        }
    }

    //--------------------------------------------------------------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        //Si el objeto con el que ha empezado la colision es un Pollo...
        if (collision.gameObject.CompareTag("Chicken"))
        {
            //Agregamos el Pollo a la Lista
            chickensList.Add(collision.gameObject);
        }
    }

    //--------------------------------------------------------------------------------------

    private void OnCollisionExit(Collision collision)
    {
        //Si el objeto con el que deja de colisionar es un Pollo...
        if (collision.gameObject.CompareTag("Chicken"))
        {
            //Si el pollo estaba en la lista...
            if (chickensList.Contains(collision.gameObject))
            {
                //Lo quitamos de la Lista
                chickensList.Remove(collision.gameObject);
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // FUNCION: Rellenar con nuevos ingredientes
    public void Refill_with_NewIngredients(float maizPer, float soyaPer, float harinaPer, float gusanosPer)
    {
        // Vaciamos la lista de ingredientes
        ingredientsList.Clear();

        //Actualizamos la proporcion de Ingredientes
        perMaiz = maizPer;
        perSoya = soyaPer;
        perHarina = harinaPer;
        perGusanos = gusanosPer;

        // Revisamos los ingredientes para controlar
        // su visualizacion en el panel
        CheckIngredientsForPanel();

        //Reproducimos Animacion
        mAnimator.Play("refill");

        //Llevamnos el valor del Slider al Maximo
        mFoodLevelSlider.value = mFoodLevelSlider.maxValue;
    }

    // -------------------------------------------------------------------------------------
    // FUNCION: REVISAR INGREDIENTES PARA EL PANEL
    private void CheckIngredientsForPanel()
    {
        // Inicializamos le ancho del Panel de ingredientes en 0
        IngredientsPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(0, panelHeight);

        // Dependiendo de si los ingredientes estan o no en la mezcla
        // se activa o desactiva su info en el Panel

        if (perMaiz > 0.00f)
        {
            // Aumentamos el ancho del panel de ingredientes para que quepa 1 ingrediente mas 
            IncreaseIngredientsPanelWidth();
            //Agregamos le ingrediente a la Lista de ingredientes actuales
            ingredientsList.Add(Ingredient.Maiz);
            //Mostramos la info del ingrediente
            infoMaiz.SetActive(true);
            //Actualizmaos su texto segun su porcentaje
            infoMaiz.GetComponentInChildren<TextMeshProUGUI>().text = $"{perMaiz * 100}%";
            
        }
        else infoMaiz.SetActive(false);

        if (perSoya > 0.00f)
        {
            IncreaseIngredientsPanelWidth();
            ingredientsList.Add(Ingredient.Soya);
            infoSoya.SetActive(true);
            infoSoya.GetComponentInChildren<TextMeshProUGUI>().text = $"{perSoya * 100}%";
        }
        else infoSoya.SetActive(false);

        if (perHarina > 0.00f)
        {
            IncreaseIngredientsPanelWidth();
            ingredientsList.Add(Ingredient.Harina);
            infoHarina.SetActive(true);
            infoHarina.GetComponentInChildren<TextMeshProUGUI>().text = $"{perHarina * 100}%";
        }
        else infoHarina.SetActive(false);

        if (perGusanos > 0.00f)
        {
            IncreaseIngredientsPanelWidth();
            ingredientsList.Add(Ingredient.Gusanos);
            infoGusanos.SetActive(true);
            infoGusanos.GetComponentInChildren<TextMeshProUGUI>().text = $"{perGusanos * 100}%";
        }
        else infoGusanos.SetActive(false);
    }

    //--------------------------------------------------------------------------------------
    // FUNCION: Ubcrementar ancho del panel de ingredientes

    private void IncreaseIngredientsPanelWidth()
    {
        // Aumentamos el ancho del panel de ingredientes para que quepa 1 ingrediente mas
        IngredientsPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(
            IngredientsPanel.GetComponent<RectTransform>().sizeDelta.x + panelIngredientWidth,
            panelHeight
            );
    }

    //--------------------------------------------------------------------------------------
    // FUNCION: Mostrar Informacion de ingredientes
    public void ShowIngredientsInfo()
    {
        IngredientsPanel.SetActive(true);
    }

    //--------------------------------------------------------------------------------------
    // FUNCION: Ocultar Informacion de ingredientes
    public void HideIngredientsInfo()
    {
        IngredientsPanel.SetActive(false);
    }

    //--------------------------------------------------------------------------------------

    private void OnMouseOver()
    {
        // Mostramos el Toggle con ingredientes
        ShowIngredientsInfo();
    }

    private void OnMouseDown()
    {
        print(ingredientsList); 
        //Abrimos el Panel de Comida
        OpenFoodPanel();
    }



    private void OnMouseExit()
    {
        // Ocultamos el Toogle con ingredientes
        HideIngredientsInfo();
    }
}
