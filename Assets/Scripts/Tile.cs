using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;


public class Tile : MonoBehaviour
{
    private GameObject item;
    private ItemScript itemScript;

    private ItemAction[] emptyTileOptions;

    private Vector3Int pos;

    private ShopItem shopItem;

    private bool busy = false;

    public void Initialise(Vector3Int pos)
    {
        this.pos = pos;
        emptyTileOptions = new ItemAction[] { new ItemAction("Build", "", 0, false, OpenShop),
            new ItemAction("Call Rover", "Call the rover to this tile.", 0, true, CallRoverHere) };
    }

    public void OverwriteItem(GameObject itemPrefab)
    {
        EmptyTile();

        item = Instantiate(itemPrefab, transform.position, transform.rotation, transform);
        itemScript = item.GetComponent<ItemScript>();
        itemScript.Initialise(pos);
    }

    public void EmptyTile()
    {

        if (item != null)
        {
            Destroy(item);
            itemScript = null;
        }
    }

    public bool CreateItem(GameObject itemPrefab)
    {

        if (item != null) return false;

        OverwriteItem(itemPrefab);
        return true;
    }

    public GameObject GetItem()
    {
        return item;
    }

    public void Click()
    {
        if (!busy)
        {
            //GameManager.instance.ClearItemMenu();
            if (item != null)
            {
                GameManager.gm.AddItemMenu(itemScript.getItemActions());
            }
            else
            {
                GameManager.gm.AddItemMenu(emptyTileOptions);
            }
        }
    }

    public bool Rovable()
    {
        if (item != null)
            return itemScript.Rovable;
        return true;
    }

    public void OpenShop()
    {
        GameManager.cam.OpenShop(pos);
    }

    public void PreBuild(ShopItem shopItem)
    {
        this.shopItem = shopItem;
        GameManager.gm.CallRover(pos, shopItem.buildTime, RBuild);
        busy = true;
    }

    public void CallRoverHere()
    {
        GameManager.gm.CallRover(pos, 0f, null);
    }

    public void RBuild()
    {
        CreateItem(shopItem.itemPrefab);
        busy = false;
    }
}
