using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField]
    private bool isInteractable = false; // 상호작용 가능 여부

    [SerializeField]
    private AudioClip interactionSound; // 상호작용 사운드

    [SerializeField] public Sprite originSprite;
    
    [SerializeField] public Sprite interactSprite;
    
    [SerializeField] public AudioClip interactSound;

    public bool isWorking = false;
    
    
    
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
        if (interactionSound != null)
        {
           SoundManager.Instance.PlaySFX(interactSound,Player.Instance.transform.position);
        }
    }

    protected virtual void ProvideVisualFeedback(bool isInteract)
    {
        _image.sprite = isInteract ?  interactSprite : originSprite;
        // 시각적 피드백 로직 구현 (예: 깜빡이는 애니메이션)
    }

    private void SetBangMark(bool isOn)
    {
        Player.Instance.BangMarkVisible = isOn;
    }

    public void Interact()
    {
        if (isInteractable)
        {
            
            OnInteract();
            PlayInteractionSound();
            ProvideVisualFeedback(true);
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
        if (other.CompareTag("Player"))
        {
            
            PlayerTrigger(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerTrigger(false);
        }
    }

}