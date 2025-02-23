using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    private Button button;
    private bool clicked = false;
    
    public void Initialize()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        Invoke("CheckMiss", 1f); // بعد از ۱ ثانیه بررسی کند که کلیک نشده است یا نه
    }

    void OnButtonClick()
    {
        clicked = true;
        GameManagerMohsen.instance.OnButtonClicked(gameObject);
    }
    
    void CheckMiss()
    {
        if (!clicked) // اگر بازیکن کلیک نکرده باشد
        {
            GameManagerMohsen.instance.CheckMissedButtons();
            Destroy(gameObject);
        }
    }
}
