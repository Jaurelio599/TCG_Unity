using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic; // Necesario para la lista

public class FieldSpiritInteraction : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            List<MenuOption> opciones = new List<MenuOption>();

            // Opción 1: Atacar (Podemos dejarla encendida por ahora, o apagarla si no es tu turno)
            bool canAttack = GameManager.instance.isPlayerTurn; // Condición básica
            opciones.Add(new MenuOption
            {
                buttonText = "Atacar",
                isInteractable = canAttack,
                onClickAction = () => { Debug.Log("¡Modo ataque activado! Elige un objetivo..."); /* Futura función */ }
            });

            // Opción 2: Activar Efecto 
            // (Aquí luego evaluaremos si el espíritu tiene un efecto manual y si puedes pagarlo)
            opciones.Add(new MenuOption
            {
                buttonText = "Activar Efecto",
                isInteractable = false, // Por ahora apagado hasta que programemos efectos manuales
                onClickAction = () => { Debug.Log("Efecto activado."); }
            });

            // Opción 3: ¡Tu Instakill para pruebas!
            opciones.Add(new MenuOption
            {
                buttonText = "Destruir (Debug)",
                isInteractable = true,
                onClickAction = () => { MatarEspiritu(); }
            });

            ActionMenuManager.instance.ShowMenu(opciones);
        }
    }

    public void MatarEspiritu()
    {
        CardDisplay display = GetComponent<CardDisplay>();
        if (display != null && display.originalData != null)
        {
            display.TriggerDeath(); 
            SpiritDeck.instance.ReturnSpirit(display.originalData);
            Debug.Log($"[INSTAKILL] {display.originalData.cardName} fue destruido y devuelto al cementerio.");
        }
        Destroy(gameObject); 
    }
}