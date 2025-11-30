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

    [Header("Area de influencia")]
    [SerializeField] private Transform applauseArea;
    [SerializeField] private Vector3 maxRadioScale = new Vector3(4.45f, 0.0085f, 4.45f);
    [SerializeField] private Vector3 minRadioScale = Vector3.zero;
    private float areaInterpolation;
    private float areaIncreaseSpeed;
    private bool bClapped;

    // Referencia al RigidBody
    private Rigidbody mRb;
    private AudioSource mAudioSource;

    [Header("Animator")]
    [SerializeField] private Animator mAnimator;

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

        //Actualizamos la escala del Area en base a la interpolacion
        applauseArea.localScale = minRadioScale;

        //Flag de "Aplaudi�" empieza en false
        bClapped = false;

        areaInterpolation = 0.00f;
        areaIncreaseSpeed = 3.00f;
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

        mAnimator.SetFloat("X", movementInput.x);
        mAnimator.SetFloat("Y", movementInput.z);

        // Si el player se esta moviendo...
        if (movementInput != Vector3.zero)
        {
            mAnimator.SetBool("Moving", true);
        }
        else
        {
            mAnimator.SetBool("Moving", false);
        }

        // Hacemos que el Body siempre mire hacia donde se dirige el movimiento
        orientationBody.LookAt(transform.position + movementInput);

        //Si el flag de "Aplaudi�" esta activo...
        if (bClapped)
        {
            //Incrementamos el valor de interpolacion
            areaInterpolation += Time.deltaTime * areaIncreaseSpeed;

            //Actualizamos la escala del Area en base a la interpolacion
            applauseArea.localScale = Vector3.Lerp(minRadioScale, maxRadioScale, areaInterpolation);

            // Si la interpolacion llega a 0
            if (areaInterpolation >= 1)
            {
                //Desactivamos el flag de "Aplaudi�"
                bClapped = false;

                //La retornamos a 0
                areaInterpolation = 0;

                //Restauramos el aplauso
                Invoke(nameof(RestoreApplauseArea), 0.35f);

                
            }
        }
    }

    // ---------------------------------------------------

    private void RestoreApplauseArea()
    {
        //Actualizamos la escala del Area en base a la interpolacion
        applauseArea.localScale = minRadioScale;
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

        //Activamos flag de "Aplaudi�"
        bClapped = true;

        // Despertar y hacer escapar a los pollos que estén dentro del radio del aplauso
        float radius = Mathf.Max(maxRadioScale.x, maxRadioScale.z);
        Collider[] hits = Physics.OverlapSphere(applauseArea.position, radius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Chicken"))
            {
                ChickenController ch = hit.GetComponent<ChickenController>();
                if (ch != null)
                {
                    // Si estaba dormido, despertarlo
                    if (ch.sleepingFlag)
                    {
                        ch.WakeFromSleep();
                    }

                    // Hacer que salga del círculo
                    ch.RunAwayFromApplause(applauseArea.position);
                }
            }
        }
    }
    
}
