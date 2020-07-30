using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemScript : MonoBehaviour
{
    protected ItemAction[] itemActions;
    protected bool rovable;
    public bool Rovable { get; }
    protected Vector3Int pos;

    protected void ItemInitialise(Vector3Int pos, ItemAction[] itemActions, bool rovable)
    {
        this.pos = pos;
        this.itemActions = itemActions;
        this.rovable = rovable;
    }

    public abstract void Initialise(Vector3Int pos);

    public ItemAction[] getItemActions() {
        return itemActions;
    }

}
