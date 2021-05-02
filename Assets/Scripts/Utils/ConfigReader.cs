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

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class ConfigReader
{
    public static List<LocalizationItem> ReadLocalizationData(string filePath)
    {
        List<LocalizationItem> localizationItems = new List<LocalizationItem>();
        if (File.Exists(filePath))
        {
            string[] localizationLines = File.ReadAllLines(filePath);

            for (int i = 0; i < localizationLines.Length; i++)
            {
                // The config file can contains comments which are indicated by a starting # character
                if (localizationLines[i].Length > 2 && localizationLines[i][0] != '#')
                {
                    // Matches to the first word at the beginning of a line 
                    // (containing only letters and underscore)
                    string readKey = Regex.Match(localizationLines[i], @"^([\w]+)").Value;

                    // Matches to a text between double quotes (second capturing group)
                    // Note: The first and third capturing group matches to the double quote itself!
                    string readValue = Regex.Match(localizationLines[i], "\"(.*)\"").Groups[1].Value;

                    readValue = Regex.Unescape(readValue);

                    if (!string.IsNullOrEmpty(readKey) && !string.IsNullOrEmpty(readValue))
                    {
                        localizationItems.Add(new LocalizationItem
                        {
                            Key = readKey,
                            Value = readValue
                        });
                    }
                    else
                    {
                        Debug.LogError($"Cannot interpret the following line: {localizationLines[i]}");
                    }
                }
            }
        }
        else
        {
            Debug.LogError($"Cannot find the language file {filePath}!");
        }

        return localizationItems;
    }
}
