using UnityEngine;

// Plays a character's frame animations on its SpriteRenderer: idle loops, attack and hit play once and return to idle.
// Only the picture moves; the battle's rules never wait for it. Frames share one canvas and pivot (at the feet), so
// they line up.
public class SpriteAnimator : MonoBehaviour
{
    [System.Serializable]
    public class Clip
    {
        public Sprite[] frames;
        public float framesPerSecond = 10f;
    }

    public SpriteRenderer target;
    public Clip idle;
    public Clip attack;
    public Clip hit;

    private Clip current;
    private float time;

    void Awake(){
        if(target == null) target = GetComponent<SpriteRenderer>();
        current = idle;
    }

    void Update(){
        if(current == null || current.frames == null || current.frames.Length == 0) return;
        time += Time.deltaTime;
        int frame = Mathf.FloorToInt(time * current.framesPerSecond);
        if(current != idle && frame >= current.frames.Length){
            current = idle;
            time = 0f;
            frame = 0;
            if(idle == null || idle.frames == null || idle.frames.Length == 0) return;
        }
        target.sprite = current.frames[frame % current.frames.Length];
    }

    public void PlayAttack(){
        // A multi-hit action deals several hits at once: one swing is enough.
        if(current == attack) return;
        Play(attack);
    }

    public void PlayHit(){
        // A swing isn't cut short by a counter-hit (Thorns); the hit shows when nothing else is playing.
        if(current == attack) return;
        Play(hit);
    }

    private void Play(Clip clip){
        if(clip == null || clip.frames == null || clip.frames.Length == 0) return;
        current = clip;
        time = 0f;
        target.sprite = clip.frames[0];
    }
}
