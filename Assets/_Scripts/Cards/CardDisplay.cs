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
    public TMP_Text costText;

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
    private int currentCost;

    void Start()
    {
        if (originalData != null)
        {
            LoadCardData();
            UpdateUI();
        }
    }

    void LoadCardData()
    {
        currentPd = originalData.basePd;
        currentPs = originalData.basePs;
        currentCost = originalData.cost;
    }

    public void UpdateUI()
    {
        // 1. Textos básicos y Arte (Con chequeos de seguridad)
        if (nameText) nameText.text = originalData.cardName;
        if (descriptionText) descriptionText.text = originalData.description;
        if (pdText) pdText.text = currentPd.ToString();
        if (psText) psText.text = currentPs.ToString();
        if (costText) costText.text = currentCost.ToString();
        if (archetypeText) archetypeText.text = originalData.archetype;
        
        if (artImage && originalData.art) artImage.sprite = originalData.art;

        // 2. Icono del Elemento
        if (elementIconSlot != null)
        {
            // Protegemos el acceso a GameAssets
            if (GameAssets.i != null)
            {
                elementIconSlot.sprite = GameAssets.i.GetElementSprite(originalData.element);
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

        foreach (var state in statesList)
        {
            if (state.icon != null)
            {
                GameObject newIcon = Instantiate(stateIconPrefab, container);
                // Intentamos obtener la imagen, si falla no crasheamos
                Image iconImg = newIcon.GetComponent<Image>();
                if (iconImg != null) iconImg.sprite = state.icon;
            }
        }
    }

    // --- AQUÍ ESTABA EL ERROR (Línea 138) ---
    // Esta versión revisa si los hijos existen antes de intentar usarlos
    private void FillEffectIndicators(Transform container, List<EffectData> effectsList, Sprite triggerIcon)
    {
        if (container == null || effectIndicatorPrefab == null || effectsList == null) return;

        foreach (var effect in effectsList)
        {
            SealEffect sealEffect = effect as SealEffect;

            if (sealEffect != null)
            {
                GameObject indicator = Instantiate(effectIndicatorPrefab, container);

                // SEGURIDAD: Verificamos que el prefab tenga al menos 3 hijos
                if (indicator.transform.childCount < 3)
                {
                    Debug.LogError($"[CardDisplay] ERROR: El prefab 'EffectIndicator' solo tiene {indicator.transform.childCount} hijos. Necesita 3 (Trigger, Text, Seal).");
                    continue; // Saltamos al siguiente efecto para no romper el juego
                }

                // Obtenemos los componentes de forma segura
                var triggerImg = indicator.transform.GetChild(0).GetComponent<Image>();
                var amountTxt = indicator.transform.GetChild(1).GetComponent<TMP_Text>();
                var sealImg = indicator.transform.GetChild(2).GetComponent<Image>();

                // Asignamos datos SOLO si el componente existe
                if (triggerImg != null) triggerImg.sprite = triggerIcon;
                
                if (amountTxt != null) amountTxt.text = "x" + sealEffect.amount.ToString();
                
                if (sealImg != null && GameAssets.i != null) 
                    sealImg.sprite = GameAssets.i.GetElementSprite(sealEffect.sealType);
            }
        }
    }

    // --- TRIGGERS DE COMBATE ---
    public void TriggerSummon()
    {
        ExecuteEffects(originalData.onSummonEffects);
    }

    public void TriggerDeath()
    {
        ExecuteEffects(originalData.onDeathEffects);
    }

    private void ExecuteEffects(List<EffectData> effectsList)
    {
        if (effectsList == null) return;
        foreach (var effect in effectsList)
        {
            effect.Activate(this);
        }
    }
}