﻿// Copyright 2023 James Keesey
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

using Dalamud.Game.ClientState;
using HousingInv.Model.Servers;

namespace HousingInv.Model.Players;

/// <summary>
///     Information about the player running this plugin.
/// </summary>
public class Myself : IMyself
{
    private readonly ClientState _clientState;
    private readonly ServerManager _serverManager;

    private Server? _homeWorld;
    private string? _name;

    public Myself(ClientState clientState, ServerManager serverManager)
    {
        _clientState = clientState;
        _serverManager = serverManager;
        
    }

    public uint ObjectId => _clientState.LocalPlayer?.ObjectId ?? 0;

    /// <summary>
    ///     The player character's name.
    /// </summary>
    public string Name
    {
        get { return _name ??= _clientState.LocalPlayer?.Name.TextValue ?? "Who am I?"; }
    }

    /// <summary>
    ///     The player character's home world.
    /// </summary>
    public Server HomeServer
    {
        get
        {
            return _homeWorld ??= _serverManager.GetWorld(_clientState.LocalPlayer?.HomeWorld.GameData?.Name.ToString());
        }
    }

    /// <summary>
    ///     Returns my full name (name plus home world).
    /// </summary>
    public string FullName => $"{Name}@{HomeServer.Name}";
}
