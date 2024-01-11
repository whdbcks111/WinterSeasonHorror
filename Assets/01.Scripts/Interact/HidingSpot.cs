using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : InteractableObject
{
   
    

    public override void OnInteract()
    {
        if (Player.Instance.IsHidden)
        {
            Player.Instance.Reveal();
        }
        else
        {
            Player.Instance.Hide();
        }
    }

   
    
}
