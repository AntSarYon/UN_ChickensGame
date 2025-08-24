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

    [SerializeField] private Slider sliderReduccionEstres;
    [SerializeField] private TextMeshProUGUI txtReduccionEstres;

    [SerializeField] private Slider sliderAumentoEstres;
    [SerializeField] private TextMeshProUGUI txtAumentoEstres;

    [SerializeField] private Slider sliderEstresPelear;
    [SerializeField] private TextMeshProUGUI txtEstresPelear;

    [SerializeField] private Slider sliderAumentoPeso;
    [SerializeField] private TextMeshProUGUI txtAumentoPeso;

    [SerializeField] private Slider sliderReduccionPeso;
    [SerializeField] private TextMeshProUGUI txtReduccionPeso;

    [SerializeField] private Slider sliderReduccionSalud;
    [SerializeField] private TextMeshProUGUI txtReduccionSalud;


    [SerializeField] private Slider sliderHoraInicio;
    [SerializeField] private TextMeshProUGUI txtHoraInicio;

    [SerializeField] private Slider sliderHoraFin;
    [SerializeField] private TextMeshProUGUI txtHoraFin;

    [SerializeField] private Slider sliderEscalaTiempo;
    [SerializeField] private TextMeshProUGUI txtEscalaTiempo;


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
            sliderReduccionEstres.value = GameRulesManager.instance.velocidadReduccionEstres;
            sliderAumentoEstres.value = GameRulesManager.instance.velocidadIncrementoEstres;
            sliderEstresPelear.value = GameRulesManager.instance.estresParaPelear;
            sliderAumentoPeso.value = GameRulesManager.instance.velocidadIncrementoPeso;
            sliderReduccionPeso.value = GameRulesManager.instance.velocidadReduccionPeso;
            sliderReduccionSalud.value = GameRulesManager.instance.velocidadReduccionHP;
            sliderHoraInicio.value = GameRulesManager.instance.startingTime;
            sliderHoraFin.value = GameRulesManager.instance.finishTime;
            sliderEscalaTiempo.value = GameRulesManager.instance.timeScale;
        }
        // casoc ontrario, los que estan por defecto
        else
        {
            sliderAumentoHambre.value = 2;
            sliderReduccionHambre.value = 4;
            sliderConsumoAlimento.value = 3;
            sliderReduccionEstres.value = 3;
            sliderAumentoEstres.value = 1;
            sliderEstresPelear.value = 65;
            sliderAumentoPeso.value = 0.15f;
            sliderReduccionPeso.value = 0.1f;
            sliderReduccionSalud.value = 5;
            sliderHoraInicio.value = 9;
            sliderHoraFin.value = 18;
            sliderEscalaTiempo.value = 2;
        }

    }

    // -----------------------------------------------------------------------

    void Update()
    {
        txtAumentoHambre.text = sliderAumentoHambre.value.ToString("F2");
        txtReduccionHambre.text = sliderReduccionHambre.value.ToString("F2");

        txtConsumoAlimento.text = sliderConsumoAlimento.value.ToString("F2");

        txtReduccionEstres.text = sliderReduccionEstres.value.ToString("F2");
        txtAumentoEstres.text = sliderAumentoEstres.value.ToString("F2");
        txtEstresPelear.text = sliderEstresPelear.value.ToString("F2");

        txtAumentoPeso.text = sliderAumentoPeso.value.ToString("F2");
        txtReduccionPeso.text = sliderReduccionPeso.value.ToString("F2");

        txtReduccionSalud.text = sliderReduccionSalud.value.ToString("F2");

        txtHoraInicio.text = sliderHoraInicio.value.ToString("F2");
        txtHoraFin.text = sliderHoraFin.value.ToString("F2");
        txtEscalaTiempo.text = sliderEscalaTiempo.value.ToString("F2");
    }

    // -----------------------------------------------------------------------

    public void GuardarParametros()
    {
        GameRulesManager.instance.velocidadIncrementoHambre = sliderAumentoHambre.value;
        GameRulesManager.instance.velocidadReduccionHambre = sliderReduccionHambre.value;
        GameRulesManager.instance.velocidadIncrementoEstres = sliderAumentoEstres.value;
        GameRulesManager.instance.velocidadReduccionEstres = sliderReduccionEstres.value;
        GameRulesManager.instance.estresParaPelear = sliderEstresPelear.value;
        GameRulesManager.instance.velocidadIncrementoPeso = sliderAumentoPeso.value;
        GameRulesManager.instance.velocidadReduccionPeso = sliderReduccionPeso.value;
        GameRulesManager.instance.velocidadReduccionHP = sliderReduccionSalud.value;
        GameRulesManager.instance.foodDecreaseSpeed = sliderConsumoAlimento.value;
        GameRulesManager.instance.startingTime = (int)sliderHoraInicio.value;
        GameRulesManager.instance.finishTime = (int)sliderHoraFin.value;
        GameRulesManager.instance.timeScale = sliderEscalaTiempo.value;

        //Activamos Flag de nuevos Parametros guardados
        GameRulesManager.instance.nuevosParametrosGuardados = true;
    }
}
