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
using System.Runtime.InteropServices;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using HousingInv.Model.Aetherytes;
using HousingInv.Model.Territories;

namespace HousingInv.Model.Teleports;

public class TeleportLocationManager : ITeleportLocationManager
{
    private readonly AetheryteManager _aetheryteManager;
    private readonly List<TeleportLocation> _cache = new();
    private readonly TerritoryManager _territoryManager;

    public TeleportLocationManager(AetheryteManager aetheryteManager, TerritoryManager territoryManager)
    {
        _aetheryteManager = aetheryteManager;
        _territoryManager = territoryManager;
    }

    public IEnumerable<TeleportLocation> GetTeleportLocations()
    {
        if (_cache.Count == 0) FillCache();
        return _cache;
    }

    public IEnumerable<TeleportLocation> GetHouses()
    {
        return GetTeleportLocations().Where(t => t is not AetheryteLocation);
    }

    public IEnumerable<SharedHouseLocation> GetSharedHouses()
    {
        return GetTeleportLocations()
              .OfType<SharedHouseLocation>()
              .OrderBy(t => t.Aetheryte.Order)
              .ThenBy(t => t.Ward)
              .ThenBy(t => t.Plot);
    }

    public void ClearCache()
    {
        _cache.Clear();
    }

    private unsafe void FillCache()
    {
        var instance = Telepo.Instance();
        instance->UpdateAetheryteList();
        var list = instance->TeleportList;
        for (ulong i = 0; i < list.Size(); i++)
        {
            var baseTeleportInfo = list.Get(i);
            var teleportInfo = (MyTeleportInfo*) (byte*) &baseTeleportInfo;
            var aetheryte = _aetheryteManager.Get(teleportInfo->AetheryteId);
            var territory = _territoryManager.Get(teleportInfo->TerritoryId);

            switch (aetheryte.Name)
            {
                case "Estate Hall (Free Company)":
                    PrintInfo("Fc", teleportInfo);
                    _cache.Add(new FreeCompanyLocation(aetheryte, territory, teleportInfo->Ward, teleportInfo->Plot));
                    break;
                case "Estate Hall (Private)" when teleportInfo->IsSharedHouse:
                    PrintInfo("Shared", teleportInfo);
                    Search((byte*) teleportInfo, 32);
                    _cache.Add(new SharedHouseLocation(aetheryte, territory, teleportInfo->Ward, teleportInfo->Plot));
                    break;
                case "Estate Hall (Private)" when teleportInfo->IsApartment:
                    PrintInfo("Apart", teleportInfo);
                    _cache.Add(new ApartmentLocation(aetheryte, territory, -1));
                    break;
                case "Estate Hall (Private)":
                    PrintInfo("House", teleportInfo);
                    _cache.Add(new PrivateHouseLocation(aetheryte, territory, teleportInfo->Ward, teleportInfo->Plot));
                    break;
                default:
                    _cache.Add(new AetheryteLocation(aetheryte, territory));
                    break;
            }
        }
    }

    private unsafe void PrintInfo(string name, MyTeleportInfo* teleportInfo)
    {
        PluginLog.Log($"@@@@ {name,-6}: Ward:   {teleportInfo->Ward}  Plot:   {teleportInfo->Plot}");
    }

    public FreeCompanyLocation? GetFreeCompany()
    {
        return GetTeleportLocations().OfType<FreeCompanyLocation>().FirstOrDefault((FreeCompanyLocation?) null);
    }

    public PrivateHouseLocation? GetPrivateHouse()
    {
        return GetTeleportLocations().OfType<PrivateHouseLocation>().FirstOrDefault((PrivateHouseLocation?) null);
    }

    [StructLayout(LayoutKind.Explicit, Size = 32)]
    private readonly struct MyTeleportInfo
    {
        [FieldOffset(0)] public readonly uint AetheryteId;
        [FieldOffset(4)] public readonly uint GilCost;
        [FieldOffset(8)] public readonly ushort TerritoryId;
        [FieldOffset(16)] private readonly byte _plot;
        [FieldOffset(18)] private readonly byte _ward;
        [FieldOffset(24)] public readonly byte SharedWard; // This is only valid for shared houses
        [FieldOffset(25)] public readonly byte SharedPlot; // This is only valid for shared houses
        [FieldOffset(26)] public readonly byte SubIndex;
        [FieldOffset(27)] public readonly byte IsFavourite;

        public bool IsSharedHouse => SharedWard != 0 && SharedPlot != 0;

        public bool IsApartment => SubIndex == 128 && !IsSharedHouse;

        /// <summary>
        ///     Returns the ward for this location. This is only valid if the <see cref="TerritoryId" /> references a residential
        ///     area.
        /// </summary>
        public int Ward => _ward + 1;

        /// <summary>
        ///     Returns the plot for this location. This is only valid if the <see cref="TerritoryId" /> references a residential
        ///     area.
        /// </summary>
        public int Plot => _plot + 1;
    }

#if DEBUG
    public void ListAll()
    {
        //var list = GetTeleportLocations();
        var list = GetHouses();
        foreach (var teleportLocation in list) PluginLog.Log($"{teleportLocation}");
    }

    private unsafe void Search(byte* ptr, int length)
    {
        const ushort room1 = 33;
        const ushort room = 34;
        // const ushort ward = 2;
        // const ushort plot = 41;
        for (var i = 0; i < length; i++)
        {
            if (ptr[i] == room) PluginLog.Log($"@@@@ {room} at 0x{i:X} ({i})");
            if (ptr[i] == room1) PluginLog.Log($"@@@@ {room1} at 0x{i:X} ({i})");
            // if (ptr[i] == ward) PluginLog.Log($"@@@@ {ward} at 0x{i:X} ({i})");
            //
            // if (ptr[i] == plot) PluginLog.Log($"@@@@ {plot} at 0x{i:X} ({i})");
        }
    }
#endif
}
