using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    // Texto de interaccion
    [SerializeField] private TextMeshProUGUI txtInteraction;
    private string interactionMessage;

    // ----------------------------------------------------------

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
