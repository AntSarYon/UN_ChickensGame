using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemperatureOverlay : MonoBehaviour
{
    [Header("Overlay Settings")]
    [SerializeField] private Image overlayImage;
    [SerializeField] private float minTemp = 0f;
    [SerializeField] private float maxTemp = 40f;
    [SerializeField] private float overlayMaxAlpha = 0.3f; // Transparencia máxima del overlay

    private Yard assignedYard;

    void Start()
    {
        // Si no hay Image asignada, crear una automáticamente
        if (overlayImage == null)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                GameObject overlayObj = new GameObject("TemperatureOverlay");
                overlayObj.transform.SetParent(canvas.transform, false);
                
                RectTransform rectTransform = overlayObj.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                overlayImage = overlayObj.AddComponent<Image>();
                overlayImage.raycastTarget = false;
            }
        }

        // Buscar el Yard en la escena
        assignedYard = FindObjectOfType<Yard>();
        
        if (assignedYard != null)
        {
            assignedYard.OnTemperatureChanged += UpdateOverlay;
            // Actualizar inicialmente
            UpdateOverlay(assignedYard.temperature);
        }
    }

    private void OnDestroy()
    {
        if (assignedYard != null)
        {
            assignedYard.OnTemperatureChanged -= UpdateOverlay;
        }
    }

    private void UpdateOverlay(float currentTemperature)
    {
        if (overlayImage == null) return;

        // Normalizar temperatura a rango 0..1
        float t = Mathf.Clamp01((currentTemperature - minTemp) / (maxTemp - minTemp));

        // Interpolar entre Azul (frío) -> Transparente -> Rojo (caliente)
        Color overlayColor;
        float alpha;

        if (t < 0.5f)
        {
            // Primera mitad: Azul (0) a Transparente (0.5)
            float localT = t * 2f; // Remap 0..0.5 to 0..1
            overlayColor = Color.Lerp(new Color(0.2f, 0.5f, 1f), Color.clear, localT); // Azul a Transparente
            alpha = Mathf.Lerp(overlayMaxAlpha, 0f, localT);
        }
        else
        {
            // Segunda mitad: Transparente (0.5) a Rojo (1)
            float localT = (t - 0.5f) * 2f; // Remap 0.5..1 to 0..1
            overlayColor = Color.Lerp(Color.clear, new Color(1f, 0.2f, 0.2f), localT); // Transparente a Rojo
            alpha = Mathf.Lerp(0f, overlayMaxAlpha, localT);
        }

        overlayColor.a = alpha;
        overlayImage.color = overlayColor;
    }
}
