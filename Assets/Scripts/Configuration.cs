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

[System.Serializable]
public class Configuration
{
    public int CarCount { get; set; }
    public int SelectionMethod { get; set; }
    public int MutationChance { get; set; }
    public float MutationRate { get; set; }
    public int LayersCount { get; set; }
    public int NeuronPerLayerCount { get; set; }
    public int TrackNumber { get; set; }
    public bool Navigator { get; set; }
    public bool StopConditionActive { get; set; }
    public int StopGenerationNumber { get; set; }
    public bool DemoMode { get; set; }
    public bool IsPopulated { get; set; }
}
