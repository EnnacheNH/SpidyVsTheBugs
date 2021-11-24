using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ObjectiveTitle : MonoBehaviour
{
    public Text objectiveText;
    public string additionalText;

    void Start()
    {
        objectiveText.text = SceneManager.GetActiveScene().name.ToUpper() +":\n"+ additionalText;
    }
}
