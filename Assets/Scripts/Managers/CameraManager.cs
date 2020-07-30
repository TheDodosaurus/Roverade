using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : MonoBehaviour
{
    public GameObject cursor;
    private bool inMenu = false;
    private bool inShop = false;

    private bool InUI { get => inMenu || inShop; }

    public float panSpeed = .2f;
    public float panLerp = 8f;
    public float reclampLerp = 5f;
    public float zoomSpeed = 1f;
    public float zoomLerp = 2f;

    public float minZoom = 2f;
    public float maxZoom = -1f;

    private float zoom = 5f;
    private float camHeight;

    private Vector3 targetPosition;
    private float targetZoom;
    private Camera mainCamera;
    private Vector3Int cursorPos;

    private Tile tile;

    private void Awake()
    {
        GameManager.cam = this;
    }

    private void Start()
    {
        transform.position = new Vector3(GameManager.sfm.surfaceCentre.x, 10, GameManager.sfm.surfaceCentre.z);
        targetPosition = transform.position;
        mainCamera = GetComponent<Camera>();
        zoom = mainCamera.orthographicSize;
        targetZoom = zoom;
        if (maxZoom < 0f)
        {
            maxZoom = GameManager.sfm.surfaceSize / 2;
        }
        camHeight = transform.position.y;
    }
    private void Update()
    {
        if (!InUI)
        {

            targetPosition.x += Input.GetAxis("Horizontal") * panSpeed;
            targetPosition.z += Input.GetAxis("Vertical") * panSpeed;
            targetZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;


            // Cursor
            Vector2 cursorPixelPos = Input.mousePosition;
            cursorPos = GameManager.sfm.ClampToSurfaceRounded(mainCamera.ScreenToWorldPoint(cursorPixelPos));
            cursorPos.y = 0;
        }

        targetPosition = Vector3.Lerp(targetPosition, GameManager.sfm.ClampToSurface(targetPosition), reclampLerp * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPosition, panLerp * Time.deltaTime);

        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        zoom = Mathf.Lerp(zoom, targetZoom, zoomLerp * Time.deltaTime);

        mainCamera.orthographicSize = zoom;

        // Cursor
        cursor.transform.position = cursorPos;

        if (Input.GetMouseButtonDown(0))
        {
            if (!InUI)
            {
                // TODO: check if cursorPos bugged
                GameObject tileObject = GameManager.sfm.surface[cursorPos.x, cursorPos.z];
                tile = tileObject.GetComponent<Tile>();
                tile.Click();
                Focus(tileObject.transform.position);
                SetMenu(true);

                //LinkedList<Vector3Int> k = GameManager.sfm.Dijkstra(new Vector3Int(0, 0, 0), cursorPos);
                //if (k == null) Destroy(GameManager.sfm.surface[cursorPos.x, cursorPos.z]); // todo: remove
                //else foreach (Vector3Int kx in k)
                //{
                //    Destroy(GameManager.sfm.surface[kx.x, kx.z]);
                //}

            }
            else if (inMenu)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    SetMenu(false);
                }
            }
        }
    }

    public void Focus(Vector3 pos)
    {
        targetZoom = minZoom;
        targetPosition = GameManager.sfm.ClampToSurface(pos);
        targetPosition.y = camHeight;
    }

    public void SetMenu(bool inMenu)
    {

        this.inMenu = inMenu;

        GameManager.gm.SetItemActionMenuVisible(inMenu);
        if (!inMenu) GameManager.gm.ClearItemMenu();
        else CloseShop();
    }

    public void OpenShop(Vector3Int pos)
    {
        SetMenu(false);
        GameManager.shm.Show(pos);
        inShop = true;
    }

    public void CloseShop()
    {
        GameManager.shm.Hide();
        inShop = false;
    }
}
