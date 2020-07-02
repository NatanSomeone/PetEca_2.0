using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;


public class ClickableText : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    TextMeshProUGUI text;
    Menu_manager menu;
    int linkIndex => TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, null);
    TMP_LinkInfo linkInfo => (linkIndex > -1) ? text.textInfo.linkInfo[linkIndex] : default;

    //bool overTheText;



    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        menu = FindObjectOfType<Menu_manager>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (linkIndex > -1)
            {
                Application.OpenURL(linkInfo.GetLinkText());
                //Debug.Log(linkInfo.GetLinkText());
                Debug.Log(linkIndex);
            }
        }
    }
    string originalText;
    bool highlightText;
    public void OnPointerEnter(PointerEventData eventData)
    {
        originalText = text.text;
        highlightText = true;
        StartCoroutine(waitToHighlight());
        //overTheText = true;
    }

    IEnumerator waitToHighlight()
    {
        while (highlightText)
        {
            if (linkIndex > -1)
            {
                int i = -1;
                text.text = Regex.Replace(originalText, @"(<link\b[^>]*>(.*?)</link>)",
                  (m) => (++i == linkIndex) ? $@"<b><i>{m.Value} </b></i> " : m.Value);
                break;
            }
            yield return null;

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlightText = false;
        text.text = originalText;
        //overTheText = false;
    }

    float countDown;
    private void Update()
    {

        //TOD  arruma isso
        //if (overTheText && (linkIndex > -1)) // apontando pro link
        //{

        //    countDown = 1;
        //    if(!menu.infoBox.gameObject.activeSelf)
        //    menu.infoBox.gameObject.SetActive(true);
        //    menu.infoBox.GetComponentInChildren<TextMeshProUGUI>().text = linkInfo.GetLinkText();
        //    menu.infoBox.position = Input.mousePosition;
        //}
        //else
        //{
        //    countDown -= Time.deltaTime;
        //}
        //if (countDown <= 0)
        //{
        //    menu.infoBox.gameObject.SetActive(false);
        //    countDown = 0;
        //}


    }

}
