using UnityEngine;
using System.Collections.Generic;
using System;
#region AudioDatas
[Serializable]
public class SceneBgmData // 각 Scene에 맞는 BGM
{
    public string sceneName;
    public AudioClip bgmClip;
}

[Serializable]
public class SFXData // SFX 데이터들을 저장하여 관리하다 필요한 경우 사용함
{
    public string sfxName;
    public AudioClip sfxClip;
}
#endregion

#region
public enum SFXCategory
{
    UI,
    Dog,
    Object
}
#endregion

#region SFXVolumeData // SFX의 볼륨을 관리하기 위한 클래스
[Serializable]
public class SFXVolumeData
{
    public List<CategoryVolumeData> categoryVolumes;
}
[Serializable]
public class CategoryVolumeData
{
    public string categoryName;
    public float categoryVolume;
}
#endregion