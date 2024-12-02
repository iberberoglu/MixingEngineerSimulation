using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupControl : MonoBehaviour
{
    [SerializeField] float scaleSpeed = 2f; // Ölçek değişim hızı
    [SerializeField] float minScale = 0.5f; // Minimum ölçek değeri
    [SerializeField] float maxScale = 1.5f; // Maksimum ölçek değeri

    private bool isIncreasing = true; // Büyüyüp küçülme yönünü takip eder
    private Vector3 currentScale;

    void Start()
    {
        // Objenin başlangıç ölçeğini kaydediyoruz
        currentScale = transform.localScale;
    }

    void Update()
    {
        // Ölçek değişimi için zamanı kullanarak bir kontrol yapıyoruz
        if (isIncreasing)
        {
            currentScale.x += scaleSpeed * Time.deltaTime;
            currentScale.y += scaleSpeed * Time.deltaTime;

            if (currentScale.x >= maxScale)
            {
                isIncreasing = false;
            }
        }
        else
        {
            currentScale.x -= scaleSpeed * Time.deltaTime;
            currentScale.y -= scaleSpeed * Time.deltaTime;

            if (currentScale.x <= minScale)
            {
                isIncreasing = true;
            }
        }

        // Ölçek değerini objeye uygula
        transform.localScale = currentScale;
    }
}
