using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDisplay : MonoBehaviour
{
    public GameObject maskSprint;
    public GameObject maskGlide;
    public GameObject maskWeb;
    public GameObject maskLife;

    public void activateMaskSprint()
    {
        maskSprint.SetActive(true);
    }
    public void activateMaskGlide()
    {
        maskGlide.SetActive(true);
    }
    public void activateMaskWeb()
    {
        maskWeb.SetActive(true);
    }
    public void activateMaskLife()
    {
        maskLife.SetActive(true);
    }
}
