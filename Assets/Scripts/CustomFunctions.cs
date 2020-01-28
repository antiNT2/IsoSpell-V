﻿using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomFunctions : MonoBehaviour
{
    static GameObject soundHolder;
    public static CustomFunctions instance;

    private void Awake()
    {
        instance = this;
    }

    public static void PlaySound(AudioClip soundToPlay, float volume = 0.5f)
    {
        if (soundHolder == null)
        {
            soundHolder = GameObject.Instantiate(new GameObject());
            soundHolder.name = "Sound Holder";
        }

        AudioSource audio;
        audio = soundHolder.AddComponent<AudioSource>();
        audio.volume = volume;
        audio.clip = soundToPlay;
        audio.Play();
        Destroy(audio, soundToPlay.length + 0.2f);
    }

    public static void HitPause()
    {
        if (Time.timeScale == 1f)
            instance.StartCoroutine(instance.HitPauseEffect());
    }

    public IEnumerator HitPauseEffect()
    {
        CameraShaker.Instance.ShakeOnce(2f, 10f, 0.1f, 0.5f);
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(0.1f);
        if (Time.timeScale != 0)
            Time.timeScale = 1f;

    }

    public static Color PlayerIdToColor(int playerId)
    {
        if (playerId == 0)
            return new Color(1, 0.15f, 0.15f, 1);
        else if (playerId == 1)
            return new Color(0.15f, 0.58f, 1f, 1);
        else if (playerId == 2)
            return new Color(0.15f, 1f, 0.25f, 1);
        else
            return new Color(0.94f, 1f, 0.15f, 1);
    }

    public static Color DarkColor(Color input)
    {
        float H, S, V;
        Color.RGBToHSV(input, out H, out S, out V);
        V = Mathf.Clamp(V - 0.2f, 0f, 1f);

        return Color.HSVToRGB(H, S, V);
    }

    public static Vector2 GetAmmoSpawnPos(GameObject _player)
    {
        float aimAngle = _player.GetComponent<PlayerController>().aimAngle;
        var x = _player.transform.position.x + 0.015f * Mathf.Cos(aimAngle);
        var y = _player.transform.position.y + 0.015f * Mathf.Sin(aimAngle);

        var crossHairPosition = new Vector3(x, y, 0);

        return crossHairPosition;
    }
}