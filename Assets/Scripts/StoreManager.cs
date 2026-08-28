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
    [SerializeField] List<TextMeshProUGUI> itemDescriptionText;
    
    private Coroutine notEnoughMoneyCoroutine; // Coroutine referansı

    private void Start()
    {
        for (int i = 0; i < itemCosts.items.Count; i++)
        {
            itemDescriptionText[i].text = itemCosts.items[i].description;
        }
        
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

            // Speaker satın alma mantığı
            if(item.itemName == "Speaker 1")
            {
                // Eğer Speaker 2 alınmamışsa Speaker 1'in ayarlarını kullan
                if (!itemCosts.items.Find(x => x.itemName == "Speaker 2").isPurchased)
                {
                    giveMixTips.howMuchTipsToGive = 40;
                    Debug.Log("Speaker 1 satın alındı ve aktif!");
                }
            }
            else if(item.itemName == "Speaker 2")
            {
                giveMixTips.howMuchTipsToGive = 30;
                Debug.Log("Speaker 2 satın alındı ve aktif!");
            }

            // En az bir hoparlör satın alındıysa
            itemCosts.isPlayerHasSpeaker = true;
            
            // Eğer aktif görev varsa tolerance area'yı ayarla
            if(mixTasksManager.isTaskActive && mixTasksManager.selectedTask != null)
            {
                giveMixTips.SetMixTipsActive(true);
                giveMixTips.SetToleranceArea(
                    mixTasksManager.selectedTask.tolerance,
                    mixTasksManager.selectedTask.ch1IdealLevel,
                    mixTasksManager.selectedTask.ch2IdealLevel,
                    mixTasksManager.selectedTask.ch3IdealLevel,
                    mixTasksManager.selectedTask.ch4IdealLevel
                );
            }
            else
            {
                giveMixTips.SetMixTipsActive(false);
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

    // Speaker durumunu kontrol eden yeni method
    public void UpdateSpeakerSettings()
    {
        // Speaker 2 alınmışsa onun ayarlarını kullan
        if (itemCosts.items.Find(x => x.itemName == "Speaker 2").isPurchased)
        {
            giveMixTips.howMuchTipsToGive = 30;
        }
        // Sadece Speaker 1 alınmışsa onun ayarlarını kullan
        else if (itemCosts.items.Find(x => x.itemName == "Speaker 1").isPurchased)
        {
            giveMixTips.howMuchTipsToGive = 40;
        }
    }
}
