using UnityEngine;
using TMPro;
using System.Collections.Generic;

// Numbers that pop over a combatant's head when its HP changes: red for HP lost (hits and ticks alike), green for HP
// healed. They rise and fade; only the picture, the rules never wait for them. The layer stretches over the whole
// canvas with its pivot at the bottom left, like the enemy cards' layer.
public class FloatingNumbersUI : MonoBehaviour
{
    public RectTransform layer;
    // Inactive; copied for every number.
    public TextMeshProUGUI template;
    public Color lossColor = new Color32(0xFF, 0x5A, 0x4A, 0xFF);
    public Color healColor = new Color32(0x7F, 0xE0, 0x7A, 0xFF);

    private const float Lifetime = 0.9f;
    private const float Rise = 110f;

    private class Number
    {
        public TextMeshProUGUI text;
        // Its own copy of the font's material, with an outline; destroyed with the number.
        public Material material;
        public Vector2 start;
        public float age;
    }

    private readonly Dictionary<Entity, int> lastHP = new Dictionary<Entity, int>();
    private readonly Dictionary<Entity, System.Action> handlers = new Dictionary<Entity, System.Action>();
    private readonly List<Number> numbers = new List<Number>();

    // Starts watching a combatant; its current HP is the baseline, so nothing pops for the battle's setup.
    public void Track(Entity entity){
        if(entity == null || handlers.ContainsKey(entity)) return;
        lastHP[entity] = entity.currentHP;
        System.Action handler = () => OnHealthChanged(entity);
        handlers[entity] = handler;
        entity.OnHealthChanged += handler;
    }

    void OnDestroy(){
        foreach(KeyValuePair<Entity, System.Action> pair in handlers){
            if(pair.Key != null) pair.Key.OnHealthChanged -= pair.Value;
        }
    }

    private void OnHealthChanged(Entity entity){
        int previous = lastHP[entity];
        lastHP[entity] = entity.currentHP;
        int change = entity.currentHP - previous;
        if(change == 0) return;
        Spawn(entity, change < 0 ? "-" + (-change) : "+" + change, change < 0 ? lossColor : healColor);
    }

    private void Spawn(Entity entity, string value, Color color){
        Camera view = Camera.main;
        SpriteRenderer sprite = entity.GetComponent<SpriteRenderer>();
        if(view == null || sprite == null || sprite.sprite == null) return;
        // At chest height (clear of the intent over the head), a little to one side so numbers in a row don't sit
        // on top of each other.
        Bounds local = sprite.sprite.bounds;
        Vector3 head = sprite.transform.TransformPoint(new Vector3(local.center.x, Mathf.Lerp(local.min.y, local.max.y, 0.6f), 0f));
        Vector2 point = Vector2.Scale(view.WorldToViewportPoint(head), layer.rect.size);
        point.x += Random.Range(-40f, 40f);

        TextMeshProUGUI text = Instantiate(template, layer);
        text.gameObject.SetActive(true);
        text.text = value;
        text.color = color;
        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = Vector2.zero;
        rect.anchoredPosition = point;
        // A dark outline so the number reads over the arena. A per-number copy of the font's own material keeps the
        // dynamic atlas's texture (a separate material asset lost it).
        Material material = text.fontMaterial;
        material.EnableKeyword(ShaderUtilities.Keyword_Outline);
        material.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.28f);
        material.SetColor(ShaderUtilities.ID_OutlineColor, new Color32(0x1A, 0x0E, 0x0A, 0xFF));
        material.SetFloat(ShaderUtilities.ID_FaceDilate, 0.1f);
        text.fontMaterial = material;
        numbers.Add(new Number{ text = text, material = material, start = point });
    }

    void Update(){
        for(int i = numbers.Count - 1; i >= 0; i--){
            Number number = numbers[i];
            number.age += Time.deltaTime;
            float t = number.age / Lifetime;
            if(t >= 1f){
                Destroy(number.text.gameObject);
                Destroy(number.material);
                numbers.RemoveAt(i);
                continue;
            }
            // Pops in big, rises fast then slower, fades over its last 40%.
            float scale = t < 0.12f ? Mathf.Lerp(1.6f, 1f, t / 0.12f) : 1f;
            number.text.rectTransform.localScale = new Vector3(scale, scale, 1f);
            number.text.rectTransform.anchoredPosition = number.start + new Vector2(0f, Rise * (1f - (1f - t) * (1f - t)));
            Color c = number.text.color;
            c.a = t < 0.6f ? 1f : 1f - (t - 0.6f) / 0.4f;
            number.text.color = c;
        }
    }
}
