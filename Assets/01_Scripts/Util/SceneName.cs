using System.Collections.Generic;

public enum SceneType
{
    None,
    Title,
    Game
}

public static class SceneName
{
    private static readonly Dictionary<SceneType, string> sceneTable
        = new Dictionary<SceneType, string>()
    {
        { SceneType.Title, "TitleScene"},
        { SceneType.Game, "GameScene"},
    };

    public static string GetSceneName(SceneType type)
    {
        return sceneTable[type];
    }
}
