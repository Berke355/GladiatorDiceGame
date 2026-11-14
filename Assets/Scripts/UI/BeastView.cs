using System.Collections;
using TMPro;
using UnityEngine;

// The Beastmaster's beast beside the player (GDD "Beastmaster"): hidden while no beast is out, fades in when one is
// summoned, lunges toward the enemy on every bite, flashes red when it is hurt and fades out when it dies. Under its
// feet a small HP bar with its HP and Hound. Only the picture: the rules are Beast and BeastCompanion. Its resting
// place is where it stands in the scene.
public class BeastView : MonoBehaviour
{
    public Entity owner;
    public SpriteRenderer spriteRenderer;
    public float lungeDistance = 0.6f;
    public float lungeTime = 0.18f;
    public float fadeTime = 0.35f;
    // The HP bar over the beast's back (under its feet it read as part of the player's bar), in world units.
    public float barWidth = 1.1f;
    public float barHeight = 0.16f;
    public float barLift = 0.12f;

    private Vector3 rest;
    // The beast's height at rest, so the bar stands over it.
    private float height;
    private bool shown;
    private Coroutine fade, lunge, flash;
    private BeastCompanion watched;
    private int lastHP;
    private SpriteRenderer barBack, barFill;
    private TextMeshPro barText;
    private static Sprite pixel;

    void Awake(){
        rest = transform.position;
        height = spriteRenderer != null && spriteRenderer.sprite != null ? spriteRenderer.bounds.size.y : 1f;
        SetAlpha(0f);
        BuildBar();
        ShowBar(false);
    }

    void OnEnable(){
        if(owner == null) return;
        owner.BeastBit += OnBite;
    }

    void OnDisable(){
        if(owner == null) return;
        owner.BeastBit -= OnBite;
    }

    // Polled: the beast is created by the battle's flow after this view starts, and it changes during effects.
    void LateUpdate(){
        if(owner == null) return;
        BeastCompanion beast = owner.beast;
        if(beast != watched){
            watched = beast;
            lastHP = beast != null ? beast.HP : 0;
        }
        bool show = beast != null && beast.IsOut && !owner.IsDead;
        if(beast != null && beast.HP < lastHP && lastHP > 0){
            if(flash != null) StopCoroutine(flash);
            flash = StartCoroutine(Flash());
        }
        lastHP = beast != null ? beast.HP : 0;
        if(show != shown){
            shown = show;
            if(fade != null) StopCoroutine(fade);
            fade = StartCoroutine(Fade(show ? 1f : 0f));
        }
        ShowBar(show);
        if(show) DrawBar(beast);
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

    // Hurt: a red flash and a small shake.
    private IEnumerator Flash(){
        const float time = 0.3f;
        for(float t = 0f; t < time; t += Time.deltaTime){
            float k = 1f - t / time;
            Color color = Color.Lerp(Color.white, new Color(1f, 0.25f, 0.25f), k);
            color.a = spriteRenderer.color.a;
            spriteRenderer.color = color;
            transform.position = rest + new Vector3(Mathf.Sin(t * 70f) * 0.05f * k, 0f, 0f);
            yield return null;
        }
        Color white = Color.white;
        white.a = spriteRenderer.color.a;
        spriteRenderer.color = white;
        transform.position = rest;
    }

    private void SetAlpha(float alpha){
        if(spriteRenderer == null) return;
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    // ---------- HP bar ----------

    private void BuildBar(){
        if(pixel == null){
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            pixel = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0f, 0.5f), 1f);
        }
        int order = spriteRenderer != null ? spriteRenderer.sortingOrder + 2 : 10;
        int layer = spriteRenderer != null ? spriteRenderer.sortingLayerID : 0;
        barBack = MakeBarPart("BeastHPBack", new Color(0.12f, 0.06f, 0.05f, 0.9f), order, layer);
        barFill = MakeBarPart("BeastHPFill", new Color(0.93f, 0.62f, 0.2f, 1f), order + 1, layer);
        GameObject text = new GameObject("BeastHPText");
        barText = text.AddComponent<TextMeshPro>();
        barText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/VT323 SDF");
        barText.fontSize = 4.2f;
        barText.alignment = TextAlignmentOptions.Center;
        barText.rectTransform.sizeDelta = new Vector2(4f, 0.6f);
        barText.sortingLayerID = layer;
        barText.sortingOrder = order + 2;
        barText.outlineWidth = 0.25f;
        barText.outlineColor = Color.black;
    }

    private SpriteRenderer MakeBarPart(string name, Color color, int order, int layer){
        SpriteRenderer part = new GameObject(name).AddComponent<SpriteRenderer>();
        part.sprite = pixel;
        part.color = color;
        part.sortingLayerID = layer;
        part.sortingOrder = order;
        return part;
    }

    private void ShowBar(bool show){
        if(barBack == null) return;
        barBack.enabled = show;
        barFill.enabled = show;
        barText.enabled = show;
    }

    // The bar sits over the beast's resting place (it doesn't follow the lunge), filled by HP / max HP, the numbers above.
    private void DrawBar(BeastCompanion beast){
        Vector3 left = rest + new Vector3(-barWidth / 2f, height + barLift, 0f);
        barBack.transform.position = left - new Vector3(0.03f, 0f, 0f);
        barBack.transform.localScale = new Vector3(barWidth + 0.06f, barHeight + 0.06f, 1f);
        float share = beast.MaxHP > 0 ? Mathf.Clamp01((float)beast.HP / beast.MaxHP) : 0f;
        barFill.transform.position = left;
        barFill.transform.localScale = new Vector3(barWidth * share, barHeight, 1f);
        barText.transform.position = rest + new Vector3(0f, height + barLift + barHeight + 0.16f, 0f);
        int hound = owner.GetStatus(StatusType.Hound);
        barText.text = beast.HP + "/" + beast.MaxHP + (beast.Sheltered ? "  safe" : "") + "   " + GameText.Icon("Hound") + hound;
    }

    void OnDestroy(){
        if(barBack != null) Destroy(barBack.gameObject);
        if(barFill != null) Destroy(barFill.gameObject);
        if(barText != null) Destroy(barText.gameObject);
    }
}
