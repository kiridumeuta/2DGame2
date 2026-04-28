using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    private bool OnPlayer;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (OnPlayer==true)
            {
                Debug.Log("ƒhƒA‚É“ü‚Á‚½");
                SceneManager.LoadScene("Main");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnPlayer = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnPlayer = false;
    }
}
