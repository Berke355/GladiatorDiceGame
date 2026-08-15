using UnityEngine;

[CreateAssetMenu(fileName = "NewBlockEffect", menuName = "Dice/Effects/Block Effect")]
public class BlockEffectSO : EffectSO
{
    public override void Execute(Entity player, Entity enemy, int value){
        player.GainBlock(value);
        Debug.Log(value + " kadar blok kazanıldı.");
    }
}
