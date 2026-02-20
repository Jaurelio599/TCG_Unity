using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using UnityEngine.InputSystem; // <--- 1. NUEVA LIBRERÍA OBLIGATORIA

public enum GamePhase
{
    StartPhase,
    MainPhase,
    EndPhase,
    EnemyTurn
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    [Header("--- ESTADO DE LA PARTIDA ---")]
    public GamePhase currentPhase;
    public int turnCount = 0;
    public bool isPlayerTurn; 

    [Header("--- JUGADOR (Aurelio) ---")]
    public int playerHP = 1700;
    public int maxHP = 1700;
    
    [Header("Recursos")]
    public int playerRitualMana; 
    public int playerSoulMana;
    public int manaCap = 10;     
    public Dictionary<CostType, int> playerSeals = new Dictionary<CostType, int>();

    [Header("--- OPONENTE ---")]
    public int opponentHP = 1700;

    [Header("Zona de Campo (Field Zone)")]
    public Transform playerFieldZone;      // Arrastra aquí tu Field_Zone de la escena
    public GameObject spiritFieldPrefab;   // Arrastra aquí tu prefab del Espíritu para el campo

    public event Action OnGameStateChanged;

    void Start()
    {
        InitializeSeals();
        StartGame();
    }

    void InitializeSeals()
    {
        playerSeals.Clear();
        playerSeals.Add(CostType.FireSeal, 0);
        playerSeals.Add(CostType.WaterSeal, 0);
        playerSeals.Add(CostType.EarthSeal, 0);
        playerSeals.Add(CostType.WindSeal, 0);
        playerSeals.Add(CostType.VoidSeal, 0);
        playerSeals.Add(CostType.LightSeal, 0);
        playerSeals.Add(CostType.DarkSeal, 0); 
        playerSeals.Add(CostType.GenericSeal, 0); 
    }

    public void StartGame()
    {
        playerHP = 1700;
        opponentHP = 1700;
        turnCount = 0;
        isPlayerTurn = true;
        StartTurnSequence();
    }

    public void StartTurnSequence()
    {
        if (isPlayerTurn)
        {
            currentPhase = GamePhase.StartPhase;
            turnCount++; 
            Debug.Log($"--- TURNO {turnCount} (JUGADOR) ---");

            int manaToGive = Mathf.Min(turnCount, manaCap);
            playerRitualMana = manaToGive;
            playerSoulMana = manaToGive;

            if (DeckSystem.instance != null)
            {
                if (turnCount == 1)
                {
                    Debug.Log("Primer turno: Robando 4 cartas...");
                    DeckSystem.instance.DrawMultipleCards(4);
                }
                else
                {
                    Debug.Log("Inicio de turno: Robando 1 carta...");
                    DeckSystem.instance.DrawCard();
                }
            }

            UpdateUI();
            StartCoroutine(TransitionToMainPhase());
        }
        else
        {
            StartCoroutine(EnemyTurnRoutine());
        }
    }

    IEnumerator TransitionToMainPhase()
    {
        yield return new WaitForSeconds(1.0f); 
        currentPhase = GamePhase.MainPhase;
        Debug.Log("Fase Principal: Juega cartas o ataca.");
        UpdateUI();
    }

    public void EndTurnButton()
    {
        if (isPlayerTurn && currentPhase == GamePhase.MainPhase)
        {
            currentPhase = GamePhase.EndPhase;
            Debug.Log("Fin del Turno.");
            isPlayerTurn = false;
            StartTurnSequence();
        }
    }

    IEnumerator EnemyTurnRoutine()
    {
        currentPhase = GamePhase.EnemyTurn;
        Debug.Log("--- TURNO OPONENTE ---");
        UpdateUI();
        yield return new WaitForSeconds(2.0f);
        isPlayerTurn = true;
        StartTurnSequence();
    }

    // --- 2. EL CAMBIO PROFESIONAL (NEW INPUT SYSTEM) ---
    void Update()
    {
        // Primero verificamos que exista un teclado conectado (Buena práctica)
        if (Keyboard.current == null) return;

        // Sintaxis Nueva: Keyboard.current.[TECLA].wasPressedThisFrame
        if (Keyboard.current.digit1Key.wasPressedThisFrame) 
        {
            AddSeal(CostType.FireSeal, 1);
        }
        
        if (Keyboard.current.digit2Key.wasPressedThisFrame) 
        {
            AddSeal(CostType.WaterSeal, 1);
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame) 
        {
            AddSeal(CostType.EarthSeal, 1);
        }

        if (Keyboard.current.digit4Key.wasPressedThisFrame) 
        {
            AddSeal(CostType.WindSeal, 1);
        }
        
        // Tecla G
        if (Keyboard.current.gKey.wasPressedThisFrame) 
        {
             AddSeal(CostType.FireSeal, 1);
             AddSeal(CostType.WaterSeal, 1);
        }

        // Tecla H: Te hace 100 de daño a TI
        if (Keyboard.current.hKey.wasPressedThisFrame) 
        {
            Debug.Log("¡Recibiste 100 de daño!");
            DamagePlayer(100);
        }

        // Tecla J: Le hace 100 de daño al OPONENTE
     /*   if (Keyboard.current.jKey.wasPressedThisFrame) 
        {
            Debug.Log("¡Atacaste al oponente con 100 de daño!");
            DamageOpponent(100);
        }*/
    }

    // --- SISTEMA DE COSTOS ---

    public bool CanPayCardCost(List<CardCost> costs)
    {
        if (costs == null || costs.Count == 0) return true;

        foreach (CardCost cost in costs)
        {
            switch (cost.type)
            {
                case CostType.RitualMana:
                    if (playerRitualMana < cost.amount) return false;
                    break;
                case CostType.SoulMana:
                    if (playerSoulMana < cost.amount) return false;
                    break;
                case CostType.Health:
                    if (playerHP < cost.amount) return false;
                    break;
                
                case CostType.GenericSeal:
                    int totalSealsOwned = 0;
                    foreach (var sealCount in playerSeals.Values) totalSealsOwned += sealCount;
                    if (totalSealsOwned < cost.amount) return false;
                    break;

                default: 
                    if (playerSeals.ContainsKey(cost.type))
                    {
                        if (playerSeals[cost.type] < cost.amount) return false;
                    }
                    else return false; 
                    break;
            }
        }
        return true; 
    }

    public void PayCardCost(List<CardCost> costs)
    {
        if (costs == null || costs.Count == 0) return;

        foreach (CardCost cost in costs)
        {
            switch (cost.type)
            {
                case CostType.RitualMana:
                    playerRitualMana -= cost.amount;
                    break;
                case CostType.SoulMana:
                    playerSoulMana -= cost.amount;
                    break;
                case CostType.Health:
                    DamagePlayer(cost.amount);
                    break;

                case CostType.GenericSeal:
                    int remainingToPay = cost.amount;
                    List<CostType> sealTypes = new List<CostType>(playerSeals.Keys);
                    foreach (CostType type in sealTypes)
                    {
                        if (remainingToPay <= 0) break;
                        int available = playerSeals[type];
                        if (available > 0)
                        {
                            int amountToTake = Mathf.Min(available, remainingToPay);
                            playerSeals[type] -= amountToTake;
                            remainingToPay -= amountToTake;
                        }
                    }
                    break;

                default:
                    if (playerSeals.ContainsKey(cost.type))
                    {
                        playerSeals[cost.type] -= cost.amount;
                    }
                    break;
            }
        }
        UpdateUI();
    }

    public void AddSeal(CostType sealType, int amount)
    {
        if (playerSeals.ContainsKey(sealType))
        {
            playerSeals[sealType] += amount;
            UpdateUI();
        }
    }

    public void DamagePlayer(int damage)
    {
        playerHP -= damage;
        if (playerHP < 0) playerHP = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        OnGameStateChanged?.Invoke();
    }

  public void SummonSpiritToField(CardData spiritData)
    {
        if (playerFieldZone != null && spiritFieldPrefab != null)
        {
            // 1. Buscar un slot que esté vacío
            Transform emptySlot = null;
            
            // Esto revisa a los 6 hijos (cardSlot1, cardSlot2, etc.) dentro de Field_zone
            foreach (Transform slot in playerFieldZone)
            {
                // Si el slot no tiene ningún hijo adentro, significa que está libre
                if (slot.childCount == 0) 
                {
                    emptySlot = slot;
                    break; // Ya encontramos uno vacío, dejamos de buscar
                }
            }

            // 2. Si encontramos un lugar disponible...
            if (emptySlot != null)
            {
                // Instanciamos el Prefab ADENTRO del slot vacío, no en la zona general
                GameObject newSpiritObj = Instantiate(spiritFieldPrefab, emptySlot);
                
                // Asegurarnos de que quede centrado exactamente en el cuadro azul
                RectTransform rect = newSpiritObj.GetComponent<RectTransform>();
                if (rect != null) rect.localPosition = Vector3.zero;
                
                // --- AQUÍ ESTÁ EL CAMBIO ---
                // Conectar los gráficos y disparar los efectos de invocación
                CardDisplay display = newSpiritObj.GetComponent<CardDisplay>();
                if (display != null) 
                {
                    display.Setup(spiritData);
                    display.TriggerSummon(); // <--- ¡AQUÍ ESTÁ LA LÍNEA NUEVA!
                }
                // ---------------------------

                // Quitarlo del SpiritDeck (cementerio)
                SpiritDeck.instance.RemoveSpirit(spiritData);

                Debug.Log($"¡{spiritData.cardName} ha sido invocado en {emptySlot.name}!");
                UpdateUI();
            }
            else
            {
                // Si revisó los 6 slots y ninguno estaba vacío
                Debug.LogWarning("¡Tu campo está lleno! No caben más espíritus.");
                // Ojo: Si llega a pasar esto, ya te cobró el maná. 
                // Luego podemos agregar un chequeo antes de pagar, pero para probar está perfecto.
            }
        }
        else
        {
            Debug.LogError("¡Falta asignar el Field Zone o el Spirit Prefab en el GameManager!");
        }
    }
}