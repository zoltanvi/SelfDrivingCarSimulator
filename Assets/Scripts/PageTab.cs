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

using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PageTab : MonoBehaviour
{

    private TextMeshProUGUI m_TextMesh;
    private int m_PageNumber;
    private static readonly Color DeactivatedColor = new Color(0.2f, 0.3f, 0.4f, 1f);
    private static readonly Color ActivatedColor = new Color(0.07f, 0.12f, 0.4f, 1f);
    public event EventHandler ClickedEvent;

    public int PageNumber
    {
        get
        {
            return m_PageNumber;
        }
        set
        {
            m_TextMesh.text = value.ToString();
            m_PageNumber = value;
        }
    }

    private void Awake()
    {
        m_TextMesh = GetComponentInChildren<TextMeshProUGUI>();
        if (m_TextMesh == null)
        {
            Debug.LogError("PageTab.TextMesh not found!");
        }
    }

    public void OnClicked()
    {
        ClickedEvent?.Invoke(this, new PageTabEventArgs { PageNumber = m_PageNumber });
    }

    public void ActivateTab(bool activate)
    {
        if (activate)
        {
            GetComponentInChildren<Image>().color = ActivatedColor;
        }
        else
        {
            GetComponentInChildren<Image>().color = DeactivatedColor;
        }
    }
}

public class PageTabEventArgs : EventArgs
{
    public int PageNumber { get; set; }
}