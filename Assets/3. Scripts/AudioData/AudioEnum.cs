using UnityEngine;
using System.Collections.Generic;
using System;
#region AudioDatas
[Serializable]
public class SceneBgmData // �� Scene�� �´� BGM
{
    public string sceneName;
    public AudioClip bgmClip;
}

[Serializable]
public class SFXData // SFX �����͵��� �����Ͽ� �����ϴ� �ʿ��� ��� �����
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

#region SFXVolumeData // SFX�� ������ �����ϱ� ���� Ŭ����
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