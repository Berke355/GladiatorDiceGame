using UnityEngine;
using System.Collections;

// The enemies' turn, paced so it can be read: a pause after the player's action (its swing and numbers), the ENEMY
// TURN banner, then each living enemy acts in turn order with a pause after each, then their statuses tick. Only the pacing is here; the
// steps are BattleFlow's, the same ones the simulator runs without pauses.
public class EnemyTurnState : BattleState
{
    // Seconds after the player's action, after each enemy's action and after the enemies' statuses tick.
    public const float PauseAfterPlayer = 0.9f;
    public const float PauseBetweenEnemies = 0.85f;
    public const float PauseAfterTicks = 0.5f;
    // After the ENEMY TURN banner pops, before the first enemy acts.
    public const float PauseAfterBanner = 0.6f;

    private Coroutine routine;

    public EnemyTurnState(BattleManager manager) : base(manager) {

    }

    public override void Enter(){
        if(UIManager.Instance != null) UIManager.Instance.ShowEnemyTurn();
        routine = battleManager.StartCoroutine(Run());
    }

    private IEnumerator Run(){
        yield return new WaitForSeconds(PauseAfterPlayer);
        if(UIManager.Instance != null) UIManager.Instance.ShowTurnBanner("ENEMY TURN");
        AudioManager.Play(Sfx.EnemyTurn);
        yield return new WaitForSeconds(PauseAfterBanner);
        BattleFlow flow = battleManager.flow;
        // By index: an enemy can join while enemies act (a Hydra head).
        for(int i = 0; i < flow.enemies.Count; i++){
            if(!flow.EnemyActOne(i)) continue;
            if(battleManager.CheckBattleOver()) yield break;
            yield return new WaitForSeconds(PauseBetweenEnemies);
        }

        flow.EndEnemyTurn();
        if(battleManager.CheckBattleOver()) yield break;
        yield return new WaitForSeconds(PauseAfterTicks);

        routine = null;
        battleManager.ChangeState(new PlayerRollState(battleManager));
    }

    public override void Execute(){

    }

    public override void Exit(){
        if(routine != null) battleManager.StopCoroutine(routine);
        routine = null;
    }
}
