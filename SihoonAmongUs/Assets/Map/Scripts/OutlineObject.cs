using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineObject : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    private Color outlineColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    void Start()
    {
        var inst = Instantiate(spriteRenderer.material);
        spriteRenderer.material = inst;
        spriteRenderer.material.SetColor("_OutlineColor", outlineColor);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        var character = collision.GetComponent<CharacterMover>();
        if(character != null && character.isOwned)
        {
            spriteRenderer.enabled = true;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        var character = collision.GetComponent<CharacterMover>();
        if (character != null && character.isOwned)
        {
            spriteRenderer.enabled = false;
        }
    }

}
