using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public static SurfaceManager sfm;
    public static ShopManager shm;
    public static CameraManager cam;
    public static ResourceManager rm;
    public static Rover rover;

    public GameObject itemActionButtonPrefab;
    public GameObject ItemActionPanel;
    public GameObject roverObject;

    private LinkedList<GameObject> itemActionButtons;
   

    private void Start()
    {
        if (gm != null) return;
        itemActionButtons = new LinkedList<GameObject>();
        gm = this;

        rover = roverObject.GetComponent<Rover>();
        sfm.Initialise();
        InitRover();
    }

    public void ClearItemMenu()
    {
        while (itemActionButtons.Count > 0)
        {

            GameObject iab = itemActionButtons.First.Value;
            Destroy(iab);
            itemActionButtons.RemoveFirst();
        }
    }

    public void AddItemMenu(ItemAction[] itemActions)
    {

        foreach (ItemAction ia in itemActions)
        {
            GameObject iab = Instantiate(itemActionButtonPrefab, ItemActionPanel.transform);
            iab.GetComponent<ItemActionScript>().SetItemAction(ia);

            itemActionButtons.AddLast(iab);
        }
    }

    public void SetItemActionMenuVisible(bool isActive)
    {
        ItemActionPanel.SetActive(isActive);
    }

    public bool CallRover(Vector3Int pos, float timer, Action callback)
    {
        if (rover.Busy)
        {
            Debug.LogErrorFormat("Rover too busy to reach {0},{1}.", pos.x, pos.y);
        }

        rover.SetTarget(pos, timer, callback);
        return true;
    }

    public void InitRover()
    {
        Vector3Int pos = rover.GetComponent<Rover>().Pos;
        sfm.tiles[pos.x, pos.z].EmptyTile();
    }
}
