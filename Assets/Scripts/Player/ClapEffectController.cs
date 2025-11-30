using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClapEffectController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float effectDuration = 0.6f;

    void Start()
    {
        // Si no está asignado, obtener referencia automáticamente
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void PlayClap()
    {
        // Asegurar que el GameObject está activo
        gameObject.SetActive(true);

        if (animator != null)
        {
            // Reproducir desde el estado inicial sin parámetros
            animator.Play(0, 0, 0f);
            Debug.Log("[ClapEffectController] Animación de clap iniciada");
        }
        else
        {
            Debug.LogWarning("[ClapEffectController] Animator no asignado");
        }

        StartCoroutine(HideAfterDuration());
    }

    private IEnumerator HideAfterDuration()
    {
        yield return new WaitForSeconds(effectDuration);
        gameObject.SetActive(false);
        Debug.Log("[ClapEffectController] Efecto de clap desactivado");
    }
}
