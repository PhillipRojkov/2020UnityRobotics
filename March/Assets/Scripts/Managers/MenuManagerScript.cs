using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManagerScript : MonoBehaviour
{
    [SerializeField] private bool pauseMenuOpen = false;
    [SerializeField] private GameObject mainMenuButtonGO;
    [SerializeField] private Button mainMenuButton;
    public bool externalPause = false;

    private void Start()
    {
        mainMenuButton.onClick.AddListener(mainMenuButtonTask);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && pauseMenuOpen == false || externalPause == true)
        {
            pauseMenuOpen = true;
            mainMenuButtonGO.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        }
        else if (Input.GetButtonDown("Cancel") && pauseMenuOpen == true)
        {
            pauseMenuOpen = false;
            mainMenuButtonGO.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void mainMenuButtonTask()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
