using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillingZoneController : MonoBehaviour
{
    //Flag 
    private bool flagChickenInSale;


    // COMPONENTES
    private Animator mAnimator;

    //----------------------------------------------------------

    void Awake()
    {
        //Iniciamos flag de Pollitoen Venta en Falso
        flagChickenInSale = false;

       mAnimator = GetComponent<Animator>();
    }

    //----------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //-------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Chicken"))
        {
            // Activamos Flag de - ChickenForSale
            Debug.Log("Gallina en zona de Cash");

            mAnimator.SetBool("Hover",true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Chicken"))
        {
            Debug.Log("Gallina salió de zona de Cash");

            mAnimator.SetBool("Hover", false);
        }
    }

    private void OnMouseUp()
    {
        
    }
}
