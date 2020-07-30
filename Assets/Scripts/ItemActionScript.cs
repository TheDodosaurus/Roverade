using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct ItemAction
{

    public string name;
    public string description;
    public int cost;
    public bool roved;
    public Action callback;

    public ItemAction(string name, string description, int cost, bool roved, Action callback)
    {
        this.name = name;
        this.description = description;
        this.cost = cost;
        this.roved = roved;
        this.callback = callback;
    }
}

public class ItemActionScript : MonoBehaviour
{

    public ItemAction ia;
    public Text nameText;
    public Text descriptionText;
    public Text costText;
    public Button button;


    private void Update()
    {
        SetAvailable();
    }

    public void SetItemAction(ItemAction ia)
    {
        this.ia = ia;
        nameText.text = ia.name;
        descriptionText.text = ia.description;
        if (ia.cost != 0) costText.text = GameManager.rm.FormatResource(ResourceManager.MONEY, ia.cost);
        else costText.text = "";

        SetAvailable();
    }

    private void SetAvailable()
    {
        if (ia.roved) button.interactable = !GameManager.rover.Busy && GameManager.rm.CheckBalance(ResourceManager.MONEY, ia.cost);
    }

    public void ActionClicked()
    {
        bool success = GameManager.rm.Use(ResourceManager.MONEY, ia.cost);
        if (success)
        {
            GameManager.cam.SetMenu(false);
            ia.callback?.Invoke();
        }
    }
}
