using UnityEngine;
using UnityEngine.SceneManagement;

namespace Powers
{
    public class TryAgain : MonoBehaviour
    {
        public void RestartLevel()
        {
            SceneManager.LoadScene("PowersScene");
        }
    }
}

