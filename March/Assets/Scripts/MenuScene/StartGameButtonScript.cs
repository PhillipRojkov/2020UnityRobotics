using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGameButtonScript : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private string NextScene;

    void Start()
    {
        button.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        Debug.Log("Entered Game");
        SceneManager.LoadScene("Level1");
    }
}
