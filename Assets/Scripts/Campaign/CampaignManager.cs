using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignManager : MonoBehaviour
{
    public static CampaignManager Instance;

    private int campaignCounter;
    private int dayCounter;

    public int CampaignCounter { get => campaignCounter; private set => campaignCounter = value; }
    public int DayCounter { get => dayCounter; private set => dayCounter = value; }

    // ---------------------------------------------------------------

    void Awake()
    {
        //Los contadores inician en 1
        campaignCounter = 1;
        dayCounter = 1;

        ManageInstance();
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
