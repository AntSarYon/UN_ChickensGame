using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignManager : MonoBehaviour
{
    public static CampaignManager Instance;

    private int campaignCounter;
    private int dayCounter;

    //Cash del jugador en la Campaña
    [HideInInspector] public float campaignCurrentCash;

    // Escala de perdida de dinero...
    [HideInInspector] public float campaignCashPassiveOutcome;

    //Cantidad de Pollitos Muertos en la Campaña
    [HideInInspector] public int campaignCurrentDeathChicken;


    public int CampaignCounter { get => campaignCounter; private set => campaignCounter = value; }
    public int DayCounter { get => dayCounter; private set => dayCounter = value; }

    // ---------------------------------------------------------------

    void Awake()
    {
        ManageInstance();

        //Los contadores inician en 1
        campaignCounter = 1;
        dayCounter = 1;

        //El dinero empieza en 300$
        campaignCurrentCash = 600.00f;

        //Empezamos con 0 pollitos muertos
        campaignCurrentDeathChicken = 0;

        //Iniciamos el valor de Salida Pasiva del Cash
        campaignCashPassiveOutcome = 1.5f;
    }

    // ---------------------------------------------------------------

    public void RestartToFirstCampaign()
    {
        //Regresamos al dia 1 de la Campaña inicial
        campaignCounter = 1;
        dayCounter = 1;
    }

    // ---------------------------------------------------------------

    public void IncreaseDay()
    {
        //Incrementamos el contador de dias
        dayCounter++;

        //Si pasa la semana...
        if (dayCounter > 5)
        {
            //Pasamos a la siguiente jornada
            IncreaseCampaign();
        }
    }

    // ---------------------------------------------------------------

    public void IncreaseCampaign()
    {
        //Pasamos a la siguiente Jornada
        campaignCounter++;

        //El contador de dias regresa a 1
        dayCounter = 1;
    }

    // ---------------------------------------------------------------

    public void UpdateCash(float newCash)
    {
        //Actualizamos el monto actual de Cash
        campaignCurrentCash = newCash;
    }

    // ---------------------------------------------------------------

    public void UpdateDeadChickens(int newDeadChickensCount)
    {
        //Incrementamos la cantidad de Politos muertos
        campaignCurrentDeathChicken += newDeadChickensCount;
    }

    // ---------------------------------------------------------------

    public void UpdateCashPassiveOutcome(float Difference)
    {
        //Modificamos el valor del Outcome pasivo segun parametro recibido
        campaignCashPassiveOutcome += Difference;
    }

    // ---------------------------------------------------------------

    private void ManageInstance()
    {
        //Si no hay instancia
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
