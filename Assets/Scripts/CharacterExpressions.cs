using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterExpressions : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    public int defaultSprite;
    private Image spriteSlot;

    public void Awake()
    {
        spriteSlot = this.gameObject.GetComponent<Image>();
        spriteSlot.sprite = sprites[defaultSprite];
    }
    public void SetSprite(int n) {
        if (n >= sprites.Count || n < 0)
        {
            Debug.Log("Invalid expression! Expression was not changed.");
            return; // Error check
        }
        spriteSlot.sprite = sprites[n];
    }
}
