using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CardDisplay : MonoBehaviour
{
    [Header("Card Data")]
    public CardData originalData;

    [Header("Basic UI References")]
    public Image artImage;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text pdText;
    public TMP_Text psText;

    [Header("New UI References")]
    public Image elementIconSlot;
    public TMP_Text archetypeText;

    [Header("Containers")]
    public Transform lateralStatesContainer;
    public Transform bottomEffectsContainer;

    [Header("Prefabs")]
    public GameObject stateIconPrefab;
    public GameObject effectIndicatorPrefab;

    // Variables internas
    private int currentPd;
    private int currentPs;
    [Header("Cost UI")]
    public Transform costContainer;   // El objeto en la esquina superior derecha
    public GameObject costPrefab;     // Un prefab simple: Icono + Texto (ej. Gota de agua + "1")

    void Start()
    {
        if (originalData != null)
        {
            LoadCardData();
            UpdateUI();
        }
    }

    public void LoadCardData()
    {
        if (originalData != null)
        {
            currentPd = originalData.basePd;
            currentPs = originalData.basePs;
        }
    }

    public void UpdateUI()
    {
        if (originalData == null) return;

        // 1. Textos básicos y Arte (Con chequeos de seguridad)
        if (nameText) nameText.text = originalData.cardName;
        if (descriptionText) descriptionText.text = originalData.description;
        if (pdText) pdText.text = currentPd.ToString();
        if (psText) psText.text = currentPs.ToString();
        if (archetypeText) archetypeText.text = originalData.archetype;     
        if (artImage && originalData.art) artImage.sprite = originalData.art;

       // 2. Icono del Elemento
        if (elementIconSlot != null)
        {
            // Protegemos el acceso a GameAssets
            if (GameAssets.i != null)
            {
                // ¡AQUÍ ESTÁ EL ARREGLO! Solo agregamos .ToString() al final de element
                elementIconSlot.sprite = GameAssets.i.GetElementSprite(originalData.element.ToString());
                elementIconSlot.gameObject.SetActive(elementIconSlot.sprite != null);
            }
        }

        // 3. Barra Lateral (Passive States)
        FillStateIcons(lateralStatesContainer, originalData.passiveStates);

        // 4. Área de Efectos (Bottom Container)
        ClearContainer(bottomEffectsContainer);
        
        if (GameAssets.i != null)
        {
            FillEffectIndicators(bottomEffectsContainer, originalData.onSummonEffects, GameAssets.i.iconOnSummon);
            FillEffectIndicators(bottomEffectsContainer, originalData.onDeathEffects, GameAssets.i.iconOnDeath);
        }

        // 5. Costos
        if (costContainer != null && costPrefab != null)
        {
            // 1. Limpiar lo viejo
            foreach (Transform child in costContainer) Destroy(child.gameObject);

            // 2. Pintar los nuevos costos
            foreach (var cost in originalData.costs)
            {
                GameObject newCost = Instantiate(costPrefab, costContainer);
                
                // 1. Buscamos la Imagen en el objeto principal (o en los hijos, por si acaso)
                Image iconImg = newCost.GetComponent<Image>();
                if (iconImg == null) iconImg = newCost.GetComponentInChildren<Image>();

                // 2. Buscamos el texto
                TMP_Text amountTxt = newCost.GetComponentInChildren<TMP_Text>();

                // Asignar Icono
                if (iconImg != null && GameAssets.i != null)
                {
                    iconImg.sprite = GameAssets.i.GetCostIcon(cost.type);
                    iconImg.color = Color.white; 
                }

                // Asignar Texto
                if (amountTxt != null)
                {
                    amountTxt.text = cost.amount.ToString();
                }
            }
        }
    }

    // --- LOGICA VISUAL ---

    private void ClearContainer(Transform container)
    {
        if (container == null) return;
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

   private void FillStateIcons(Transform container, List<StateData> statesList)
{
    if (container == null || stateIconPrefab == null || statesList == null) return;

    // --- AGREGA ESTO: LIMPIEZA PREVIA ---
    // Borramos los hijos que ya existan para no duplicarlos
    foreach (Transform child in container)
    {
        Destroy(child.gameObject);
    }
    // ------------------------------------

    foreach (var state in statesList)
    {
        if (state.icon != null)
        {
            GameObject newIcon = Instantiate(stateIconPrefab, container);
            Image iconImg = newIcon.GetComponent<Image>();
            if (iconImg != null) iconImg.sprite = state.icon;
        }
    }
}

    private void FillEffectIndicators(Transform container, List<EffectData> effectsList, Sprite triggerIcon)
    {
        if (container == null || effectIndicatorPrefab == null || effectsList == null) return;

        foreach (var effect in effectsList)
        {
            SealEffect sealEffect = effect as SealEffect; // Asumiendo que usas SealEffect para lógica visual
            
            // Si quieres mostrar todos los efectos aunque no sean SealEffect, quita esta condición
            // Pero mantengo tu lógica original aquí:
            if (sealEffect != null)
            {
                GameObject indicator = Instantiate(effectIndicatorPrefab, container);

                if (indicator.transform.childCount < 3)
                {
                    // Debug.LogError($"[CardDisplay] Prefab incompleto."); // Comentado para no spamear si no es crítico
                    continue; 
                }

                var triggerImg = indicator.transform.GetChild(0).GetComponent<Image>();
                var sealImg = indicator.transform.GetChild(1).GetComponent<Image>();
                var amountTxt = indicator.transform.GetChild(2).GetComponent<TMP_Text>();

                if (triggerImg != null) triggerImg.sprite = triggerIcon;
                if (amountTxt != null) amountTxt.text = "x" + sealEffect.amount.ToString();
              // Le quitamos la palabra "Seal" (ej. "WaterSeal" -> "Water") para que busque bien la imagen
        if (sealImg != null && GameAssets.i != null) 
            sealImg.sprite = GameAssets.i.GetElementSprite(sealEffect.sealType.ToString().Replace("Seal", ""));
            }
        }
    }

    // --- TRIGGERS DE COMBATE ---
    public void TriggerSummon()
    {
        if (originalData != null) ExecuteEffects(originalData.onSummonEffects);
    }

    public void TriggerDeath()
    {
        if (originalData != null) ExecuteEffects(originalData.onDeathEffects);
    }

    private void ExecuteEffects(List<EffectData> effectsList)
    {
        if (effectsList == null) return;
        foreach (var effect in effectsList)
        {
            effect.Activate(this);
        }
    }

    // --- FUNCIONES DE CONFIGURACIÓN ---

    // Esta es tu función original
    public void SetUpCard(CardData newData)
    {
        originalData = newData;
        LoadCardData(); 
        UpdateUI(); 
    }

    // --- NUEVA FUNCIÓN COMPATIBLE ---
    // Agregamos esto para que el SpiritDeck pueda llamarla como "Setup"
    // Sin romper nada de lo anterior.
    public void Setup(CardData newData)
    {
        SetUpCard(newData);
    }
}