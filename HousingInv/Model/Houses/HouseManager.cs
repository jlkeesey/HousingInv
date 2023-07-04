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

using System.Linq;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.Housing;
using HousingInv.Model.Location;
using HousingInv.Model.Teleports;
using HousingInv.Model.Territories;

namespace HousingInv.Model.Houses;

public unsafe class HouseManager : IHouseManager
{
    private readonly HousingManager* _housingManager;
    private readonly Locator _locator;
    private readonly TeleportLocationManager _teleportLocationManager;

    public HouseManager(Locator locator, TeleportLocationManager teleportLocationManager)
    {
        _locator = locator;
        _teleportLocationManager = teleportLocationManager;
        _housingManager = HousingManager.Instance();
    }

    public bool HasPermissions => _housingManager->HasHousePermissions();

    public bool IsInside => _housingManager->IsInside();

    public int Ward =>
        _housingManager->GetCurrentWard() >= 0
            ? _housingManager->GetCurrentWard() + 1
            : _housingManager->GetCurrentWard();

    public int Plot =>
        _housingManager->GetCurrentPlot() >= 0
            ? _housingManager->GetCurrentPlot() + 1
            : _housingManager->GetCurrentPlot();

    public int Room => _housingManager->GetCurrentRoom();

    public HouseType WhichHouse()
    {
        var territory = _locator.CurrentTerritory;
        if (territory.TerritoryUse != TerritoryUse.ResidentialZone && territory.TerritoryUse != TerritoryUse.Residence)
            return HouseType.Unknown;
        if (Plot is -128 or -127) return HouseType.MyApartment;
        var shares = _teleportLocationManager.GetSharedHouses().ToArray();
        switch (shares.Length)
        {
            case > 0 when Matches(shares[0], territory, Ward, Plot):
                return HouseType.Shared1;
            case > 1 when Matches(shares[1], territory, Ward, Plot):
                return HouseType.Shared2;
        }

        var freeCompany = _teleportLocationManager.GetFreeCompany();
        if (freeCompany != null)
            if (freeCompany.Territory == territory && freeCompany.Ward == Ward && freeCompany.Plot == Plot)
                return HouseType.MyFc;

        var privateHouse = _teleportLocationManager.GetPrivateHouse();
        if (privateHouse != null)
            if (privateHouse.Territory == territory && privateHouse.Ward == Ward && privateHouse.Plot == Plot)
                return HouseType.MyHouse;

        return HouseType.Unknown;
    }

#if DEBUG
    public void LogHere()
    {
        PluginLog.Log($"@@@@ terr:{_locator.CurrentTerritory} ward:{Ward}  plot:{Plot}  room:{Room}  inside:{IsInside}  perms:{HasPermissions}");
        PluginLog.Log($"@@@@ which: {WhichHouse()}");
    }
#endif

    private static bool Matches(SharedHouseLocation share, Territory territory, int ward, int plot)
    {
        return Territory.MatchResidential(share.Territory, territory) && share.Ward == ward && share.Plot == plot;
    }

    // private static int GetZeroTermLength(byte* bytes, int length)
    // {
    //     for (var i = 0; i < length; i++)
    //         if (bytes[i] == 0)
    //             return i;
    //
    //     return length;
    // }
    //
    // private static string ReadAsString(byte* bytes, int length)
    // {
    //     var count = GetZeroTermLength(bytes, length);
    //     var arr = new byte[count];
    //     Marshal.Copy((IntPtr) bytes, arr, 0, count);
    //     return Encoding.UTF8.GetString(arr, 0, count);
    // }
}
