using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HidingSpot : InteractableObject
{

    protected override void InvokeInteract()
    {
        if (isInteractable && Input.GetKeyDown(KeyCode.E))
        {
            if(!Player.Instance.IsInHideCooldown)
                Interact();
        }
    }
    protected override void PlayerTrigger(bool isOn)
    {
        if (Player.Instance.IsInHideCooldown && isOn)
        {
            return;
        }
        isInteractable = isOn;
        SetBangMark(isOn);
    }

    protected override void TriggerStay( )
    {
        if (isInteractable)
        {
            if(Player.Instance.IsInHideCooldown&& Player.Instance.BangMarkVisible)
            {
                SetBangMark(false);
                
            }
            else if(!Player.Instance.IsInHideCooldown&&! Player.Instance.BangMarkVisible)
            {
                SetBangMark(true);
            }
            
        }
    }


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
