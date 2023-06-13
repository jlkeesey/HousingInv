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
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS �AS IS�
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
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using HousingInv.Localization;
using HousingInv.Model.Aetherytes;
using HousingInv.Model.FC;
using HousingInv.Model.Houses;
using HousingInv.Model.Teleports;
using HousingInv.Model.Territories;
using ImGuiScene;

namespace HousingInv.Windows;

/// <summary>
///     Manages the top-level windows of the plugin including binding them with the plugin system.
/// </summary>
public sealed class JlkWindowManager : IDisposable
{
    private readonly ConfigWindow _configWindow;
    private readonly DalamudPluginInterface _pluginInterface;
    private readonly WindowSystem _windowSystem;

    /// <summary>
    ///     Creates the manager, all top-level windows, and binds them where needed.
    /// </summary>
    /// <param name="pluginInterface"></param>
    /// <param name="config"></param>
    /// <param name="territoryManager"></param>
    /// <param name="aetheryteManager"></param>
    /// <param name="teleportLocationManager"></param>
    /// <param name="houseManager"></param>
    /// <param name="nameSpace"></param>
    /// <param name="chatterImage"></param>
    /// <param name="loc"></param>
    /// <param name="freeCompanyManager"></param>
    public JlkWindowManager(DalamudPluginInterface pluginInterface,
                            Configuration config,
                            TerritoryManager territoryManager,
                            AetheryteManager aetheryteManager,
                            TeleportLocationManager teleportLocationManager,
                            FreeCompanyManager freeCompanyManager,
                            HouseManager houseManager,
                            string nameSpace,
                            TextureWrap chatterImage,
                            Loc loc)
    {
        _pluginInterface = pluginInterface;
        _windowSystem = new WindowSystem(nameSpace);
        _configWindow = Add(new ConfigWindow(config,
                                             chatterImage,
                                             territoryManager,
                                             aetheryteManager,
                                             teleportLocationManager,
                                             freeCompanyManager,
                                             houseManager,
                                             loc));
        _pluginInterface.UiBuilder.Draw += _windowSystem.Draw;
        _pluginInterface.UiBuilder.OpenConfigUi += ToggleConfig;
    }

    /// <summary>
    ///     Unbinds from the plugin window system.
    /// </summary>
    public void Dispose()
    {
        _pluginInterface.UiBuilder.OpenConfigUi -= ToggleConfig;
        _pluginInterface.UiBuilder.Draw -= _windowSystem.Draw;

        _windowSystem.RemoveAllWindows();

        _configWindow.Dispose();
    }

    /// <summary>
    ///     Toggles the visibility of the configuration window.
    /// </summary>
    public void ToggleConfig()
    {
        _configWindow.IsOpen = !_configWindow.IsOpen;
    }

    /// <summary>
    ///     Adds the given window to the plugin system window list.
    /// </summary>
    /// <typeparam name="TType">The window type.</typeparam>
    /// <param name="window">The window to add.</param>
    /// <returns>The given window.</returns>
    private TType Add<TType>(TType window) where TType : Window
    {
        _windowSystem.AddWindow(window);
        return window;
    }
}
