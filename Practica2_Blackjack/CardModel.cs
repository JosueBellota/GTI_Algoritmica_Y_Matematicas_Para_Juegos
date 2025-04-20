using UnityEngine;

public class CardModel : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public Sprite cardBack;
    public Sprite front;
    public int value;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ToggleFace(bool showFace)
    {
        spriteRenderer.sprite = showFace ? front : cardBack;
    }
}