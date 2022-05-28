using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI armourText;
    private TextMeshProUGUI ammoText;
    // Start is called before the first frame update
    void Start()
    {
        armourText = GetComponent<TextMeshProUGUI>();
        ammoText = GetComponent<TextMeshProUGUI>();
    }
    public void UpdateArmourText(PlayerCollect playerCollect)
    {
        armourText.text = playerCollect.armourAmount.ToString();
        
    }
    
    public void UpdateAmmoText(PlayerCollect playerCollect)
    {
        ammoText.text = playerCollect.ammoAmount.ToString();
    }
}
