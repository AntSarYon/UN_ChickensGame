using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Ingredient{
    Maiz,
    Soya,
    Harina,
    Gusanos
};

public class UI_FoodPanel : MonoBehaviour
{
    public Food foodReference;

    [SerializeField] private TextMeshProUGUI txtTitle;

    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnMix;

    [Header("Ingredients Buttons")]
    [SerializeField] private Button btnAddMaiz;
    [SerializeField] private Button btnAddSoya;
    [SerializeField] private Button btnAddHarina;
    [SerializeField] private Button btnAddGusanos;

    [Header("Ingredients Texts")]
    [SerializeField] private TextMeshProUGUI txtPerMaiz;
    [SerializeField] private TextMeshProUGUI txtPerSoya;
    [SerializeField] private TextMeshProUGUI txtPerHarina;
    [SerializeField] private TextMeshProUGUI txtPerGusanos;

    private float perMaiz = 0.00f;
    private float perSoya = 0.00f;
    private float perHarina = 0.00f;
    private float perGusanos = 0.00f;

    private int priceMaiz = 10;
    private int priceSoya = 10;
    private int priceHarina = 10;
    private int priceGusanos = 10;

    [Header("Prices Texts")]
    [SerializeField] private TextMeshProUGUI txtPriceMaiz;
    [SerializeField] private TextMeshProUGUI txtPriceSoya;
    [SerializeField] private TextMeshProUGUI txtPriceHarina;
    [SerializeField] private TextMeshProUGUI txtPriceGusanos;
    

    [Header("Ingredients Slider")]
    [SerializeField] private Slider ingredientsSlider;

    [Header("Sounds")]
    [SerializeField] private AudioClip clipButton;

    private Animator mAnimator;
    private AudioSource mAudioSource;

    // ---------------------------------------------------------

    void Awake()
    {
        // Obtenemos referencias
        mAnimator = GetComponent<Animator>();
        mAudioSource = GetComponent<AudioSource>();

        perMaiz = 0.00f;
        perSoya = 0.00f;
        perHarina = 0.00f;
        perGusanos = 0.00f;
}

    // --------------------------------------------------------

    void Start()
    {
        // Ocultamos todos los Textos de Porcentaje
        txtPerMaiz.gameObject.SetActive(false);
        txtPerGusanos.gameObject.SetActive(false);
        txtPerHarina.gameObject.SetActive(false);
        txtPerSoya.gameObject.SetActive(false);

        // Agregamos Listener de Cerrar Panel
        btnClose.onClick.AddListener(HidePanel);

        btnMix.onClick.AddListener(PlayClickSound);
        btnMix.onClick.AddListener(SaveMix);

        btnAddGusanos.onClick.AddListener(PlayClickSound);
        btnAddGusanos.onClick.AddListener(AddGusanos);

        btnAddHarina.onClick.AddListener(PlayClickSound);
        btnAddHarina.onClick.AddListener(AddHarina);

        btnAddMaiz.onClick.AddListener(PlayClickSound);
        btnAddMaiz.onClick.AddListener(AddMaiz);

        btnAddSoya.onClick.AddListener(PlayClickSound);
        btnAddSoya.onClick.AddListener(AddSoya);

        // El slider de ingredientes empieza en 0...
        ingredientsSlider.value = 0;

        //Traemos los precios seteados para los Ingredientes
        priceHarina = GameRulesManager.instance.precioHarina;
        priceMaiz = GameRulesManager.instance.precioMaiz;
        priceSoya = GameRulesManager.instance.precioSoya;
        priceGusanos = GameRulesManager.instance.precioGusanos;

        //Actualizamos los Textos con os Precios de los Ingredientes...
        txtPriceHarina.text = $"${priceHarina}";
        txtPriceMaiz.text = $"${priceMaiz}";
        txtPriceSoya.text = $"${priceSoya}";
        txtPriceGusanos.text = $"${priceGusanos}";

    }

    // --------------------------------------------------------

    public void IncreaseBarPercenteage()
    {
        // Obtenemos el supuesto nuevo valor del Slider
        float newBarValue = ingredientsSlider.value += 0.25f;

        //Limitamos el valor en caso intente excederse...
        newBarValue = Mathf.Clamp(newBarValue, 0.00f, 1.00f);

        // Asignamos el nuevo valor de la Barra
        ingredientsSlider.value = newBarValue;

        // Si el Slider llega a su valor maximo...
        if (ingredientsSlider.value == ingredientsSlider.maxValue)
        {
            //Habilitamos el boton de Mezclar
            btnMix.GetComponent<Animator>().Play("Enable");
        }
    }

    public void ShowPanel()
    {
        // Actualizamos el Titulo en base al corral al que pertenece el comedero
        txtTitle.text = $"Comedero en '{foodReference.yard.yardName}'";

        // Reproducimos sonido de Panel apareciendo
        GameSoundsController.Instance.PlayShowFoodPanelSound();

        // Mostramos el Panel (Animacion)
        mAnimator.Play("show");
    }

    // -----------------------------------------------------------

    public void HidePanel()
    {
        // Reproducimos sonido de Panel desapareciendo
        GameSoundsController.Instance.PlayHideFoodPanelSound();

        // Ocultamos el Panel (Animacion)
        mAnimator.Play("hide");
    }

    // -----------------------------------------------------------
    // FUNCION : GUARDRA MEZCLA

    public void SaveMix()
    {
        //Llenamos el Comedero con los nuevos ingredientes
        foodReference.Refill_with_NewIngredients(perMaiz, perSoya, perHarina, perGusanos);

        //Escondemos el Panel
        HidePanel();
        
    }

    // -----------------------------------------------------------

    private void PlayClickSound()
    {
        mAudioSource.PlayOneShot(clipButton, 0.75f);
    }

    // -----------------------------------------------------------

    private void ReturnIngredientsToZero()
    {
        ingredientsSlider.value = 0;

        perMaiz = 0.00f;
        perSoya = 0.00f;
        perHarina = 0.00f;
        perGusanos = 0.00f;

        // Ocultamos todos los Textos de Porcentaje
        txtPerMaiz.gameObject.SetActive(false);
        txtPerGusanos.gameObject.SetActive(false);
        txtPerHarina.gameObject.SetActive(false);
        txtPerSoya.gameObject.SetActive(false);

        btnMix.GetComponent<Animator>().Play("DefaultHide");

    }

    // -----------------------------------------------------------

    public void AddHarina()
    {
        // Si la Barra de Mezcla aun no llega a su tope maximo...
        if (ingredientsSlider.value < ingredientsSlider.maxValue)
        {
            DayStatusManager.Instance.TriggerEvent_IngredientAdded(Ingredient.Harina, priceHarina);

            //Incrementamos porcentaje de Harina
            perHarina += 0.25f;

            // Limitamos el posible valor del ingrediente
            perHarina = Mathf.Clamp(perHarina, 0.00f, 1.00f);

            // Actualizamos el texto del ingrediente
            txtPerHarina.text = $"{perHarina*100}% : H a r i n a";

            // Activamos el Texto
            txtPerHarina.gameObject.SetActive(true);

            // Incrementamos valor de la Barra
            IncreaseBarPercenteage();
        }

    }

    // -----------------------------------------------------------

    public void AddMaiz()
    {
        // Si la Barra de Mezcla aun no llega a su tope maximo...
        if (ingredientsSlider.value < ingredientsSlider.maxValue)
        {
            DayStatusManager.Instance.TriggerEvent_IngredientAdded(Ingredient.Maiz, priceMaiz);

            //Incrementamos porcentaje de Harina
            perMaiz += 0.25f;

            // Limitamos el posible valor del ingrediente
            perMaiz = Mathf.Clamp(perMaiz, 0.00f, 1.00f);

            // Actualizamos el texto del ingrediente
            txtPerMaiz.text = $"{perMaiz * 100}% : M a i z";

            // Activamos el Texto
            txtPerMaiz.gameObject.SetActive(true);

            // Incrementamos valor de la Barra
            IncreaseBarPercenteage();
        }
    }

    // -----------------------------------------------------------

    public void AddSoya()
    {
        // Si la Barra de Mezcla aun no llega a su tope maximo...
        if (ingredientsSlider.value < ingredientsSlider.maxValue)
        {
            DayStatusManager.Instance.TriggerEvent_IngredientAdded(Ingredient.Soya, priceSoya);

            //Incrementamos porcentaje de Harina
            perSoya += 0.25f;

            // Limitamos el posible valor del ingrediente
            perSoya = Mathf.Clamp(perSoya, 0.00f, 1.00f);

            // Actualizamos el texto del ingrediente
            txtPerSoya.text = $"{perSoya * 100}% : S o y a";

            // Activamos el Texto
            txtPerSoya.gameObject.SetActive(true);

            // Incrementamos valor de la Barra
            IncreaseBarPercenteage();
        }
    }

    // -----------------------------------------------------------

    public void AddGusanos()
    {
        // Si la Barra de Mezcla aun no llega a su tope maximo...
        if (ingredientsSlider.value < ingredientsSlider.maxValue)
        {
            DayStatusManager.Instance.TriggerEvent_IngredientAdded(Ingredient.Gusanos, priceGusanos);

            //Incrementamos porcentaje de Harina
            perGusanos += 0.25f;

            // Limitamos el posible valor del ingrediente
            perGusanos = Mathf.Clamp(perGusanos, 0.00f, 1.00f);

            // Actualizamos el texto del ingrediente
            txtPerGusanos.text = $"{perGusanos * 100}% : G u s a n o s";

            // Activamos el Texto
            txtPerGusanos.gameObject.SetActive(true);

            // Incrementamos valor de la Barra
            IncreaseBarPercenteage();
        }
    }


}
