using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    // Texto de interaccion
    [SerializeField] private TextMeshProUGUI txtInteraction;
    private string interactionMessage;

    // ----------------------------------------------------------

    [Header("Stamina")]
    [SerializeField] private Slider staminaSlider;

    // Establece el valor normalizado (0..1) de la barra de stamina
    public void UpdateStamina(float normalized)
    {
        // Si por orden de ejecución el slider aún no está asignado, intentamos encontrarlo ahora
        if (staminaSlider == null)
        {
            staminaSlider = GetComponentInChildren<Slider>();
            if (staminaSlider == null)
            {
                staminaSlider = FindObjectOfType<Slider>();
            }

            if (staminaSlider != null)
            {
                staminaSlider.minValue = 0f;
                staminaSlider.maxValue = 1f;
                staminaSlider.gameObject.SetActive(true);
            }
        }

        if (staminaSlider != null)
        {
            staminaSlider.value = Mathf.Clamp01(normalized);
        }
    }

    void Start()
    {
        // Intentamos localizar el Slider de stamina en la escena si no está asignado
        if (staminaSlider == null)
        {
            // Buscar en hijos de este componente primero
            staminaSlider = GetComponentInChildren<Slider>();

            // Si no, buscar todos los Sliders en la Canvas y ver cuál tiene nombre relacionado a "Stamina"
            if (staminaSlider == null)
            {
                Slider[] allSliders = FindObjectsOfType<Slider>();
                foreach (Slider slider in allSliders)
                {
                    if (slider.gameObject.name.Contains("Stamina") || slider.gameObject.name.Contains("stamina") || slider.gameObject.name.Contains("Energy"))
                    {
                        staminaSlider = slider;
                        Debug.Log($"[PlayerUI] Stamina Slider encontrado por nombre: {slider.gameObject.name}");
                        break;
                    }
                }
            }

            // Si aún no lo encontramos, usar el primer Slider que encuentre (último recurso)
            if (staminaSlider == null)
            {
                staminaSlider = FindObjectOfType<Slider>();
                if (staminaSlider != null)
                {
                    Debug.LogWarning($"[PlayerUI] Stamina Slider no encontrado por nombre, usando el primer Slider de la escena: {staminaSlider.gameObject.name}");
                }
            }
        }

        if (staminaSlider != null)
        {
            // Normalizamos el slider a 0..1 (el PlayerController manda valores normalizados)
            staminaSlider.minValue = 0f;
            staminaSlider.maxValue = 1f;
            staminaSlider.value = 1f;
            // Aseguramos que esté activo para que sea visible
            staminaSlider.gameObject.SetActive(true);
            Debug.Log($"[PlayerUI] Stamina Slider inicializado correctamente: {staminaSlider.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("PlayerUI: staminaSlider no asignado y no fue encontrado en la escena.");
        }
    }

    public void SetInteractionMessage(string interaction)
    {
        interactionMessage = $"[E] {interaction}";
    }

    // ----------------------------------------------------------

    public void ShowInteractionMessage()
    {
        txtInteraction.text = interactionMessage;
        txtInteraction.gameObject.SetActive(true);
    }

    public void HideInteractionMessage()
    {
        txtInteraction.gameObject.SetActive(false);
    }

    // ----------------------------------------------------------
}
