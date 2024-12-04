using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "Songs", menuName = "Songs")]
public class Songs : ScriptableObject
{
    public EventReference channel1Event;
    public EventReference channel2Event;
    public EventReference channel3Event;
    public EventReference channel4Event;
    public string songName; // Şarkı adı
}
