using System.Collections;
using UnityEngine;

// The beast's bite at the end of the player's turn, shown a moment after the player's own action so it reads as a
// partner acting on its own (GDD "Beastmaster"). Buttons are off meanwhile. Only the pacing is here: the bite and the
// turn's end are BattleFlow's, which the simulator runs without pauses.
public class BeastBiteState : BattleState
{
    // Seconds after the player's action before the beast leaps, and after its bite before the turn ends.
    public const float PauseBeforeBite = 0.8f;
    public const float PauseAfterBite = 0.35f;

    private Coroutine routine;

    public BeastBiteState(BattleManager manager) : base(manager){ }

    public override void Enter(){
        if(UIManager.Instance != null) UIManager.Instance.ShowEnemyTurn("Your beast");
        routine = battleManager.StartCoroutine(Run());
    }

    private IEnumerator Run(){
        yield return new WaitForSeconds(PauseBeforeBite);
        battleManager.flow.BeastBite();
        if(battleManager.CheckBattleOver()) yield break;
        yield return new WaitForSeconds(PauseAfterBite);
        battleManager.flow.EndPlayerTurn(false);
        if(battleManager.CheckBattleOver()) yield break;
        routine = null;
        battleManager.ChangeState(new EnemyTurnState(battleManager));
    }

    public override void Execute(){ }

    public override void Exit(){
        if(routine != null) battleManager.StopCoroutine(routine);
        routine = null;
    }
}
