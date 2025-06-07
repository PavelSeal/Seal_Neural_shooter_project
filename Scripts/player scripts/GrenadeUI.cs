using UnityEngine;
using UnityEngine.UI;

public class GrenadeUI : MonoBehaviour
{
    public GrenadeThrower grenadeThrower;
    public Text grenadeText; 

    void Update()
    {
        if (grenadeThrower != null && grenadeText != null)
        {
            grenadeText.text = $"{grenadeThrower.currentGrenades}/{grenadeThrower.maxGrenades}";
        }
    }
}