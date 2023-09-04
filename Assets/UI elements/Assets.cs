using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Assets : MonoBehaviour
{
    public void Scene1()
    {
        SceneManager.LoadScene("Credits");
    }
    public void Scene2()
    {
        SceneManager.LoadScene("AssetsScene");
    }
}
