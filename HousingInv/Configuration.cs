// Copyright 2023 James Keesey
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS “AS IS”
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Dalamud.Configuration;
using Dalamud.Game.Text;
using Dalamud.Plugin;
using NodaTime;

namespace HousingInv;

/// <summary>
///     Contains all of the user configuration settings.
/// </summary>
[Serializable]
public partial class Configuration : IPluginConfiguration
{
    public string Temp = string.Empty;

#if DEBUG
    public bool IsDebug = true;
#else
    public bool IsDebug = false;
#endif

    public int Version { get; set; } = 1;


    /// <summary>
    ///     Saves the current state of the configuration.
    /// </summary>
    public void Save()
    {
        _pluginInterface?.SavePluginConfig(this);
    }

    /// <summary>
    ///     Loads the most recently saved configuration or creates a new one.
    /// </summary>
    /// <returns>The configuration to use.</returns>
    public static Configuration Load(DalamudPluginInterface pluginInterface)
    {
        // var config = new Configuration();
        // pluginInterface.SavePluginConfig(config);
        if (pluginInterface.GetPluginConfig() is not Configuration config) config = new Configuration();
        config._pluginInterface = pluginInterface;

#if DEBUG
        config.InitializeForDebug();
#endif
        config.Initialize();

        config.Save();
        return config;
    }

    public void Initialize()
    {
    }

#if DEBUG
    public void InitializeForDebug()
    {
        // ReSharper disable StringLiteralTypo
        // ReSharper restore StringLiteralTypo
    }
#endif

    [JsonIgnore] private DalamudPluginInterface? _pluginInterface;
}
