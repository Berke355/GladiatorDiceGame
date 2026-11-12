using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    // The scene's enemy object, which takes the stats of the picked boss or the first enemy; copies of it hold the
    // other members of an enemy group.
    public Entity enemy;
    public Entity player;
    // The arena behind everything; boss battles use the night arena.
    public SpriteRenderer arenaBackground;
    public Sprite bossArena;

    private BattleState currentState;
    [System.NonSerialized]
    public DieFace currentRolledFace;

    // Borrowed from the run in Awake; the run owns the die.
    [System.NonSerialized]
    public List<DieFace> currentDiceFaces;

    // The turn steps and the equipment triggers, shared with the balance simulator.
    [System.NonSerialized]
    public BattleFlow flow;

    void Awake(){
        // A run always starts by choosing the starting face. If this scene was played directly
        // (e.g. Play pressed with Battle open in the Editor), send the player to the run start screen.
        // Disabling this component also skips Start, so no turn begins.
        if(!RunManager.Instance.run.startingFaceChosen){
            enabled = false;
            RunManager.Instance.GoToRunStart();
            return;
        }

        // Load the run's die and the player's HP before UIManager.Start reads them.
        RunData run = RunManager.Instance.run;
        currentDiceFaces = run.dieFaces;
        player.maxHP = run.maxHP;
        player.currentHP = run.currentHP;

        // The run picks this battle's boss or enemy; the scene's Enemy object takes its stats.
        BossSO boss;
        EnemyGroup encounter;
        RunManager.Instance.TakeNextBattle(out boss, out encounter);
        List<BattleEnemy> enemies = new List<BattleEnemy>();
        if(boss != null){
            if(arenaBackground != null && bossArena != null) arenaBackground.sprite = bossArena;
            enemy.entityName = boss.bossName;
            enemy.maxHP = boss.maxHP;
            enemy.currentHP = boss.maxHP;
            enemies.Add(new BattleEnemy(enemy, new BossBrain(boss, enemy, player, currentDiceFaces), true){ sprite = boss.sprite, spriteHeight = boss.spriteHeight });
        }
        else{
            List<string> names = MemberNames(encounter);
            for(int i = 0; i < encounter.members.Count; i++){
                EnemySO enemyData = encounter.members[i];
                Entity entity = i == 0 ? enemy : Instantiate(enemy, enemy.transform.parent);
                entity.name = "Enemy " + names[i];
                entity.entityName = names[i];
                // Later battles of a tier are tougher: more HP and starting Rage (see GDD "Difficulty Curve").
                int battleNumber = run.battlesWon + 1;
                entity.maxHP = RunManager.Instance.encounters.ScaledHP(encounter, enemyData, battleNumber);
                entity.currentHP = entity.maxHP;
                int startingRage = RunManager.Instance.encounters.ScalingFor(battleNumber).startingRage;
                if(startingRage > 0) entity.ApplyStatus(StatusType.Rage, startingRage);
                enemies.Add(new BattleEnemy(entity, new EnemyBrain(enemyData, entity, player)){ sprite = enemyData.sprite, spriteHeight = enemyData.spriteHeight });
            }
        }

        // Enemies that join later (Hydra heads) are copies of the scene's enemy object.
        flow = new BattleFlow(player, enemies, currentDiceFaces, run.equipment, run.rerollsPerBattle, () => Instantiate(enemy, enemy.transform.parent), run.dieEnchantments);
        enemySpriteScale = enemy.transform.localScale;
        enemySpriteY = enemy.transform.position.y;
        enemySpriteX = enemy.transform.position.x;
        SpriteRenderer placeholder = enemy.GetComponent<SpriteRenderer>();
        if(placeholder != null){
            placeholderSprite = placeholder.sprite;
            placeholderColor = placeholder.color;
        }
        foreach(BattleEnemy each in flow.enemies) each.entity.OnHealthChanged += UpdateEnemySprites;
        flow.EnemyAdded += added => {
            added.entity.OnHealthChanged += UpdateEnemySprites;
            ArrangeEnemySprites();
        };
        ArrangeEnemySprites();
    }

    // The enemies share the right side of the screen, in turn order, shrinking when there are more of them.
    // An enemy with a sprite stands on the ground line (the placeholder's bottom edge) and is HumanHeight ×
    // spriteHeight tall; one without keeps the scene's placeholder. Dead ones are hidden (a revived Hydra head shows
    // again) but keep their place.
    private const float SpriteAreaLeft = 0.2f;
    private const float SpriteAreaRight = 8.6f;
    // World height of a gladiator-sized sprite.
    private const float HumanHeight = 3.0f;
    private Vector3 enemySpriteScale;
    private float enemySpriteY;
    private float enemySpriteX;
    private Sprite placeholderSprite;
    private Color placeholderColor = Color.white;

    // Each enemy takes its own width; if the row doesn't fit, all of them shrink by the same factor, so a boss stays
    // bigger than its heads. The row is centered on the placeholder's x and kept inside the sprite area.
    private void ArrangeEnemySprites(){
        int count = flow.enemies.Count;
        float area = SpriteAreaRight - SpriteAreaLeft;
        float groundY = enemySpriteY - PlaceholderSize().y / 2f;
        float[] widths = new float[count];
        float total = SpriteGap * (count - 1);
        for(int i = 0; i < count; i++){
            BattleEnemy each = flow.enemies[i];
            widths[i] = Mathf.Max(MinSlotWidth, each.sprite != null ? each.sprite.bounds.size.x * FullScale(each) : PlaceholderSize().x);
            total += widths[i];
        }
        float shrink = Mathf.Min(1f, area / total);
        float left = Mathf.Clamp(enemySpriteX - total * shrink / 2f, SpriteAreaLeft, SpriteAreaRight - total * shrink);
        for(int i = 0; i < count; i++){
            BattleEnemy each = flow.enemies[i];
            Transform sprite = each.entity.transform;
            SpriteRenderer renderer = each.entity.GetComponent<SpriteRenderer>();
            float x = left + widths[i] * shrink / 2f;
            each.slotWidth = widths[i] * shrink;
            left += (widths[i] + SpriteGap) * shrink;
            if(each.sprite != null && renderer != null){
                renderer.sprite = each.sprite;
                renderer.color = Color.white;
                float scale = FullScale(each) * shrink;
                sprite.position = new Vector3(x, groundY, sprite.position.z);
                sprite.localScale = new Vector3(scale, scale, 1f);
            }
            else{
                if(renderer != null){
                    renderer.sprite = placeholderSprite;
                    renderer.color = placeholderColor;
                }
                sprite.position = new Vector3(x, enemySpriteY, sprite.position.z);
                sprite.localScale = new Vector3(enemySpriteScale.x * shrink, enemySpriteScale.y * shrink, enemySpriteScale.z);
            }
        }
        UpdateEnemySprites();
    }

    private const float SpriteGap = 0.15f;
    // Each enemy gets at least its card's width (UIManager's plate), so plates and intents don't overlap.
    private const float MinSlotWidth = 1.75f;

    // The scale that makes the enemy's sprite HumanHeight × spriteHeight tall.
    private static float FullScale(BattleEnemy enemy){
        return HumanHeight * enemy.spriteHeight / enemy.sprite.bounds.size.y;
    }

    private Vector2 PlaceholderSize(){
        return placeholderSprite != null ? Vector2.Scale(placeholderSprite.bounds.size, enemySpriteScale) : new Vector2(3f, 3f);
    }

    private void UpdateEnemySprites(){
        foreach(BattleEnemy each in flow.enemies){
            SpriteRenderer renderer = each.entity.GetComponent<SpriteRenderer>();
            if(renderer != null) renderer.enabled = !each.IsDead;
        }
    }

    // Enemy names, with a letter when the same enemy appears twice ("Tiro A", "Tiro B").
    public static List<string> MemberNames(EnemyGroup encounter){
        List<string> names = new List<string>();
        foreach(EnemySO member in encounter.members){
            int copies = encounter.members.FindAll(other => other == member).Count;
            int index = names.FindAll(name => name.StartsWith(member.enemyName)).Count;
            names.Add(copies > 1 ? member.enemyName + " " + (char)('A' + index) : member.enemyName);
        }
        return names;
    }

    void OnDestroy(){
        if(flow != null){
            foreach(BattleEnemy each in flow.enemies){
                if(each.entity != null) each.entity.OnHealthChanged -= UpdateEnemySprites;
            }
            flow.Dispose();
        }
    }

    public void ChangeState(BattleState newState){
        if(currentState != null){
            currentState.Exit();
        }

        currentState = newState;

        currentState.Enter();
    }

    // Ends the battle if either side is dead. Callers must stop their turn flow when this returns true.
    public bool CheckBattleOver(){
        if(flow.IsOver){
            ChangeState(new BattleOverState(this, !player.IsDead));
            return true;
        }
        return false;
    }

    void Start(){
        // Hook the UI up before the battle starts, so BattleStart items flash too.
        if(UIManager.Instance != null){
            UIManager.Instance.ShowEquipment(RunManager.Instance.run.equipment);
            UIManager.Instance.flow = flow;
            RunData battleRun = RunManager.Instance.run;
            UIManager.Instance.ShowBattleInfo(battleRun.battlesWon + 1, RunManager.Instance.finalBattle, battleRun.gold, flow.enemies[0].endsBattleOnDeath);
            UIManager.Instance.ShowEnemies(flow.enemies, OnEnemyClicked);
            UIManager.Instance.WireDieChips(OnDieFaceClicked);
            flow.EnemyAdded += added => UIManager.Instance.AddEnemyCard(added, flow.enemies.IndexOf(added), OnEnemyClicked);
            flow.triggers.ItemTriggered += UIManager.Instance.FlashEquipment;
        }
        flow.StartBattle();
        if(CheckBattleOver()) return;
        ChangeState(new PlayerRollState(this));
    }

    void Update(){
        if(currentState != null){
            currentState.Execute();
        }
    }

    public void OnActionSelected(int actionIndex){
        ActionType selectedAction = (ActionType)actionIndex;

        if(currentState is PlayerActionSelectState playerState){
            playerState.ExecuteAction(selectedAction);
        }
    }

    // Called by the button on the battle result screen.
    public void OnResultButton(){
        if(currentState is BattleOverState overState){
            overState.Continue();
        }
    }

    // The turn goes on with a new roll right away (Change, Flip, Tumble, a chosen face).
    public void RollAgainAtOnce(){
        DieFace face = flow.Roll();
        currentRolledFace = face;
        Debug.Log("Rolled again. Face: " + face.definition.faceName);
        if(UIManager.Instance != null){
            UIManager.Instance.ShowRolledFace(face, currentDiceFaces, true);
        }
        if(CheckBattleOver()) return;
        ChangeState(new PlayerActionSelectState(this));
    }

    // Called by a die face on the net while the player chooses the next face.
    public void OnDieFaceClicked(int position){
        if(currentState is PlayerChooseFaceState chooseState){
            chooseState.Choose(position);
        }
    }

    // Called by the Pass button.
    public void OnPassClicked(){
        if(currentState is PlayerActionSelectState actionState){
            actionState.Pass();
        }
    }

    // Called by the Reroll button.
    public void OnRerollClicked(){
        if(currentState is PlayerActionSelectState actionState){
            actionState.Reroll();
        }
    }

    // Called by an enemy card: selects that enemy as the target, before or after rolling.
    public void OnEnemyClicked(int index){
        bool choosing = currentState is PlayerRollState || currentState is PlayerActionSelectState;
        if(!choosing || index < 0 || index >= flow.enemies.Count) return;
        if(!flow.SelectTarget(flow.enemies[index]) || UIManager.Instance == null) return;
        // The action previews depend on the target (e.g. Ignite reads its Burn).
        if(currentState is PlayerActionSelectState) UIManager.Instance.ShowRolledFace(currentRolledFace, currentDiceFaces);
        else UIManager.Instance.RefreshEnemies();
    }

    public void OnRolled(){
        if(currentState is PlayerRollState playerState){
            playerState.RollTheDice();
        }
    }
}
