using UnityEngine;
using UnityEngine.EventSystems;

public class FieldSpiritInteraction : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // CLICK DERECHO = INSTAKILL
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            MatarEspiritu();
        }
    }

    public void MatarEspiritu()
    {
        CardDisplay display = GetComponent<CardDisplay>();
        if (display != null && display.originalData != null)
        {
            // 1. Activar efectos de Muerte (ej. Darte sellos)
            display.TriggerDeath(); 

            // 2. Devolver la carta al cementerio (SpiritDeck)
            SpiritDeck.instance.ReturnSpirit(display.originalData);

            Debug.Log($"[INSTAKILL] {display.originalData.cardName} fue destruido y devuelto al cementerio.");
        }

        // 3. Destruir el objeto físico del campo
        Destroy(gameObject); 
    }
}