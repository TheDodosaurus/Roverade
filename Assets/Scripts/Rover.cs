using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class Rover : MonoBehaviour
{

    public float speed = 1f;
    public Image progress;

    private bool moving;
    public bool Busy { get; private set; }
    private Vector3Int target;
    private Vector3Int pos;
    public Vector3Int Pos { get => pos; }
    private Vector3 lastPos;
    private LinkedList<Vector3Int> path;
    private Action callback;
    private float tileDistance = 0f;
    private float workTimer = 0f;
    private float targetTime = 0f;

    private float distancePerStep = 0f;
    private float pathIndex = 0f;


    private void Start()
    {
        lastPos = pos = GameManager.sfm.ClampToSurfaceRounded(transform.position);

    }

    public bool SetTarget(Vector3Int target, float targetTime, Action callback)
    {
        this.target = target;
        this.callback = callback;
        this.targetTime = targetTime;

        return SetTarget(target);
    }

    private bool SetTarget(Vector3Int target)
    {
        path = GameManager.sfm.Dijkstra(pos, target);
        if (path == null)
        {
            Debug.LogErrorFormat("Rover can't reach tile at {0},{1}", target.x, target.z);
            return false;
        };
        Busy = true;
        moving = true;
        tileDistance = 0f;
        distancePerStep = 1f / path.Count;
        pathIndex = -1f;


        Move();
        return true;
    }

    public void ClearTarget()
    {
        Busy = false;
        moving = false;
        Move();
    }

    private void Update()
    {
        if (Busy)
        {
            if (moving)
            {
                tileDistance += Time.deltaTime * speed;
                if (tileDistance >= 1)
                {
                    tileDistance--;
                    Move();
                }

                progress.fillAmount = (pathIndex + tileDistance) * distancePerStep;
            }
            else
            {
                workTimer += Time.deltaTime;
                if (workTimer >= targetTime)
                {
                    Busy = false;
                    progress.fillAmount = 0f;
                    callback?.Invoke();
                    return;
                }

                progress.fillAmount = workTimer / targetTime;
            }
        }

        transform.position = Vector3.Lerp(lastPos, pos, tileDistance);
    }

    public void Move()
    {
        if (Busy)
        {
            if (path.Count == 0)
            {
                moving = false;
                lastPos = pos;
                workTimer = 0f;
                return;
            }

            // Check whether path still holds
            if (!GameManager.sfm.IsPathValid(path))
                SetTarget(target);
            // TODO: ^^ handle a false ^^

            lastPos = pos;
            pos = path.First.Value;
            path.RemoveFirst();

            pathIndex++;
        }
        else
        {
            lastPos = pos;
        }
    }

}
