using UnityEngine;

[CreateAssetMenu(fileName = "NewBlockEffect", menuName = "Dice/Effects/Block Effect")]
public class BlockEffectSO : EffectSO
{
    public override void Execute(Entity source, Entity target, int value){
        source.GainBlock(value);
        Debug.Log(value + " kadar blok kazanıldı.");
    }
}
