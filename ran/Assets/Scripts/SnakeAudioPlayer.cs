using UnityEngine;

public class SnakeAudioPlayer
{
    private readonly AudioSource nguonAmThanh;

    public SnakeAudioPlayer(Transform nhomAmThanh)
    {
        nguonAmThanh = nhomAmThanh.GetComponent<AudioSource>();
        if (nguonAmThanh == null)
            nguonAmThanh = nhomAmThanh.gameObject.AddComponent<AudioSource>();

        nguonAmThanh.playOnAwake = false;
        nguonAmThanh.loop = false;
        nguonAmThanh.spatialBlend = 0f;
        nguonAmThanh.volume = 1f;
    }

    public void PhatAmThanh(AudioClip clip, float amLuong)
    {
        if (nguonAmThanh == null || clip == null)
            return;

        nguonAmThanh.PlayOneShot(clip, Mathf.Clamp01(amLuong));
    }
}
