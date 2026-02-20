using UnityEngine;
using UnityEngine.EventSystems; 
using System.Collections.Generic;// Para detectar el click

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
        // CLICK DERECHO = ABRIR MENÚ DE ACCIÓN
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            List<MenuOption> opciones = new List<MenuOption>();

            // 1. Calcular si el jugador puede pagar
            bool canPay = GameManager.instance.CanPayCardCost(myData.costs);
            bool isValidRitual = true;

            // 2. Si es ritual, calcular si hay algo que invocar
            if (myData.type.ToString() == "Ritual")
            {
                isValidRitual = SpiritDeck.instance.GetValidSpiritsForSummon(myData.summonRequirement).Count > 0;
            }

            // 3. Es tu turno y fase principal?
            bool isCorrectPhase = GameManager.instance.isPlayerTurn && GameManager.instance.currentPhase == GamePhase.MainPhase;

            // ¿El botón debe estar encendido? (Debe cumplir todo)
            bool canActivate = canPay && isValidRitual && isCorrectPhase;

            // 4. Crear el botón de "Activar"
            opciones.Add(new MenuOption
            {
                buttonText = "Activar",
                isInteractable = canActivate,
                onClickAction = () => { TryToPlayCard(); } // Si le dan click, ejecuta tu función de siempre
            });

            // Mostramos el menú con esta lista de opciones
            ActionMenuManager.instance.ShowMenu(opciones);
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

        // 2. Verificar Costos y Tipos
        if (myData != null)
        {
            if (GameManager.instance.CanPayCardCost(myData.costs))
            {
               // --- SI ES RITUAL ---
                if (myData.type.ToString() == "Ritual")
                {
                    List<CardData> validTargets = SpiritDeck.instance.GetValidSpiritsForSummon(myData.summonRequirement);

                    if (validTargets.Count > 0)
                    {
                        GameManager.instance.PayCardCost(myData.costs); 
                        Debug.Log($"Activando Ritual: {myData.cardName}. Elige un espíritu...");
                        
                        // 1. ABRIMOS LA VENTANA DE SELECCIÓN
                        SpiritDeck.instance.AbrirVentanaDeSeleccion(validTargets, (cartaElegida) => 
                        {
                            // 2. ESTO OCURRE CUANDO DAS CLICK A LA CARTA EN LA UI
                            GameManager.instance.SummonSpiritToField(cartaElegida);
                        });

                        Destroy(gameObject); // Destruimos la carta de ritual de la mano
                    }
                    else
                    {
                        Debug.Log("¡No hay espíritus en tu reserva que cumplan los requisitos de este ritual!");
                    }
                }
                // --- SI ES OTRA CARTA (Alma, Voluntad, etc.) ---
                else 
                {
                    GameManager.instance.PayCardCost(myData.costs);
                    Debug.Log($"¡Jugaste {myData.cardName} exitosamente!");
                    
                    // Aquí irían los efectos de esas otras cartas...

                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.Log("¡No tienes recursos suficientes para jugar esta carta!");
            }
        }
    }
    
}