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

using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.Housing;

namespace HousingInv.Model.Houses;

public unsafe class HouseManager
{
    private readonly HousingManager* _housingManager;

    public HouseManager()
    {
        _housingManager = HousingManager.Instance();
    }

    public bool HasPermissions => _housingManager->HasHousePermissions();

    public bool IsInside => _housingManager->IsInside();

    public int Ward => _housingManager->GetCurrentWard();

    public int Plot => _housingManager->GetCurrentPlot();

    public int Room => _housingManager->GetCurrentRoom();

#if DEBUG
    public void LogHere()
    {
        PluginLog.Log($"@@@@ ward:{Ward}  plot:{Plot}  room:{Room}  inside:{IsInside}  perms:{HasPermissions}");
    }
#endif
}

public abstract class Residence
{
    public bool IsApartment { get; } = false;
}
