/*
Copyright (C) 2021 zoltanvi

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance = null;

    public AudioClip HoverClip;
    public AudioClip ClickClip;
    public AudioClip ClickBackClip;

    public AudioSource AudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        AudioSource.playOnAwake = false;
    }

    public void HoverButton()
    {
        AudioSource.PlayOneShot(HoverClip);
    }

    public void ClickButton()
    {
        AudioSource.PlayOneShot(ClickClip);
    }

    public void ClickBackButton()
    {
        AudioSource.PlayOneShot(ClickBackClip);
    }
}

