using UnityEngine;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    public Image imageStar1;
    public Image imageStar2;
    public Image imageStar3;
    void Start()
    {
        if (GameMaster.instance.timerValue <= GameMaster.instance.timeStars[2])
        {
            imageStar1.color = new Color(255, 255, 225, 100);
        }
        
        if (GameMaster.instance.timerValue <= GameMaster.instance.timeStars[1])
        {
            imageStar2.color = new Color(255, 255, 225, 100);
        }
        
        if (GameMaster.instance.timerValue <= GameMaster.instance.timeStars[0])
        {
            imageStar3.color = new Color(255, 255, 225, 100);
        }
    }
}
