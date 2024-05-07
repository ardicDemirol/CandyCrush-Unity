using System.Collections;
using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

internal class SceneController : SingletonMonoBehaviour<SceneController>
{
    #region Other Methods

    public void LoadScene(int sceneIndex, float waitTime)
    {
        StartCoroutine(LoadSceneCoroutine(sceneIndex, waitTime));
    }
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneCoroutine(sceneIndex, 0f));
    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadSceneAsync(sceneIndex);
    }

    #endregion

}
