/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{

    public static GameAssets instance;

    private void Awake()
    {
        instance = this;
    }
    private float width, height;
    public GameObject soundObject;
    public Sprite snakeHeadSprite;
    public Sprite snakeBodySprite;
    public Sprite foodSprite;
    public SoundAudioClip[] soundAudioClipArray;
    [Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
    public GameObject GetSoundObject()
    {
        return soundObject;
    }
    public void SetScreenSize(float w, float h)
    {
        this.width = w;
        this.height = h;
    }
    public float GetScreenWidth()
    {
        return width;
    }
    public float GetScreenHeight()
    {
        return height;
    }
}
