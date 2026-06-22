using System.Collections.Generic;

public static class GameSceneData
{
    public static List<WaveData> SelectedMapWaves { get; set; }
    public static Difficulty SelectedDifficulty { get; set; } = Difficulty.Normal;

    public static string SelectedSceneName;
    

    
}