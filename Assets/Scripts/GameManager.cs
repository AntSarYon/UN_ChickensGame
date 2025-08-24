using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Instancia de Clase
    public static GameManager Instance;

    //---------------------------------------------------
    void Awake()
    {

        //Controlamos que esta sea la unica instancia de la Clase GameManager
        ManageInstance();
    }

    // -------------------------------------------------------------------

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
