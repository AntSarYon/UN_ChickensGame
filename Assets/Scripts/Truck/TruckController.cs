using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviour
{
    private Animator mAnimator;
    public static TruckController Instance;
    [HideInInspector] public bool bIsArriving;

    // ------------------------------------------
    void Awake()
    {
        //Flag de "Llegando" inicia en falso
        bIsArriving = false;

        Instance = this;

        mAnimator = GetComponent<Animator>();
    }

    // ------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        mAnimator.Play("hide");
    }

    // -----------------------------------------

    public void PlayArrive()
    {
        bIsArriving = true;
        mAnimator.Play("arrive");
    }

    public void PlayRun()
    {
        bIsArriving = false;
        mAnimator.Play("Run");
    }
}
