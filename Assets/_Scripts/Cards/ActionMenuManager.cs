using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem; // <-- Vital para usar el nuevo sistema de clicks sin errores

public class MenuOption
{
    public string buttonText;
    public bool isInteractable;
    public Action onClickAction; 
}

public class ActionMenuManager : MonoBehaviour
{
    public static ActionMenuManager instance;

    [Header("Referencias UI")]
    public GameObject menuContainer; 
    public GameObject buttonPrefab;  
    public Transform buttonsParent;  

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        if (menuContainer != null) menuContainer.SetActive(false);
    }

    public void ShowMenu(List<MenuOption> options)
    {
        if (menuContainer == null || buttonPrefab == null) return;

        // Forzamos al panel a ir al centro exacto de la pantalla
        RectTransform rect = menuContainer.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero; // (0,0) es el centro
        }

        // Limpiar botones viejos
        foreach (Transform child in buttonsParent)
        {
            Destroy(child.gameObject);
        }

        // Crear los nuevos botones
        foreach (var option in options)
        {
            GameObject newBtnObj = Instantiate(buttonPrefab, buttonsParent);
            Button btn = newBtnObj.GetComponent<Button>();
            TMP_Text txt = newBtnObj.GetComponentInChildren<TMP_Text>();

            if (txt != null) txt.text = option.buttonText;
            
            if (btn != null) 
            {
                btn.interactable = option.isInteractable;
                
                btn.onClick.AddListener(() =>
                {
                    option.onClickAction?.Invoke();
                    CloseMenu();
                });
            }
        }

        menuContainer.SetActive(true);
    }

    public void CloseMenu()
    {
        if (menuContainer != null) menuContainer.SetActive(false);
    }

    void Update()
    {
        // Usamos Mouse.current del Nuevo Sistema de Inputs en lugar de Input.GetMouseButtonDown
        if (menuContainer.activeSelf && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Invoke("CloseMenu", 0.1f); 
        }
    }
}