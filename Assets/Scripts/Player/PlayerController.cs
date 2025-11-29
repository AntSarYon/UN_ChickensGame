using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("UI de Player")]
    public PlayerUI pUI;

    [Header("Velocidad")]
    [SerializeField] private float speed;
    private float origSpeed;

    [Header("Cuerpo")]
    [SerializeField] private Transform orientationBody;
    [SerializeField] private SpriteRenderer spriteBody;

    // Referencia al RigidBody
    private Rigidbody mRb;
    private AudioSource mAudioSource;

    //Vector Input de Movimiento
    private Vector3 movementInput;

    [Header("Clips de Audio")]
    [SerializeField] private AudioClip ApplauseClip;

    // ----------------------------------------------------

    void Awake()
    {
        //Obtenemos referencia a componentes
        mRb = GetComponent<Rigidbody>();
        mAudioSource = GetComponent<AudioSource>();
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

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            movementInput.z = 1;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            movementInput.z = -1;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            movementInput.x = -1;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
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

        // Si se oprime la tecla C
        if (Input.GetKeyDown(KeyCode.C))
        {
            //Aplaudimos
            Applause();
        }

        // Hacemos que el Body siempre mire hacia donde se dirige el movimiento
        orientationBody.LookAt(transform.position + movementInput);
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

    // ----------------------------------------------------

    public void Applause()
    {
        //Reproducimos el sonido de Aplauso
        mAudioSource.PlayOneShot(ApplauseClip, 1);
    }
    
}
