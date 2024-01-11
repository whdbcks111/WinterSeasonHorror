using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField]
    private bool isInteractable = false; // 상호작용 가능 여부
    [SerializeField] private bool isReusable = false;
    

    [SerializeField] public Sprite originSprite;
    
    [SerializeField] public Sprite interactSprite;
    
    [SerializeField] public AudioClip interactSound;

    [SerializeField] public AudioClip DeInteractSound;

    private bool isOn = false;
  
    
    
    
    private Image _image;
    
    

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        if (isInteractable && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    public abstract void OnInteract(); // 상호작용 시 호출될 추상 메서드

    protected virtual void PlayInteractionSound()
    {
        if (DeInteractSound)
        {
           SoundManager.Instance.PlaySFX(isOn ? interactSound : DeInteractSound ,Player.Instance.transform.position);
        }
        else if (interactSound)
        {
            SoundManager.Instance.PlaySFX( interactSound ,Player.Instance.transform.position);
        }
    }

    protected virtual void ProvideVisualFeedback(bool isInteract)
    {
        if(_image)
            _image.sprite = isInteract ? interactSprite : originSprite;
    }

    private void SetBangMark(bool isOn)
    {
        Player.Instance.BangMarkVisible = isOn;
    }

    public void Interact()
    {
        if (isInteractable)
        {
            isOn = !isOn;   
            OnInteract();
            PlayInteractionSound();
            ProvideVisualFeedback(true);
            
            if(!isReusable)
                isInteractable = false;
        }
    }

    protected virtual void PlayerTrigger(bool isOn)
    {
        isInteractable = isOn;
        SetBangMark(isOn);
    }
   
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.TryGetComponent(out Player p))
        {
            PlayerTrigger(true);
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player p))
        {
            PlayerTrigger(false);
        }
    }

}