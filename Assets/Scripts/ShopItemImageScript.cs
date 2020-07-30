using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemImageScript : MonoBehaviour
{
    public Image itemImage;
    private int index;


    public void Initialise(int index)
    {
        this.index = index;
    }

    public void OnImageClick()
    {
        GameManager.shm.OnImageClick(index);

    }
}
