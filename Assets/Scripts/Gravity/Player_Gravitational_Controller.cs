using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Gravitational_Controller : MonoBehaviour
{
    public float moveSpeed = 15.00f;
    private Vector3 moveDir;

    private Rigidbody mRb;

    // -------------------------------------------------------

    void Awake()
    {
        mRb = GetComponent<Rigidbody>();
    }

    // -------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Obtenemos direccion de movimiento desde el input
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
    }

    // ------------------------------------------------------------
    private void FixedUpdate()
    {
        //Hacemos que el RigidBody kinematico se mueva hacia cierta direccion...
        mRb.MovePosition(transform.position + transform.TransformDirection(moveDir) * moveSpeed * Time.deltaTime);

        // TransformDirecion convierte unna direccion de global a local
        // Esto permite que la direccion de movimiento actua en funcion de la posicion y rotacion ocal del player
    }
}
