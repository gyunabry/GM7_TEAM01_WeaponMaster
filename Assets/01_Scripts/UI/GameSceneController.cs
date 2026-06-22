using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    #region 일시정지 메뉴
    public void OnClickResumeButton()
    {

    }

    public void OnClickRestartButton()
    {

    }

    public void OnClickOptionButton()
    {

    }

    public void OnClickExitButton()
    {
        Debug.Log("타이틀로 돌아갑니다.");
        GameSceneManager.Instance.LoadScene(SceneType.Title);
    }
    #endregion
}
