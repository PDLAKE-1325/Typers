using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] float LoadDelay = 1;
    public void LoadScene(string scene)
    {
        StartCoroutine(_LoadScene(scene));
    }
    IEnumerator _LoadScene(string scene)
    {
        yield return new WaitForSeconds(LoadDelay);
        SceneManager.LoadScene(scene);
    }
}
