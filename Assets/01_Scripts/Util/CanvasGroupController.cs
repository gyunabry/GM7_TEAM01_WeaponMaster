using UnityEngine;

public static class CanvasGroupController
{
    /// <summary>
    /// 캔버스 그룹을 활성화
    /// </summary>
    public static void EnableCG(CanvasGroup cg)
    {
        if (cg != null)
        {
            cg.alpha = 1.0f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }

    /// <summary>
    /// 캔버스 그룹을 비활성화
    /// </summary>
    /// <param name="cg"></param>
    public static void DisableCG(CanvasGroup cg)
    {
        if (cg != null)
        {
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }
}
