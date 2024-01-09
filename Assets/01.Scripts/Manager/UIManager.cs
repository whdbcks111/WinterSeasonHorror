using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Searcher;

public class UIManager : MonoBehaviour
{
    private Dictionary<string, List<Image>> imageDictionary = new Dictionary<string, List<Image>>();

    public Sprite testSprite;
    public Canvas canvas;
    public Image horrorImage; // 공포 이미지
    public Image imagePrefab;

    [SerializeField] private int maxImageCount = 3;
    private Camera cam;
    private Color originColor;

    private bool flag = false;
    [Range(0f, 3f)] public float displayImageTime = 0f; // 이미지가 사라지는 데 걸리는 시간
    [Range(0f, 3f)] public float fadeOutTime = 2.0f; // 이미지가 사라지는 데 걸리는 시간

    //public List<Image> images = new List<Image>();

    // 화면 맞춤 옵션
    public enum ScreenFit
    {
        Fill,
        Width,
        Height,
        Auto
    }

    public ScreenFit screenFit = ScreenFit.Fill;

    void Start()
    {
        originColor = Color.white;
        canvas = horrorImage.GetComponentInParent<Canvas>();
        cam = Camera.main;

    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("FitImage");
            ImageToFitScreen(testSprite, screenFit);

        }


    }
    
    
    public List<Image> FindGetListContainingImage(Image targetImage)
    {
        foreach (var pair in imageDictionary)
        {
            
            if (pair.Key.Equals(targetImage.sprite.name)) // 이미지 비교
            {
                return pair.Value;
            }
            
        }
        // 매치되는 이미지 리스트가 없는 경우 만들어서 리턴
        var imageList = new List<Image>();
        var i = Instantiate(targetImage, Vector3.zero, quaternion.identity, canvas.transform);
        imageDictionary.Add(targetImage.sprite.name, imageList);
        i.transform.localPosition = Vector3.zero;
        imageList.Add(i);
        return imageList; 
    }

    void ImageToFitScreen(Sprite sprite, ScreenFit fitType)
    {

        imagePrefab.sprite = sprite;
        var images = FindGetListContainingImage(imagePrefab);

        Image _image = null;


        foreach (var i in images)
        {

            if (!i.IsActive())
            {
                _image = i;
                break;
            }
        }

        if (!_image)
        {

            if (images.Count > maxImageCount)
            {

                flag = true;
                return;
            }

            Debug.Log("Instantiate");
            /*GameObject obj = new GameObject();
            obj.AddComponent<Image>().sprite = imagePrefab.sprite*/
            ;
            _image = Instantiate(imagePrefab, Vector3.zero, quaternion.identity, canvas.transform);
            _image.transform.localPosition = Vector3.zero;
            images.Add(_image);

        }

        if (horrorImage == null) return;
        _image.color = originColor;
        _image.gameObject.SetActive(true);
        RectTransform rectTransform = _image.GetComponent<RectTransform>();
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        switch (fitType)
        {
            //1920x 1080
            case ScreenFit.Fill:
                rectTransform.sizeDelta = new Vector2(screenWidth, screenHeight);
                break;
            case ScreenFit.Width:
                rectTransform.sizeDelta = new Vector2(screenWidth,
                    screenWidth / horrorImage.sprite.bounds.size.x * horrorImage.sprite.bounds.size.y);
                break;
            case ScreenFit.Height:
                rectTransform.sizeDelta =
                    new Vector2(screenHeight / horrorImage.sprite.bounds.size.y * horrorImage.sprite.bounds.size.x,
                        screenHeight);
                break;
            // 오토
            case ScreenFit.Auto:
                if (screenWidth - screenHeight > (screenWidth+screenHeight)/ 3 )
                {
                    rectTransform.sizeDelta = new Vector2(screenWidth, screenHeight);
                }
                else
                {
                    rectTransform.sizeDelta = (Screen.height > Screen.width)
                        ? new Vector2(screenHeight / horrorImage.sprite.bounds.size.y * horrorImage.sprite.bounds.size.x,
                            screenHeight)
                        : new Vector2(screenWidth,
                            screenWidth / horrorImage.sprite.bounds.size.x * horrorImage.sprite.bounds.size.y);
                }
                break;
        }

        StartCoroutine(FadeOutImage(_image));
    }

  
    void ImageToFitScreen(PanelUI panelUI)
    {
        
        imagePrefab.sprite = panelUI.image.sprite;
        var images = FindGetListContainingImage(imagePrefab);
       
        Image _image = null;
  

        foreach (var i in images)
        {

            if (!i.IsActive())
            {
                _image = i; 
                break;
            }
        }

        if (!_image)
        {

            if (images.Count > maxImageCount)
            {

                flag = true;
                return;
            }

            Debug.Log("Instantiate");
            /*GameObject obj = new GameObject();
            obj.AddComponent<Image>().sprite = imagePrefab.sprite*/;
            _image = Instantiate(imagePrefab, Vector3.zero, quaternion.identity, canvas.transform);
            _image.transform.localPosition = Vector3.zero;
            images.Add(_image);

        }

        if (horrorImage == null) return;
        _image.color = originColor;
        _image.gameObject.SetActive(true);
        RectTransform rectTransform = _image.GetComponent<RectTransform>();
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        switch (panelUI.fitType)
        {
            case ScreenFit.Fill:
                rectTransform.sizeDelta = new Vector2(screenWidth, screenHeight);
                break;
            case ScreenFit.Width:
                rectTransform.sizeDelta = new Vector2(screenWidth,
                    screenWidth / horrorImage.sprite.bounds.size.x * horrorImage.sprite.bounds.size.y);
                break;
            case ScreenFit.Height:
                rectTransform.sizeDelta =
                    new Vector2(screenHeight / horrorImage.sprite.bounds.size.y * horrorImage.sprite.bounds.size.x,
                        screenHeight);
                break;
        }

        StartCoroutine(FadeOutImage(_image));
    }

    IEnumerator FadeOutImage(Image image)
    {

        if (image == null) yield break;
        flag = true;
        
        float elapsedTime = 0.0f;
        Color originalColor = image.color;
        var p = image.GetComponent<PanelUI>();
        if (p)
        {
            yield return new WaitForSeconds(displayImageTime);
            while (elapsedTime < p.fadeOutTime)
            {
                if (flag && image.sprite.name.Equals(imagePrefab.sprite.name))
                {
                    elapsedTime = 0;
                    flag = false;
                }

                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(1.0f - (elapsedTime / p.fadeOutTime));
                image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(displayImageTime);
            while (elapsedTime < fadeOutTime)
            {
                if (flag && image.sprite.name.Equals(imagePrefab.sprite.name))
                {
                    elapsedTime = 0;
                    flag = false;
                }

                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(1.0f - (elapsedTime / fadeOutTime));
                image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
        }

        flag = false;
        image.color = originalColor;
        image.gameObject.SetActive(false); // 이미지 비활성화
    }
}
