/*
 * This file's license: 
 * 
 *  Copyright 2011 Matthew Beardmore
 *
 *  This file is part of Aurora.Addon.FractalTerrain.
 *  Aurora.Addon.FractalTerrain is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *  Aurora.Addon.FractalTerrain is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 *  You should have received a copy of the GNU General Public License along with Aurora.Addon.FractalTerrain. If not, see http://www.gnu.org/licenses/.
 *
 * 
 * LandscapeGenCore license:
 * 
 * Copyright (c) 2006, Bevan Coleman
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of the Author nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Collections.Generic;
using System.Text;
using LandscapeGenCore;
using Aurora.Framework;
using OpenSim.Region.Framework.Interfaces;
using OpenSim.Region.Framework.Scenes;

namespace Aurora.Addon.FractalTerrain
{
    public class FractalTerrain : ISharedRegionModule
    {
        private PerlinNoise m_perlinNoise = new PerlinNoise();
        private KochLikeNoise m_kochLikeNoise = new KochLikeNoise();
        private INoiseGenerator _noiseGen;

        public void Initialise (Nini.Config.IConfigSource source)
        {
            MainConsole.Instance.Commands.AddCommand("generate fractal terrain", "generate fractal terrain",
                "Generates a fractal terrain", Generate);
        }

        public void PostInitialise ()
        {
        }

        public void AddRegion (IScene scene)
        {
        }

        public void RegionLoaded (IScene scene)
        {
        }

        public void RemoveRegion (IScene scene)
        {
        }

        public void Close ()
        {
        }

        public string Name
        {
            get { return "FractalTerrain"; }
        }

        public Type ReplaceableInterface
        {
            get { return null; }
        }

        public void Generate (string[] s)
        {
            if(MainConsole.Instance.ConsoleScene == null)
            {
                MainConsole.Instance.Output("Select a scene first");
                return;
            }
            //string noiseGen = MainConsole.Instance.CmdPrompt("Noise generator (Perlin or Kosh)", "Perlin", new List<string>(new [] { "Perlin", "Kosh" }));
            //if(noiseGen == "Perlin")
            {
                _noiseGen = m_perlinNoise;
                PerlinNoiseSettings pns = new PerlinNoiseSettings();
                pns.ResultX = MainConsole.Instance.ConsoleScene.RegionInfo.RegionSizeX;
                pns.ResultY = MainConsole.Instance.ConsoleScene.RegionInfo.RegionSizeY;
                pns.RandomSeed = int.Parse(MainConsole.Instance.Prompt("Random Seed (0-infinity)", "10"));
                pns.CorsenessX = int.Parse(MainConsole.Instance.Prompt("Corseness (X direction) (2-1000)", "100"));
                pns.CorsenessY = int.Parse(MainConsole.Instance.Prompt("Corseness (Y direction) (2-1000)", "100"));
                pns.FlatEdges = MainConsole.Instance.Prompt("Flat Edges (recommended)", "true", new List<string>(new[] { "true", "false" })) == "true";
                pns.Octaves = int.Parse(MainConsole.Instance.Prompt("Octaves (0-infinity)", "5"));
                pns.Persistence = float.Parse(MainConsole.Instance.Prompt("Persistence", "0.8"));
                _noiseGen.Settings = pns;
            }
            /*else
            {
                _noiseGen = m_kochLikeNoise;
                KochLikeNoiseSettings kns = new KochLikeNoiseSettings();
                kns.ResultX = MainConsole.Instance.Prompt.RegionInfo.RegionSizeX;
                kns.ResultY = MainConsole.Instance.Prompt.RegionInfo.RegionSizeY;
                kns.H = double.Parse(MainConsole.Instance.Prompt("H", "1.0"));
                kns.InitalGridX = int.Parse(MainConsole.Instance.Prompt("Initial Grid X", "2"));
                if(kns.InitalGridX < 2)
                    kns.InitalGridX = 2;
                kns.InitalGridY = int.Parse(MainConsole.Instance.Prompt("Initial Grid Y", "2"));
                if(kns.InitalGridY < 2)
                    kns.InitalGridY = 2;
                kns.RandomMin = int.Parse(MainConsole.Instance.Prompt("Random Min", "-1"));
                kns.RandomMax = int.Parse(MainConsole.Instance.Prompt("Random Max", "1"));
                kns.RandomSeed = int.Parse(MainConsole.Instance.Prompt("Random Seed", "0"));
                kns.Scale = double.Parse(MainConsole.Instance.Prompt("Scale", "1.0"));
                _noiseGen.Settings = kns;
            }*/
            float scaling = float.Parse(MainConsole.Instance.Prompt("Fractal Scaling", "50"));
            float[,] land = _noiseGen.Generate();
            ITerrainChannel c = new TerrainChannel(MainConsole.Instance.ConsoleScene);
            for(int x = 0; x < MainConsole.Instance.ConsoleScene.RegionInfo.RegionSizeX; x++)
            {
                for(int y = 0; y < MainConsole.Instance.ConsoleScene.RegionInfo.RegionSizeY; y++)
                {
                    c[x, y] = (land[x, y] * scaling) + (float)MainConsole.Instance.ConsoleScene.RegionInfo.RegionSettings.WaterHeight + 10;
                }
            }
            MainConsole.Instance.ConsoleScene.RequestModuleInterface<ITerrainModule>().TerrainMap = c;
            MainConsole.Instance.ConsoleScene.RequestModuleInterface<ITerrainModule>().TaintTerrain();
            MainConsole.Instance.ConsoleScene.RegisterModuleInterface<ITerrainChannel>(c);
        }
    }
}
