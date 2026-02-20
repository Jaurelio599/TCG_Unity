using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDetailsManager : MonoBehaviour
{
    public static CardDetailsManager instance;

    [Header("Panel Principal")]
    public GameObject detailsPanel; // El fondo oscuro o ventana lateral

    [Header("Referencias UI")]
    public Image cardArtBig;
    public TMP_Text cardNameText;
    public TMP_Text descriptionText;
    public TMP_Text statsText;

    [Header("Botón de Acción")]
    public Button actionButton;
    public TMP_Text actionButtonText;

    // Variables internas para saber qué carta estamos viendo y qué hacer con ella
    private CardData currentCard;
    private System.Action<CardData> currentActionCallback;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        // Asegurarnos de que el panel empiece apagado
        if (detailsPanel != null) detailsPanel.SetActive(false);
    }

    // --- FUNCIÓN PRINCIPAL PARA MOSTRAR DETALLES ---
    // Recibe: La carta a mostrar, el texto del botón (ej. "Invocar") y la acción a realizar.
    public void ShowCardDetails(CardData card, string buttonText, System.Action<CardData> onConfirmAction = null)
    {
        currentCard = card;
        currentActionCallback = onConfirmAction;

        // 1. Llenar la información visual
        if (cardArtBig != null) cardArtBig.sprite = card.art;
        if (cardNameText != null) cardNameText.text = card.cardName;
        if (descriptionText != null) descriptionText.text = card.description;
        if (statsText != null) statsText.text = $"PD: {card.basePd}   PS: {card.basePs}";

        // 2. Configurar el botón
        if (actionButton != null)
        {
            if (onConfirmAction != null)
            {
                // Si hay una acción (ej. Invocar), mostramos el botón
                actionButton.gameObject.SetActive(true);
                if (actionButtonText != null) actionButtonText.text = buttonText;

                // Limpiamos clicks anteriores y le damos la nueva orden
                actionButton.onClick.RemoveAllListeners();
                actionButton.onClick.AddListener(OnActionButtonClicked);
            }
            else
            {
                // Si solo la estamos inspeccionando (ej. una carta en el campo), ocultamos el botón
                actionButton.gameObject.SetActive(false);
            }
        }

        // 3. Encender el panel
        if (detailsPanel != null) detailsPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        if (detailsPanel != null) detailsPanel.SetActive(false);
        currentCard = null;
        currentActionCallback = null;
    }

    private void OnActionButtonClicked()
    {
        // Si le dan click al botón, ejecutamos la acción (ej. invocarla) y cerramos el panel
        if (currentActionCallback != null && currentCard != null)
        {
            currentActionCallback.Invoke(currentCard);
        }
        ClosePanel();
    }
}