using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Arrive : MonoBehaviour
{
    public static int valid = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Car")
        {
            if (valid == 1)
            {
                Scene scene = SceneManager.GetActiveScene();
                if (scene.name == "Track1")
                {
                    SceneManager.LoadScene("Track2");
                }

                if (scene.name == "Track2")
                {
                    SceneManager.LoadScene("Track3");
                }

                if (scene.name == "Track3")
                {
                    Application.Quit();
                }

            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Car")
        {
            valid = 1;
        }
    }
    void Start()
    {
        valid = 0;
    }
}
