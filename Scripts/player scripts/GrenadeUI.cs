using UnityEngine;
using UnityEngine.UI;

public class GrenadeUI : MonoBehaviour
{
    public GrenadeThrower grenadeThrower; // —сылка на скрипт гранат
    public Text grenadeText; // —сылка на Text элемент

    void Update()
    {
        if (grenadeThrower != null && grenadeText != null)
        {
            grenadeText.text = $"{grenadeThrower.currentGrenades}/{grenadeThrower.maxGrenades}";
        }
    }
}