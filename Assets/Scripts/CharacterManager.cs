using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public List<GameObject> characters = new List<GameObject>();
    private IEnumerator bouncing;

    public void SetExpression(int ch, int ex) { characters[ch].GetComponent<CharacterExpressions>().SetSprite(ex); }

    /*
    public void BounceCharacter(int ch) // Not implemented - didn't feel like adding another animation for the character bounces
    {
        if (bouncing != null)
        {
            StopCoroutine(bouncing);
        }
        bouncing = BounceAnimation();
        StartCoroutine(bouncing);
    }

    IEnumerator BounceAnimation()
    {
        Debug.Log("bouncy");
        yield return new WaitForSeconds(1f);
    }
    */
}
