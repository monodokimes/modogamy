﻿using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class DogFactory : MonoBehaviour
{
    [SerializeField]
    private Transform _spawnTargetTransform;
    [SerializeField]
    private GameObject _dog;
    [SerializeField]
    private GameObject[] _dogGraphics;
    [SerializeField]
    private int _minSize = 10;
    [SerializeField]
    private int _maxSize = 50;
    [SerializeField]
    private double _sizeLambda = 25;
    [SerializeField]
    private float _dogScale = 0.05f;

    [SerializeField]
    private TextAsset _nameFile;
    private string[] _nameList;
    private int _nameCount;

    [SerializeField]
    private TextAsset _likeFile;
    private string[] _likeList;
    private int _likeCount;

    public GameObject[] DogGraphics { get { return _dogGraphics; } }

    void Awake()
    {
        _nameList = _nameFile.text.Split('\n');
        _nameCount = _nameList.Length;

        _likeList = _likeFile.text.Split('\n');
        _likeCount = _likeList.Length;
    }

    void Start()
    {
        //StartCoroutine(GenerateDogs());
    }

    // Temporary until we can generate dogs properly
    private IEnumerator GenerateDogs()
    {
        var delay = 6;
        var spawnOffset = 20;

        while (true)
        {
            var dog = GetNewDog();
            var offset = new Vector3
            {
                x = _spawnTargetTransform.position.x + (UnityEngine.Random.value * 2 - 1) * spawnOffset
            };

            dog.transform.Translate(offset);
            yield return new WaitForSeconds(delay);
        }
    }

    public Dog GetNewDog()
    {
        return SpawnDog(GetNewDogProfile());
    }

    public Dog SpawnDog(DogProfile profile)
    {
        var dog = Instantiate(_dog)
            .GetComponent<Dog>();

        dog.Profile = profile;

        var graphics = Instantiate(_dogGraphics[profile.Index], dog.graphics.transform);
        graphics.transform.localScale = Vector3.one * profile.Size;

        return dog;
    }

    public DogProfile GetNewDogProfile()
    {
        var profile = new DogProfile
        {
            Name = GetRandomName(),
            Size = GetRandomSize(),
            Like = GetRandomLike(),
            Index = UnityEngine.Random.Range(0, _dogGraphics.Length)
        };
        print(profile.Size);
        return profile;
    }

    private float GetRandomSize()
    {
        // first, do a rough poisson calculation
        double p = 1.0, L = Math.Exp(-_sizeLambda);
        int k = 0;
        do
        {
            k++;
            p *= UnityEngine.Random.value;
        }
        while (p > L);
        k--;
        // now we've got our poisson, let's clamp it
        return Mathf.Clamp((float)k, _minSize, _maxSize) * _dogScale;
    }

    private string GetRandomName()
    {
        string name = _nameList[UnityEngine.Random.Range(0, _nameCount)];
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower().Trim());
    }

    private string GetRandomLike()
    {
        string like = _likeList[UnityEngine.Random.Range(0, _likeCount)];
        return like.ToLower().Trim();
    }
}
