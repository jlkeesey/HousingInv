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
using Dalamud.Game.ClientState;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace HousingInv.Model.Territories;

internal unsafe class TerritoryTracker : IDisposable
{
    private readonly ClientState _clientState;
    private readonly GameMain* _gameMain;
    private readonly TerritoryManager _territoryManager;

    public TerritoryTracker(ClientState clientState, TerritoryManager territoryManager)
    {
        _clientState = clientState;
        _territoryManager = territoryManager;

        _gameMain = GameMain.Instance();

        // UpdateCurrentTerritory(unchecked((ushort)_gameMain->CurrentTerritoryTypeId));
        //
        // _clientState.TerritoryChanged += ClientStateOnTerritoryChanged;
    }

    public Territory CurrentTerritory { get; private set; } = Territory.Empty;

    public void Dispose()
    {
        // _clientState.TerritoryChanged -= ClientStateOnTerritoryChanged;
    }

    private void ClientStateOnTerritoryChanged(object? sender, ushort id)
    {
        UpdateCurrentTerritory(id);
    }

    private void UpdateCurrentTerritory(ushort id)
    {
        // var gTerritory = _gameMain->CurrentTerritoryTypeId;
        // PluginLog.Log($"@@@@ id:{id}  gameId:{gTerritory}");
        // CurrentTerritory = _territoryManager[id];
        // if (CurrentTerritory == Territory.Empty)
        //     PluginLog.Log($"@@@@ {id}: Cannot identify current Territory");
        // else
        //     PluginLog.Log($"@@@@ {id}: now in {CurrentTerritory}");
    }
}
