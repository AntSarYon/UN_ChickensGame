using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuRulesController : MonoBehaviour
{
    [SerializeField] private Slider sliderAumentoHambre;
    [SerializeField] private TextMeshProUGUI txtAumentoHambre;

    [SerializeField] private Slider sliderReduccionHambre;
    [SerializeField] private TextMeshProUGUI txtReduccionHambre;

    [SerializeField] private Slider sliderConsumoAlimento;
    [SerializeField] private TextMeshProUGUI txtConsumoAlimento;

    [SerializeField] private Slider sliderReduccionfelicidad;
    [SerializeField] private TextMeshProUGUI txtReduccionfelicidad;

    [SerializeField] private Slider sliderAumentofelicidad;
    [SerializeField] private TextMeshProUGUI txtAumentofelicidad;

    [SerializeField] private Slider sliderAumentoPeso;
    [SerializeField] private TextMeshProUGUI txtAumentoPeso;

    [SerializeField] private Slider sliderReduccionPeso;
    [SerializeField] private TextMeshProUGUI txtReduccionPeso;

    [Header("Precio de Ingredientes")]

    [SerializeField] private Slider sliderHarina;
    [SerializeField] private TextMeshProUGUI txtPrecioHarina;

    [SerializeField] private Slider sliderMaiz;
    [SerializeField] private TextMeshProUGUI txtPrecioMaiz;

    [SerializeField] private Slider sliderSoya;
    [SerializeField] private TextMeshProUGUI txtPrecioSoya;

    [SerializeField] private Slider sliderGusanos;
    [SerializeField] private TextMeshProUGUI txtPrecioGusanos;

    [Space]

    [SerializeField] private Button btnStartGame;

    //-----------------------------------------------------------------------

    void Start()
    {
        //Si hay nuevos parametros guardados...
        if (GameRulesManager.instance.nuevosParametrosGuardados)
        {
            sliderAumentoHambre.value = GameRulesManager.instance.velocidadIncrementoHambre;
            sliderReduccionHambre.value = GameRulesManager.instance.velocidadReduccionHambre;
            sliderConsumoAlimento.value = GameRulesManager.instance.foodDecreaseSpeed;
            sliderReduccionfelicidad.value = GameRulesManager.instance.velocidadReduccionfelicidad;
            sliderAumentofelicidad.value = GameRulesManager.instance.velocidadIncrementofelicidad;
            sliderAumentoPeso.value = GameRulesManager.instance.velocidadIncrementoPeso;
            sliderReduccionPeso.value = GameRulesManager.instance.velocidadReduccionPeso;

            //Iniciamoms con Precios de Ingredienntes iguales
            sliderHarina.value = GameRulesManager.instance.precioHarina;
            sliderMaiz.value = GameRulesManager.instance.precioMaiz;
            sliderSoya.value = GameRulesManager.instance.precioSoya;
            sliderGusanos.value = GameRulesManager.instance.precioGusanos;
        }
        // casoc ontrario, los que estan por defecto
        else
        {
            sliderAumentoHambre.value = 2;
            sliderReduccionHambre.value = 4;
            sliderConsumoAlimento.value = 3;
            sliderReduccionfelicidad.value = 3;
            sliderAumentofelicidad.value = 1;
            sliderAumentoPeso.value = 0.15f;
            sliderReduccionPeso.value = 0.1f;

            //Iniciamoms con Precios de Ingredienntes iguales
            sliderHarina.value = 10;
            sliderMaiz.value = 10;
            sliderSoya.value = 10;
            sliderGusanos.value = 10;
        }

    }

    // -----------------------------------------------------------------------

    void Update()
    {
        txtAumentoHambre.text = sliderAumentoHambre.value.ToString("F2");
        txtReduccionHambre.text = sliderReduccionHambre.value.ToString("F2");

        txtConsumoAlimento.text = sliderConsumoAlimento.value.ToString("F2");

        txtReduccionfelicidad.text = sliderReduccionfelicidad.value.ToString("F2");
        txtAumentofelicidad.text = sliderAumentofelicidad.value.ToString("F2");

        txtAumentoPeso.text = sliderAumentoPeso.value.ToString("F2");
        txtReduccionPeso.text = sliderReduccionPeso.value.ToString("F2");

        txtPrecioHarina.text = $"$ {(int)sliderHarina.value}";
        txtPrecioMaiz.text = $"$ {(int)sliderMaiz.value}";
        txtPrecioSoya.text = $"$ {(int)sliderSoya.value}";
        txtPrecioGusanos.text = $"$ {(int)sliderGusanos.value}";
    }

    // -----------------------------------------------------------------------

    public void GuardarParametros()
    {
        GameRulesManager.instance.velocidadIncrementoHambre = sliderAumentoHambre.value;
        GameRulesManager.instance.velocidadReduccionHambre = sliderReduccionHambre.value;
        GameRulesManager.instance.velocidadIncrementofelicidad = sliderAumentofelicidad.value;
        GameRulesManager.instance.velocidadReduccionfelicidad = sliderReduccionfelicidad.value;
        GameRulesManager.instance.velocidadIncrementoPeso = sliderAumentoPeso.value;
        GameRulesManager.instance.velocidadReduccionPeso = sliderReduccionPeso.value;
        GameRulesManager.instance.foodDecreaseSpeed = sliderConsumoAlimento.value;

        GameRulesManager.instance.precioHarina = (int) sliderHarina.value;
        GameRulesManager.instance.precioMaiz = (int) sliderMaiz.value;
        GameRulesManager.instance.precioSoya = (int) sliderSoya.value;
        GameRulesManager.instance.precioGusanos = (int) sliderGusanos.value;


        //Activamos Flag de nuevos Parametros guardados
        GameRulesManager.instance.nuevosParametrosGuardados = true;
    }
}
