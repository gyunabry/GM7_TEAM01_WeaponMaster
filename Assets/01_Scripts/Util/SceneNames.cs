using System.Collections.Generic;

public enum SceneType
{
    None,
    Title,
    Game
}

public static class SceneNames
{
    private static readonly Dictionary<SceneType, string> sceneTable
        = new Dictionary<SceneType, string>()
    {
        { SceneType.Title, "TitleScene"},
        { SceneType.Game, "PrototypeScene"}
    };

    public static string GetSceneName(SceneType type)
    {
        return sceneTable[type];
    }
}
