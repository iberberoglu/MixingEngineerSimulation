using UnityEngine;
using FMODUnity;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Songs", menuName = "Songs")]
public class Songs : ScriptableObject
{
    public EventReference channel1Event;
    public EventReference channel2Event;
    public EventReference channel3Event;
    public EventReference channel4Event;
    public List<string> channelNames = new List<string>();
    public int songIndex; // Şarkı indexi
    public float channelVolumeStart; // Kanal başlangıç sesi
    public string songName; // Şarkı adı
}
