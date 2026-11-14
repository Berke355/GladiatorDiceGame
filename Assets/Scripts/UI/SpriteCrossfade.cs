using System.Collections;
using UnityEngine;

// Swaps a SpriteRenderer's picture with a short crossfade (the Hydra changing as heads are cut and grow back): the new
// picture shows at once, and a copy of the old one lies over it and fades out.
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteCrossfade : MonoBehaviour
{
    public float duration = 0.45f;

    private SpriteRenderer main;
    private SpriteRenderer overlay;
    private Coroutine fading;

    void Awake(){
        main = GetComponent<SpriteRenderer>();
    }

    public void Swap(Sprite next){
        if(main == null) main = GetComponent<SpriteRenderer>();
        if(main.sprite == next) return;
        Sprite old = main.sprite;
        main.sprite = next;
        if(old == null || !isActiveAndEnabled) return;
        if(overlay == null){
            GameObject child = new GameObject("CrossfadeOverlay");
            child.transform.SetParent(transform, false);
            overlay = child.AddComponent<SpriteRenderer>();
        }
        overlay.sortingLayerID = main.sortingLayerID;
        overlay.sortingOrder = main.sortingOrder + 1;
        overlay.flipX = main.flipX;
        overlay.sprite = old;
        if(fading != null) StopCoroutine(fading);
        fading = StartCoroutine(Fade());
    }

    private IEnumerator Fade(){
        overlay.enabled = true;
        for(float t = 0f; t < duration; t += Time.deltaTime){
            Color c = main.color; c.a *= 1f - t / duration;
            overlay.color = c;
            yield return null;
        }
        overlay.enabled = false;
        fading = null;
    }
}
