using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI armourText;
    // Start is called before the first frame update
    void Start()
    {
        armourText = GetComponent<TextMeshProUGUI>();
    }
    public void UpdateArmourText(PlayerCollect playerCollect)
    {
        armourText.text = playerCollect.armourAmount.ToString();
    }
    
}
