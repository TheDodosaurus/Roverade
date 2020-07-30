using System.Collections.Generic;
using UnityEngine;

public class SurfaceManager : MonoBehaviour
{
    // For multiple surfaces, create a static SurfaceManager[] `surfaces` and an int `currentSurface`.
    // All static methods within SurfaceManager use `surfaces[currentSurface]`.
    public static Quaternion quaternionUp = Quaternion.Euler(90, 0, 0);
    public float surfaceSize;

    public float rubbleAttempsPerTile = 0.05f;

    public int size;
    public GameObject background;
    public GameObject tilePrefab;
    public GameObject rubblePrefab;

    public GameObject[,] surface;
    public Tile[,] tiles;
    public Vector3 surfaceCentre;

    private void Awake()
    {
        surfaceCentre = new Vector3(size / 2f, 0, size / 2f);
        surfaceSize = size;
        GameManager.sfm = this;
    }
    public void Initialise()
    {

        surface = new GameObject[size, size];
        tiles = new Tile[size, size];
        background.transform.position = new Vector3(surfaceCentre.x, -5, surfaceCentre.z);

        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                GameObject tileObject = Instantiate(tilePrefab, new Vector3(x * 1, 0, z * 1), quaternionUp, transform);
                tileObject.name = string.Format("Tile {0},{1}", x, z);
                surface[x, z] = tileObject;
                // Initialising tile
                Tile tile = tileObject.GetComponent<Tile>();
                tiles[x, z] = tile;
                tile.Initialise(new Vector3Int(x, 0, z));
            }
        }

        int rubbleAttempts = Mathf.FloorToInt(rubbleAttempsPerTile * size * size);
        for (int i = 0; i < rubbleAttempts; ++i)
        {
            int x = Random.Range(0, size);
            int z = Random.Range(0, size);
            Tile tile = tiles[x, z];
            if (tile.GetItem() == null)
            {
                tile.CreateItem(rubblePrefab);
            }
        }

    }

    private void Update()
    {
        if (surface == null) return;
    }

    public Vector3 ClampToSurface(Vector3 pos)
    {
        return new Vector3(Mathf.Clamp(pos.x, 0f, surfaceSize - 1), pos.y, Mathf.Clamp(pos.z, 0f, surfaceSize - 1));
    }

    public Vector3Int ClampToSurfaceRounded(Vector3 pos)
    {
        return new Vector3Int(Mathf.RoundToInt(Mathf.Clamp(pos.x, 0f, surfaceSize - 1)), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(Mathf.Clamp(pos.z, 0f, surfaceSize - 1)));
    }

    public LinkedList<Vector3Int> Dijkstra(Vector3Int startV, Vector3Int stopV)
    { // A*
        if (startV.x == stopV.x && startV.z == stopV.z) return new LinkedList<Vector3Int>();
        int size2 = size * size;
        int start = Vector3IntToInt(startV);
        int stop = Vector3IntToInt(stopV);
        HeapTree open = new HeapTree(size2);
        Stack<int> neighbours;

        float[] scores = new float[size2]; // todo: int?
        float[] fScores = new float[size2];
        int[] previous = new int[size2];
        bool[] rovable = new bool[size2];

        open.Insert(start, fScores);
        for (int i = 0; i < size2; i++)
        {
            scores[i] = Mathf.Infinity;
            Tile tile = tiles[i % size, i / size];
            if (tile == null) rovable[i] = false;
            else rovable[i] = tile.Rovable();
        }
        rovable[stop] = true;
        rovable[start] = true;
        scores[start] = 0;
        fScores[start] = Mathf.Sqrt(Mathf.Pow(startV.x - stopV.x, 2) + Mathf.Pow(startV.z - stopV.z, 2));

        while (open.size > 0)
        {
            int current = open.Peek();
            if (current == stop)
            {
                LinkedList<Vector3Int> dijkstra = new LinkedList<Vector3Int>();
                int last = stop;
                while (last != start)
                {
                    dijkstra.AddFirst(IntToVector3Int(last));
                    last = previous[last];
                }
                return dijkstra;

            }

            open.Extract(fScores);
            neighbours = new Stack<int>();
            if (current % size > 0) neighbours.Push(current - 1);
            if (current % size < size - 1) neighbours.Push(current + 1);
            if (current / size > 0) neighbours.Push(current - size);
            if (current / size < size - 1) neighbours.Push(current + size);

            while (neighbours.Count > 0)
            {
                int neighbour = neighbours.Pop();
                if (rovable[neighbour])
                {
                    // Check if blocked
                    float tentative = scores[current] + Mathf.Sqrt(Mathf.Pow((current % size) - stopV.x, 2) + Mathf.Pow((current / size) - stopV.z, 2));
                    if (tentative < scores[neighbour])
                    {
                        previous[neighbour] = current;
                        scores[neighbour] = tentative;
                        fScores[neighbour] = scores[neighbour] + Mathf.Sqrt(Mathf.Pow((neighbour % size) - stopV.x, 2) + Mathf.Pow((neighbour / size) - stopV.z, 2));
                        if (!open.Contains(neighbour))
                        {
                            open.Insert(neighbour, fScores);
                        }
                    }
                }
            }
        }


        return null;

    }
    // And a function to check if the current path is still valid.
    public bool IsPathValid(LinkedList<Vector3Int> path)
    {
        if (path == null) return false;
        if (path.Count == 0) return true;
        LinkedListNode<Vector3Int> current = path.Last.Previous;
        while (current != null)
        {
            Vector3Int pos = current.Value;
            if (!tiles[pos.x, pos.z].Rovable())
                return false;
            current = current.Previous;
        }
        return true;
    }

    public int Vector3IntToInt(Vector3Int pos)
    {
        return pos.x + size * pos.z;
    }

    public Vector3Int IntToVector3Int(int pos)
    {
        return new Vector3Int(pos % size, 0, pos / size);
    }
}
