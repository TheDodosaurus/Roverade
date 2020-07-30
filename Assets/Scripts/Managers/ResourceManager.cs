using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Resource
{
    public string name;
    public string format;
    public Text text;
    public int amount;
    public bool negateable;
}

public class ResourceManager : MonoBehaviour
{
    public static string[] NUMERAL_AFFIXES = { "", "k", "M", "B", "T", "Qd", "Qn", "Sx", "Sp", "O", "N" };

    public static int MONEY = 0;

    public GameObject resourcePanel;
    public Resource[] resources;

    private void Awake()
    {
        GameManager.rm = this;
    }

    private void Start()
    {

    }

    private void Update()
    {
        for (int i = 0; i < resources.Length; i++)
        {
            UpdateResource(resources[i]);
        }
    }

    /*public string FormatNumber(int n)
    {
        int length = Mathf.FloorToInt(Mathf.Log10(Mathf.Abs(n)) / 3);
        float mantissa = n / (1000 ^ length);
        if (length >= NUMERAL_AFFIXES.Length) return n.ToString("E2");
        return mantissa.ToString("G3") + " " + NUMERAL_AFFIXES[length];
    }*/

    private void UpdateResource(Resource r)
    {
        r.text.text = FormatResource(r);
    }
    private string FormatResource(Resource r)
    {
        return FormatResource(r, r.amount);
    }
    private string FormatResource(Resource r, int amount)
    {

        return string.Format(r.format, amount);
    }
    public string FormatResource(int index)
    {
        Resource r = resources[index];
        return FormatResource(r, r.amount);
    }
    public string FormatResource(int index, int amount)
    {
        Resource r = resources[index];
        return FormatResource(r, amount);
    }

    public bool CheckBalance(int index, int amount)
    {
        Resource r = resources[index];
        return CheckBalance(r, amount);
    }
    private bool CheckBalance(Resource r, int amount)
    {
        return (r.negateable || amount <= r.amount);
    }

    public bool Use(int index, int amount)
    {
        Resource r = resources[index];
        if (CheckBalance(r, amount))
        {
            resources[index].amount -= amount;
            Debug.Log(resources[index].amount);
            return true;
        }
        else
        {
            Debug.LogErrorFormat("Not enough {0} to pay {1} (current balance {2}).", r.name, FormatResource(r, amount), FormatResource(r));
            return false;
        }
    }
}
