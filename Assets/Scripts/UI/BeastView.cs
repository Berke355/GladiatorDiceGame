using System.Collections;
using UnityEngine;

// The Beastmaster's beast beside the player (GDD "Beastmaster"): hidden while the owner has no Hound, fades in when
// the first Hound face summons it, and lunges toward the enemy on every bite. Only the picture: the bite itself is
// Beast.Bite. Its resting place is where it stands in the scene.
public class BeastView : MonoBehaviour
{
    public Entity owner;
    public SpriteRenderer spriteRenderer;
    public float lungeDistance = 0.6f;
    public float lungeTime = 0.18f;
    public float fadeTime = 0.35f;

    private Vector3 rest;
    private bool shown;
    private Coroutine fade, lunge;

    void Awake(){
        rest = transform.position;
        SetAlpha(0f);
    }

    void OnEnable(){
        if(owner == null) return;
        owner.OnHealthChanged += Refresh;
        owner.BeastBit += OnBite;
        Refresh();
    }

    void OnDisable(){
        if(owner == null) return;
        owner.OnHealthChanged -= Refresh;
        owner.BeastBit -= OnBite;
    }

    private void Refresh(){
        bool show = owner.GetStatus(StatusType.Hound) > 0 && !owner.IsDead;
        if(show == shown) return;
        shown = show;
        if(fade != null) StopCoroutine(fade);
        fade = StartCoroutine(Fade(show ? 1f : 0f));
    }

    private void OnBite(Entity bitten){
        if(lunge != null) StopCoroutine(lunge);
        lunge = StartCoroutine(Lunge());
    }

    private IEnumerator Fade(float to){
        float from = spriteRenderer.color.a;
        for(float t = 0f; t < fadeTime; t += Time.deltaTime){
            SetAlpha(Mathf.Lerp(from, to, t / fadeTime));
            yield return null;
        }
        SetAlpha(to);
    }

    private IEnumerator Lunge(){
        Vector3 forward = rest + Vector3.right * lungeDistance;
        for(float t = 0f; t < lungeTime; t += Time.deltaTime){
            transform.position = Vector3.Lerp(rest, forward, t / lungeTime);
            yield return null;
        }
        for(float t = 0f; t < lungeTime; t += Time.deltaTime){
            transform.position = Vector3.Lerp(forward, rest, t / lungeTime);
            yield return null;
        }
        transform.position = rest;
    }

    private void SetAlpha(float alpha){
        if(spriteRenderer == null) return;
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}
