using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlaceableItem : MonoBehaviour
{
    public string itemName;
    public Sprite iconSprite; // Icon representation for the UI.

    // ... (add any other common properties or methods for towers and walls here)
}