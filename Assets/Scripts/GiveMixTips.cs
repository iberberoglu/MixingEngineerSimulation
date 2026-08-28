using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveMixTips : MonoBehaviour
{
    [SerializeField] List<GameObject> toleranceAreaTops;
    [SerializeField] List<GameObject> toleranceAreaBottoms;
    public int howMuchTipsToGive = 30;
    
    private void Start() {
        SetMixTipsActive(false);
    }
    public void SetToleranceArea(float tolerance, float ch1IdealLevel, float ch2IdealLevel, float ch3IdealLevel, float ch4IdealLevel)
    {
        List<float> idealLevels = new List<float> {ch1IdealLevel, ch2IdealLevel, ch3IdealLevel, ch4IdealLevel};
        for(int i = 0; i < 4; i++)
        {
            // Generate a random offset for the tolerance
            float randomOffset = Random.Range(-tolerance / 2, tolerance / 2);

            toleranceAreaTops[i].transform.localPosition = new Vector3(
                toleranceAreaTops[i].transform.localPosition.x,
                MapValueUsingMathf(tolerance + idealLevels[i] + randomOffset, true),
                toleranceAreaTops[i].transform.localPosition.z
            );

            toleranceAreaBottoms[i].transform.localPosition = new Vector3(
                toleranceAreaBottoms[i].transform.localPosition.x,
                MapValueUsingMathf(idealLevels[i] - tolerance + randomOffset, false),
                toleranceAreaBottoms[i].transform.localPosition.z
            );

        }
    }
    
    float MapValueUsingMathf(float value, bool isToleranceUpper)
    {
        // Önce değeri 0 ile 1 arasına ölçekle
        float t = Mathf.InverseLerp(0, 100, value);
        // Ardından yeni aralığa eşle
        if(isToleranceUpper)
            return Mathf.Lerp(-150, 150, t) + howMuchTipsToGive;
        else
            return Mathf.Lerp(-150, 150, t) - howMuchTipsToGive;
    }
    
    public void SetMixTipsActive(bool isActive)
    {
        foreach (var item in toleranceAreaTops)
        {
            item.SetActive(isActive);
        }
        foreach (var item in toleranceAreaBottoms)
        {
            item.SetActive(isActive);
        }
    }
    
}
