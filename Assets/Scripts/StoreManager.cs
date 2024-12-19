using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public ItemCosts itemCosts; // ScriptableObject referansı
    public List<Button> buyButtons; // Inspector'dan eklenen butonlar
    
    public List<TextMeshProUGUI> priceTexts; // Inspector'dan eklenen fiyat textleri
    [SerializeField] private TextMeshProUGUI notEnoughMoneyText;
    [SerializeField] GiveMixTips giveMixTips;
    [SerializeField] MixTasksManager mixTasksManager;

    private Coroutine notEnoughMoneyCoroutine; // Coroutine referansı

    private void Start()
    {
        // Tüm butonları döngü ile tıklama olaylarına bağla
        for (int i = 0; i < buyButtons.Count; i++)
        {
            int index = i; // Local değişkenle index tut
            buyButtons[i].onClick.AddListener(() => OnBuyButtonClicked(index));
        }
        priceTexts.ForEach(priceText => priceText.text = itemCosts.items[priceTexts.IndexOf(priceText)].price.ToString() + "$");
        itemCosts.items.ForEach(item => item.isPurchased = false);
        itemCosts.isPlayerHasSpeaker = false;
    }

    private void OnBuyButtonClicked(int buttonIndex)
    {
        // ScriptableObject üzerinden item verisini al
        if (buttonIndex < 0 || buttonIndex >= itemCosts.items.Count)
        {
            Debug.LogError("Geçersiz buton indexi!");
            return;
        }

        Item item = itemCosts.items[buttonIndex];

        // Eğer item zaten satın alınmışsa uyar
        if (item.isPurchased)
        {
            Debug.Log($"{item.itemName} zaten satın alınmış.");
            return;
        }

        // Oyuncunun parası yeterli mi kontrol et
        if (PlayerStats.Instance.money >= item.price)
        {
            // Parayı düşür ve satın alındı olarak işaretle
            PlayerStats.Instance.money -= item.price;
            PlayerStatsUI.Instance.UpdateMoneyText();
            item.isPurchased = true;

            // Butonu devre dışı bırak ve UI güncelle
            buyButtons[buttonIndex].interactable = false;
            priceTexts[buttonIndex].text = "Satın Alındı!";

            itemCosts.isPlayerHasSpeaker = true;
            if(item.itemName == "Speaker 1")
            {
                giveMixTips.howMuchTipsToGive = 30;
                Debug.Log("Speaker 1 satın alındı!");
            }
            else if(item.itemName == "Speaker 2")
            {
                giveMixTips.howMuchTipsToGive = 20;
                Debug.Log("Speaker 2 satın alındı!");
            }
            
            if(itemCosts.isPlayerHasSpeaker && mixTasksManager.isTaskActive)
            {
                giveMixTips.SetMixTipsActive(true);    
            }
            
        }
        else
        {
            ShowNotEnoughMoneyText();
        }
    }

    private void ShowNotEnoughMoneyText()
    {
        // Eğer bir coroutine çalışıyorsa iptal et
        if (notEnoughMoneyCoroutine != null)
        {
            StopCoroutine(notEnoughMoneyCoroutine);
        }

        // Text'i aktif yap ve yeni coroutine başlat
        notEnoughMoneyText.gameObject.SetActive(true);
        notEnoughMoneyCoroutine = StartCoroutine(HideNotEnoughMoneyTextAfterDelay(3f));
    }

    private IEnumerator HideNotEnoughMoneyTextAfterDelay(float delay)
    {
        // Belirtilen süreyi bekle
        yield return new WaitForSeconds(delay);

        // Text'i deaktif yap
        notEnoughMoneyText.gameObject.SetActive(false);

        // Coroutine'i sıfırla
        notEnoughMoneyCoroutine = null;
    }
}
