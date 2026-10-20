using UnityEngine;

// Starts a countdown to a big hit equal to my value (e.g. the Minotaur's Stampede). The effect only raises a request;
// BossBrain runs the countdown, since effects don't control the turn. Its length and stagger come from the boss.
[CreateAssetMenu(fileName = "NewStartCountdownEffect", menuName = "Dice/Effects/Start Countdown Effect")]
public class StartCountdownEffectSO : EffectSO
{
    public override void Execute(EffectContext context){
        context.countdownDamage = context.value;
    }

    public override string Describe(EffectContext context){
        return "starts a countdown to " + (context.value + context.RageBonus) + " damage";
    }
}
