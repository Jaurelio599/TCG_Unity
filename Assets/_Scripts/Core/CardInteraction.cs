using UnityEngine;
using UnityEngine.EventSystems; // Para detectar el click

public class CardInteraction : MonoBehaviour, IPointerClickHandler
{
    private CardDisplay display;
    private CardData myData;

    void Start()
    {
        display = GetComponent<CardDisplay>();
        if (display != null)
        {
            myData = display.originalData;
        }
    }

    // Detectar Click Derecho para "Jugar" la carta
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            TryToPlayCard();
        }
    }

    public void TryToPlayCard()
    {
        // 1. Verificar si es mi turno y estoy en Main Phase
        if (!GameManager.instance.isPlayerTurn || GameManager.instance.currentPhase != GamePhase.MainPhase)
        {
            Debug.Log("¡No puedes jugar cartas ahora!");
            return;
        }

        // 2. Verificar Costos
        if (myData != null)
        {
            // PREGUNTA AL GAMEMANAGER: ¿Me alcanza?
            if (GameManager.instance.CanPayCardCost(myData.costs))
            {
                // COBRAR
                GameManager.instance.PayCardCost(myData.costs);

                Debug.Log($"¡Jugaste {myData.cardName} exitosamente!");
                
                // AQUÍ ACTIVARÍAS EL EFECTO DE LA CARTA
                // ActivateEffect();

                // Destruir la carta de la mano (porque ya se jugó)
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("¡No tienes recursos suficientes para jugar esta carta!");
                // Aquí podrías poner un sonido de error o un parpadeo rojo
            }
        }
    }
}