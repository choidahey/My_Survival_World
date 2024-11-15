using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneManager : MonoBehaviour
{
    private Button start_button;
    private string Header = "[ SceneManager ]";

    enum Scene_List
    {
        Intro = 0,
        Main = 1,
    }

    private void Start()
    {
        GameObject start_button_object = GameObject.Find("Start_Button");

        if (start_button_object != null)
        {
            if (start_button_object.TryGetComponent<Button>(out start_button))
                start_button.onClick.AddListener(() => Change_Scene((int)Scene_List.Main));
            else
                Debug.Log(Header + "Please Add Button Component");
        }
        else
            Debug.Log(Header + "Can't Find the Start Button Object");

    }
    void Change_Scene(int index_scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(index_scene);
    }
}
