using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGameButtonScript : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private string scene;

    [SerializeField] private Dropdown dropdownLevelSelect;

    List<string> levels = new List<string>() {"Level1", "Level2"};


    void Start()
    {
        scene = "Level1";

        dropdownLevelSelect.AddOptions(levels);

        button.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        Debug.Log("Entered Game");
        SceneManager.LoadScene(scene);
    }

    public void LevelSelectChanged(int index)
    {
        scene = levels[index];
    }
}
