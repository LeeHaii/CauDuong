using UnityEngine;
using UnityEngine.SceneManagement;

public class ToSceneIndex : MonoBehaviour
{
    public void LoadSceneIndex(int index){
        SceneManager.LoadScene(index);
    }
}
