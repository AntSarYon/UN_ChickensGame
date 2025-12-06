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
    [Tooltip("Multiplicador aplicado al radio visual del Applause Area para el rango de interacción (1 = escala visual exacta).")]
    [SerializeField] private float applauseRadiusMultiplier = 1f;
    private float areaInterpolation;
    private float areaIncreaseSpeed;

    //Flags
    private bool bClapped;
    [HideInInspector] public bool bisCarryingFood;

    // Stamina / Sprint
    [Header("Stamina")]
    [SerializeField] private float maxStamina = 5f; // seconds of sprint
    [SerializeField] private float staminaDrainPerSecond = 1f;
    [SerializeField] private float staminaRegenPerSecond = 0.75f;
    [SerializeField] private float sprintMultiplier = 1.8f; // how much faster when sprinting
    [SerializeField] private float staminaCostPerApplause = 0.5f; // stamina consumed per clap
    private float currentStamina;
    private bool isSprinting = false;
    [Header("Debug")]
    [Tooltip("Habilita logs de stamina en la consola para depuración")]
    [SerializeField] private bool debugStamina = false;

    // Referencia al RigidBody
    private Rigidbody mRb;
    private AudioSource mAudioSource;

    [Header("Animator")]
    [SerializeField] private Animator mAnimator;

    //Vector Input de Movimiento
    [HideInInspector] public Vector3 movementInput;

    [Header("Clips de Audio")]
    [SerializeField] private AudioClip ApplauseClip;

    [Header("Clap Effect")]
    [SerializeField] private GameObject clapEffectGameObject;
    private Animator clapAnimator;

    // ----------------------------------------------------

    void Awake()
    {
        //Obtenemos referencia a componentes
        mRb = GetComponent<Rigidbody>();
        mAudioSource = GetComponent<AudioSource>();

        //Actualizamos la escala del Area en base a la interpolacion
        applauseArea.localScale = minRadioScale;

        //Flag de "Aplaude" y "Cargando comida" empieza en false
        bClapped = false;
        bisCarryingFood = false;

        areaInterpolation = 0.00f;
        areaIncreaseSpeed = 3.00f;
    }

    // -------------------------------------------------------

    void Start()
    {
        // Almacenamos la velocidad original
        origSpeed = speed;
        currentStamina = maxStamina;
        // Si no se asignó `pUI` en el Inspector, intentamos localizarlo en hijos o en la escena
        if (pUI == null)
        {
            pUI = GetComponentInChildren<PlayerUI>();
            if (pUI == null)
            {
                pUI = FindObjectOfType<PlayerUI>();
            }

            if (pUI == null)
            {
                Debug.LogWarning("PlayerController: pUI no asignado y no se encontró un PlayerUI en la escena.");
            }
            else
            {
                Debug.Log("PlayerController: pUI auto-asignado desde la escena.");
            }
        }

        // Obtener referencia al Animator del GameObject de clap si está asignado
        if (clapEffectGameObject != null)
        {
            clapAnimator = clapEffectGameObject.GetComponent<Animator>();
            if (clapAnimator == null)
            {
                clapAnimator = clapEffectGameObject.GetComponentInChildren<Animator>();
            }
            // Desactivar inicialmente
            clapEffectGameObject.SetActive(false);
        }
    }

    // ----------------------------------------------------
    void Update()
    {
        //Controlamos los Inputs de movimiento
        ManageMovementInput();

        // Controlamos el sprinting y la stamina
        ManageSprintingAndStamina();

        //Controlamos la Orientacion
        ManageBodyOrientation();

        // Controlamos la accion de Aplaudir
        ManageApplause();

        // Controlamos la Annimacion de Movimiento
        ManageMoveAnimation();
    }

    // -------------------------------------------------------------------------------

    private void ManageMovementInput()
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

        // Crouch (override)
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isSprinting = false;
            speed = 2.00f;
        }
        else
        {
            // Sprinting when holding LeftShift and moving and having stamina
            bool wantSprint = Input.GetKey(KeyCode.LeftShift) && movementInput != Vector3.zero;

            if (wantSprint && currentStamina > 0f)
            {
                mAnimator.SetBool("Running", true);
                isSprinting = true;
                speed = origSpeed * sprintMultiplier;
            }
            else
            {
                mAnimator.SetBool("Running", false);
                isSprinting = false;
                speed = origSpeed;
            }
        }
    }

    // ---------------------------------------------------------------------------

    private void ManageSprintingAndStamina()
    {
        // Manejo de stamina: consumir si sprinting, recargar si no
        if (isSprinting)
        {
            currentStamina -= staminaDrainPerSecond * Time.deltaTime;
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                isSprinting = false; // stop sprint when exhausted
                speed = origSpeed;
            }
        }
        else
        {
            // Regenerar stamina
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenPerSecond * Time.deltaTime;
                if (currentStamina > maxStamina) currentStamina = maxStamina;
            }
        }

        // Actualizar UI de Stamina
        if (pUI != null)
        {
            float normalized = currentStamina / Mathf.Max(0.0001f, maxStamina);
            if (debugStamina)
            {
                Debug.Log($"[PlayerController] Stamina: {currentStamina:F2} / {maxStamina:F2} -> normalized {normalized:F2} | isSprinting={isSprinting}");
            }

            //Lamamos a la UI para actualizar el valor de Stamina
            UIController.Instance.UpdateStamina(normalized);
        }
    }

    // -------------------------------------------------------------------------------

    private void ManageBodyOrientation()
    {
        // Hacemos que el Body siempre mire hacia donde se dirige el movimiento
        if (movementInput != Vector3.zero)
            orientationBody.LookAt(transform.position + movementInput);
        //Cuando no se este moviendo, seguira apuntando a la utima direccion inputada
    }

    // -------------------------------------------------------------------------------

    private void ManageMoveAnimation()
    {
        // Asignamos los valores de X e Y en base a los inputs
        mAnimator.SetFloat("X", movementInput.x);
        mAnimator.SetFloat("Y", movementInput.z);

        // Activamos Flag de Anim en base a movimiento del Player
        if (movementInput != Vector3.zero)
            mAnimator.SetBool("Moving", true);
        else 
            mAnimator.SetBool("Moving", false);

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            mAnimator.SetFloat("idleY", 1);
            mAnimator.SetFloat("idleX", 0);
        }
        else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            mAnimator.SetFloat("idleY", -1);
            mAnimator.SetFloat("idleX", 0);
        }
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            mAnimator.SetFloat("idleY", 0);
            mAnimator.SetFloat("idleX", -1);
        }
        else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            mAnimator.SetFloat("idleY", 0);
            mAnimator.SetFloat("idleX", 1);
        }
    }

    // --------------------------------------------------------------------------------

    private void ManageApplause()
    {
        // Si se oprime la tecla C
        if (Input.GetKeyDown(KeyCode.C))
        {
            //Aplaudimos
            Applause();
        }

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

    // -------------------------------------------------------------------------------

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

        // Activar el efecto de clap
        if (clapEffectGameObject != null)
        {
            clapEffectGameObject.SetActive(true);
            Debug.Log("[PlayerController] Efecto de clap activado");
            
            // Desactivar después de que termine la animación
            if (clapAnimator != null)
            {
                StartCoroutine(DeactivateClapAfterAnimation());
            }
        }

        // Consumir stamina por aplauso
        currentStamina -= staminaCostPerApplause;
        if (currentStamina < 0f) currentStamina = 0f;

        if (debugStamina)
        {
            Debug.Log($"[PlayerController] Aplauso realizado. Stamina consumida: {staminaCostPerApplause}. Stamina actual: {currentStamina:F2}");
        }

        // Despertar y hacer escapar a los pollos que estén dentro del radio del aplauso
        // Usamos la escala actual del objeto visual `applauseArea` para determinar el radio
        float visualRadius = Mathf.Max(applauseArea.localScale.x, applauseArea.localScale.z);
        float radius = visualRadius * Mathf.Max(0.0001f, applauseRadiusMultiplier);
        Collider[] hits = Physics.OverlapSphere(applauseArea.position, radius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Chicken"))
            {
                ChickenController ch = hit.GetComponent<ChickenController>();
                if (ch != null)
                {
                    // Double-check the distance to avoid waking chickens whose colliders touch the sphere but are visually outside
                    float sqrDist = (ch.transform.position - applauseArea.position).sqrMagnitude;
                    if (sqrDist <= radius * radius)
                    {
                        // Forzar despertar (maneja tanto sueño aleatorio como por temperatura)
                        ch.WakeUpForApplause();

                        // Hacer que salga del círculo
                        ch.RunAwayFromApplause(applauseArea.position);
                    }
                }
            }
        }
    }

    private IEnumerator DeactivateClapAfterAnimation()
    {
        // Esperar a que la animación termine (aproximadamente 0.6 segundos)
        yield return new WaitForSeconds(0.6f);
        clapEffectGameObject.SetActive(false);
        Debug.Log("[PlayerController] Efecto de clap desactivado");
    }
    
}
