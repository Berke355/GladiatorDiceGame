using UnityEngine;

// Small reactions on a combatant's sprite, for the picture only (the rules never wait for them): a red flash and a
// shake when a hit costs it HP, a lunge toward its opponent when it deals a hit, and a fade when it dies.
// BattleManager arranges the enemies' sprites and tells each its resting place (SetRest); the reaction only moves the
// sprite while it plays.
[RequireComponent(typeof(SpriteRenderer))]
public class HitReaction : MonoBehaviour
{
    // World units toward the opponent at the height of a lunge: negative for enemies (the player is on the left), 0 for
    // a combatant with its own attack animation.
    public float lungeDistance = -0.5f;
    public float shakeDistance = 0.12f;
    public Color flashColor = new Color(1f, 0.35f, 0.35f, 1f);

    private const float LungeDuration = 0.3f;
    private const float ShakeDuration = 0.25f;
    private const float FlashDuration = 0.25f;
    private const float FadeDuration = 0.45f;

    private Entity entity;
    private SpriteRenderer sprite;
    private Vector3 rest;
    private Color restColor = Color.white;
    private float lungeTime, shakeTime, flashTime, fadeTime;
    private bool fading;

    private bool Playing => lungeTime > 0f || shakeTime > 0f || flashTime > 0f || fading;

    void Awake(){
        entity = GetComponent<Entity>();
        sprite = GetComponent<SpriteRenderer>();
        rest = transform.position;
    }

    void OnEnable(){
        if(entity == null) return;
        entity.HitTaken += OnHitTaken;
        entity.HitDealt += OnHitDealt;
    }

    void OnDisable(){
        if(entity == null) return;
        entity.HitTaken -= OnHitTaken;
        entity.HitDealt -= OnHitDealt;
    }

    // Where the sprite stands when nothing plays (set by BattleManager when it arranges the row).
    public void SetRest(Vector3 position){
        rest = position;
        if(!Playing) transform.position = position;
    }

    // Shows or hides the sprite; hiding a visible one fades it out (a death) instead of making it vanish.
    public void SetVisible(bool visible){
        if(visible){
            fading = false;
            sprite.enabled = true;
            Color c = sprite.color; c.a = 1f; sprite.color = c;
        }
        else if(sprite.enabled && !fading){
            fading = true;
            fadeTime = FadeDuration;
        }
    }

    private void OnHitTaken(int damage){
        if(!Playing){
            rest = transform.position;
            restColor = sprite.color;
        }
        shakeTime = ShakeDuration;
        flashTime = FlashDuration;
    }

    private void OnHitDealt(Entity target, int damage){
        if(lungeDistance == 0f || lungeTime > 0f) return;
        if(!Playing){
            rest = transform.position;
            restColor = sprite.color;
        }
        lungeTime = LungeDuration;
    }

    void LateUpdate(){
        if(!Playing) return;
        float dt = Time.deltaTime;
        Vector3 offset = Vector3.zero;
        if(lungeTime > 0f){
            lungeTime = Mathf.Max(0f, lungeTime - dt);
            offset.x += lungeDistance * Mathf.Sin((1f - lungeTime / LungeDuration) * Mathf.PI);
        }
        if(shakeTime > 0f){
            shakeTime = Mathf.Max(0f, shakeTime - dt);
            offset.x += Mathf.Sin(shakeTime * 90f) * shakeDistance * (shakeTime / ShakeDuration);
        }
        transform.position = rest + offset;

        Color color = restColor;
        if(flashTime > 0f){
            flashTime = Mathf.Max(0f, flashTime - dt);
            color = Color.Lerp(restColor, flashColor, flashTime / FlashDuration);
        }
        if(fading){
            fadeTime = Mathf.Max(0f, fadeTime - dt);
            color.a = fadeTime / FadeDuration;
            if(fadeTime <= 0f){
                fading = false;
                sprite.enabled = false;
                color.a = 1f;
            }
        }
        sprite.color = color;
    }
}
