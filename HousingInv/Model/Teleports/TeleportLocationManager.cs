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

using System.Collections.Generic;
using System.Linq;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using HousingInv.Model.Aetherytes;
using HousingInv.Model.Territories;

namespace HousingInv.Model.Teleports;

public class TeleportLocationManager
{
    private readonly AetheryteManager _aetheryteManager;
    private readonly TerritoryManager _territoryManager;

    public TeleportLocationManager(AetheryteManager aetheryteManager, TerritoryManager territoryManager)
    {
        _aetheryteManager = aetheryteManager;
        _territoryManager = territoryManager;
    }

    public unsafe IEnumerable<TeleportLocation> GetTeleportLocations()
    {
        var results = new List<TeleportLocation>();
        var instance = Telepo.Instance();
        instance->UpdateAetheryteList();
        var list = instance->TeleportList;
        for (ulong i = 0; i < list.Size(); i++)
        {
            var teleportInfo = list.Get(i);
            var aetheryte = _aetheryteManager.Get(teleportInfo.AetheryteId);
            var territory = _territoryManager.Get(teleportInfo.TerritoryId);

            switch (aetheryte.Name)
            {
                case "Estate Hall (Free Company)":
                    results.Add(new FreeCompanyLocation(aetheryte, territory));
                    break;
                case "Estate Hall (Private)" when teleportInfo.IsSharedHouse:
                    results.Add(new SharedHouseLocation(aetheryte, territory, teleportInfo.Ward, teleportInfo.Plot));
                    break;
                case "Estate Hall (Private)" when teleportInfo.IsAppartment:
                    results.Add(new ApartmentLocation(aetheryte, territory, -1));
                    break;
                case "Estate Hall (Private)":
                    results.Add(new PrivateHouseLocation(aetheryte, territory));
                    break;
                default:
                    results.Add(new AetheryteLocation(aetheryte, territory));
                    break;
            }
        }

        return results;
    }

    public IEnumerable<TeleportLocation> GetHouses()
    {
        return GetTeleportLocations().Where(t => t is not AetheryteLocation);
    }

#if DEBUG
    public void ListAll()
    {
        //var list = GetTeleportLocations();
        var list = GetHouses();
        foreach (var teleportLocation in list) PluginLog.Log($"{teleportLocation}");
    }
#endif
}
