using System.Collections;
using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

internal class SceneController : SingletonMonoBehaviour<SceneController>
{
    #region Other Methods

    private IEnumerator _coroutine;
    public void LoadScene(int sceneIndex, float waitTime)
    {
        _coroutine = LoadSceneCoroutine(sceneIndex, waitTime);
        StartCoroutine(_coroutine);
    }
    public void LoadScene(int sceneIndex)
    {
        _coroutine = LoadSceneCoroutine(sceneIndex, 0f);
        StartCoroutine(_coroutine);
    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadSceneAsync(sceneIndex);
        StopCoroutine(_coroutine);
    }

    #endregion

}
