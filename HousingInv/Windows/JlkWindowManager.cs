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
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin;
using Ninject;

// ReSharper disable ClassNeverInstantiated.Global

namespace HousingInv.Windows;

/// <summary>
///     Manages the top-level windows of the plugin including binding them with the plugin system.
/// </summary>
public sealed class JlkWindowManager : IDisposable
{
    private readonly ConfigWindow _configWindow;
    private readonly DalamudPluginInterface _pluginInterface;
    private readonly WindowSystem _windowSystem;
    private readonly FileDialogManager _fileDialogManager;

    /// <summary>
    ///     Creates the manager, all top-level windows, and binds them where needed.
    /// </summary>
    /// <param name="pluginInterface"></param>
    /// <param name="configWindow"></param>
    /// <param name="fileDialogManager"></param>
    /// <param name="nameSpace"></param>
    public JlkWindowManager(DalamudPluginInterface pluginInterface,
                            ConfigWindow configWindow,
                            FileDialogManager fileDialogManager,
                            [Named("nameSpace")] string nameSpace)
    {
        _pluginInterface = pluginInterface;
        _windowSystem = new WindowSystem(nameSpace);
        _configWindow = Add(configWindow);
        _fileDialogManager = fileDialogManager;
        _pluginInterface.UiBuilder.Draw += _windowSystem.Draw;
        _pluginInterface.UiBuilder.Draw += _fileDialogManager.Draw;
        _pluginInterface.UiBuilder.OpenConfigUi += ToggleConfig;
    }

    /// <summary>
    ///     Unbinds from the plugin window system.
    /// </summary>
    public void Dispose()
    {
        // PluginLog.Log("@@@@ Disposing Window Manager");
        _pluginInterface.UiBuilder.OpenConfigUi -= ToggleConfig;
        _pluginInterface.UiBuilder.Draw -= _fileDialogManager.Draw;
        _pluginInterface.UiBuilder.Draw -= _windowSystem.Draw;

        _windowSystem.RemoveAllWindows();
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
