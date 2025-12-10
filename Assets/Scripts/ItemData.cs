using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Data")]
public class ItemData : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public bool stackable = true;
    public int maxStack = 99;

    public GameObject heldPrefab;            // what appears in the player's hands
    public Vector3 heldLocalPosition = new Vector3(0.3f, -0.35f, 0.6f);
    public Vector3 heldLocalRotation = Vector3.zero;
    public Vector3 heldLocalScale = Vector3.one;

}
