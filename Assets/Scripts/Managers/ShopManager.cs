using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ShopItem
{
    public string name;
    public string description;
    public string costString;
    public float buildTime;
    public Sprite itemSprite;
    public GameObject itemPrefab;
}


public class ShopManager : MonoBehaviour
{
    [Header("Shop GameObjects")]
    public GameObject shopPanel;
    public GameObject categoryPanel;
    public GameObject selectionContent;
    public Text nameInfo;
    public Text descriptionInfo;
    public Text costInfo;
    public GameObject buildObject;
    [Header("Prefabs")]
    public GameObject shopItemImage;

    [Header("Shop Items")]
    public ShopItem[] shopItems;

    //

    private Vector3Int pos;
    private int index = -1;
    private Button build;

    private void Awake()
    {
        GameManager.shm = this;
    }

    private void Start()
    {
        build = buildObject.GetComponent<Button>();
        // todo
        //GameManager.cam.OpenShop(new Vector3Int(0,0,0));
    }

    public void Show(Vector3Int pos)
    {
        HideAllShopItems();
        nameInfo.text = "";
        descriptionInfo.text = "";
        costInfo.text = "";
        index = -1;
        buildObject.SetActive(false);
        for (int i = 0; i < shopItems.Length; i++)
        {
            ShopItem shopItem = shopItems[i];
            GameObject imageObject = Instantiate(shopItemImage, selectionContent.transform);
            ShopItemImageScript shopItemImageScript = imageObject.GetComponent<ShopItemImageScript>();
            shopItemImageScript.Initialise(i);
            shopItemImageScript.itemImage.sprite = shopItem.itemSprite;
        }
        shopPanel.SetActive(true);
        this.pos = pos;
    }
    public void HideAllShopItems()
    {
        foreach (Transform child in selectionContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Hide()
    {
        HideAllShopItems();
        shopPanel.SetActive(false);
    }

    public void OnImageClick(int index)
    {
        nameInfo.text = shopItems[index].name;
        descriptionInfo.text = shopItems[index].description;
        costInfo.text = shopItems[index].costString;
        buildObject.SetActive(true);
        this.index = index;
    }

    public void MBuild()
    {
        if (index == -1) return;
        GameManager.sfm.tiles[pos.x, pos.z].PreBuild(shopItems[index]);
        GameManager.cam.CloseShop();
    }
}
