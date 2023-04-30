using UnityEngine;

[CreateAssetMenu(fileName = "GameStats", menuName = "ScriptableObjects/GameStats", order = 1)]
public class GameStats : ScriptableObject {
    public float gameTime;
    public int flaps;
    public int level;
}