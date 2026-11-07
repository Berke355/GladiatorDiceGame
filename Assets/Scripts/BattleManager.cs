using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    // The scene's enemy object, which takes the stats of the picked boss or the first enemy; copies of it hold the
    // other members of an enemy group.
    public Entity enemy;
    public Entity player;

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
            enemy.entityName = boss.bossName;
            enemy.maxHP = boss.maxHP;
            enemy.currentHP = boss.maxHP;
            enemies.Add(new BattleEnemy(enemy, new BossBrain(boss, enemy, player, currentDiceFaces), true));
        }
        else{
            List<string> names = MemberNames(encounter);
            for(int i = 0; i < encounter.members.Count; i++){
                EnemySO enemyData = encounter.members[i];
                Entity entity = i == 0 ? enemy : Instantiate(enemy, enemy.transform.parent);
                entity.name = "Enemy " + names[i];
                entity.entityName = names[i];
                entity.maxHP = encounter.MemberHP(enemyData);
                entity.currentHP = entity.maxHP;
                enemies.Add(new BattleEnemy(entity, new EnemyBrain(enemyData, entity, player)));
            }
        }

        // Enemies that join later (Hydra heads) are copies of the scene's enemy object.
        flow = new BattleFlow(player, enemies, currentDiceFaces, run.equipment, run.rerollsPerBattle, () => Instantiate(enemy, enemy.transform.parent));
        enemySpriteScale = enemy.transform.localScale;
        enemySpriteY = enemy.transform.position.y;
        foreach(BattleEnemy each in flow.enemies) each.entity.OnHealthChanged += UpdateEnemySprites;
        flow.EnemyAdded += added => {
            added.entity.OnHealthChanged += UpdateEnemySprites;
            ArrangeEnemySprites();
        };
        ArrangeEnemySprites();
    }

    // The enemies' placeholder sprites share the right side of the screen, in turn order, shrinking when there
    // are more of them. Dead ones are hidden (a revived Hydra head shows again) but keep their place.
    private const float SpriteAreaLeft = 3.3f;
    private const float SpriteAreaRight = 8.7f;
    private Vector3 enemySpriteScale;
    private float enemySpriteY;

    private void ArrangeEnemySprites(){
        int count = flow.enemies.Count;
        float slot = (SpriteAreaRight - SpriteAreaLeft) / count;
        float shrink = count > 1 ? Mathf.Min(1f, slot * 0.85f / SpriteWidth()) : 1f;
        for(int i = 0; i < count; i++){
            Transform sprite = flow.enemies[i].entity.transform;
            float x = count > 1 ? SpriteAreaLeft + slot * (i + 0.5f) : enemy.transform.position.x;
            sprite.position = new Vector3(x, enemySpriteY, sprite.position.z);
            sprite.localScale = new Vector3(enemySpriteScale.x * shrink, enemySpriteScale.y * shrink, enemySpriteScale.z);
        }
        UpdateEnemySprites();
    }

    private float SpriteWidth(){
        SpriteRenderer renderer = enemy.GetComponent<SpriteRenderer>();
        return renderer != null && renderer.sprite != null ? renderer.sprite.bounds.size.x * enemySpriteScale.x : 3f;
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
            UIManager.Instance.ShowEnemies(flow.enemies, OnEnemyClicked);
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
