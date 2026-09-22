using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    public List<EnemyIntent> possibleIntents = new List<EnemyIntent>();

    public EnemyIntent currentIntent;

    public void ChooseNextIntent(){
        if(possibleIntents.Count > 0){
            int randomIndex = Random.Range(0, possibleIntents.Count);
            currentIntent = possibleIntents[randomIndex];

            Debug.Log("Enemy's New Intent: " + currentIntent.intentName + " (" + currentIntent.value + ")");
        }
    }
}
