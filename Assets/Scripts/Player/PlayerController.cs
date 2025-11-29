using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    private float origSpeed;
    private Rigidbody mRb;
    private Vector3 movementInput;

    // ----------------------------------------------------

    void Awake()
    {
        //Obtenemos referencia a componentes
        mRb = GetComponent<Rigidbody>();
    }

    // -------------------------------------------------------

    void Start()
    {
        // Almacenamos la velocidad original
        origSpeed = speed;
    }

    // ----------------------------------------------------
    void Update()
    {
        //Input empieza en Zero
        movementInput = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            movementInput.z = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movementInput.z = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementInput.x = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            movementInput.x = 1;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            speed = 2.00f;
        } 
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 5.00f;
        } 
        else speed = origSpeed;
    }

    // ---------------------------------------------------

    void FixedUpdate()
    {
        Move(movementInput);
    }

    // ----------------------------------------------

    public void Move(Vector3 direction)
    {
        mRb.MovePosition(mRb.position + direction.normalized * speed * Time.fixedDeltaTime);
    }
    
}
