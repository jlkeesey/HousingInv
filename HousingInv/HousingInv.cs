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

using System.Reflection;
using Dalamud.Data;
using Dalamud.Game.ClientState;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game.Housing;
using HousingInv.Localization;
using HousingInv.Model.Aetherytes;
using HousingInv.Model.FC;
using HousingInv.Model.Houses;
using HousingInv.Model.Players;
using HousingInv.Model.Servers;
using HousingInv.Model.Teleports;
using HousingInv.Model.Territories;
using HousingInv.Properties;
using HousingInv.System;
using HousingInv.Windows;
using ImGuiScene;
using NotImplementedException = System.NotImplementedException;

namespace HousingInv;

/// <summary>
///     The entry point for this plugin.
/// </summary>
public sealed partial class HousingInv : IDalamudPlugin
{
    public static string Version = string.Empty;

    private readonly CommandManager _commandManager;
    private readonly ClientState _clientState;
    private readonly Configuration _configuration;
    private readonly ILogger _logger;

    private readonly TextureWrap _pluginIcon;
    private readonly JlkWindowManager _windowManager;
    private readonly TerritoryManager _territoryManager;

    public HousingInv([RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
                      [RequiredVersion("1.0")] ChatGui chatGui,
                      [RequiredVersion("1.0")] ClientState clientState,
                      [RequiredVersion("1.0")] CommandManager commandManager,
                      [RequiredVersion("1.0")] DataManager dataManager)
    {
        _commandManager = commandManager;
        _clientState = clientState;

        try
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "";
            _logger = new Logger();

#if DEBUG
            // SeSpecialCharacters.InitializeDebug(_logger);
#endif
            var loc = new Loc();
            loc.Load(_logger);

            _configuration = Configuration.Load(pluginInterface);

            var dateManager = new DateHelper() as IDateHelper;
            var worldManager = new ServerManager(dataManager);
            _territoryManager = new TerritoryManager(dataManager);
            var aetheryteManager = new AetheryteManager(dataManager, _territoryManager);
            var teleportLocationManager = new TeleportLocationManager(aetheryteManager, _territoryManager);
            var freeCompanyManager = new FreeCompanyManager();
            var houseManager = new HouseManager();
            var myself = new Myself(clientState, worldManager);

            _pluginIcon = pluginInterface.UiBuilder.LoadImage(Resources.Icon);
            _windowManager = new JlkWindowManager(pluginInterface,
                                                  _configuration,
                                                  _territoryManager,
                                                  aetheryteManager,
                                                  teleportLocationManager,
                                                  freeCompanyManager,
                                                  houseManager,
                                                  Name,
                                                  _pluginIcon,
                                                  loc);
            RegisterCommands();

            _clientState.TerritoryChanged += ClientStateOnTerritoryChanged;

            var list = teleportLocationManager.GetTeleportLocations();
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    private void ClientStateOnTerritoryChanged(object? sender, ushort id)
    {
        var territory = _territoryManager[id];
        if (territory == Territory.Empty)
        {
            PluginLog.Log($"@@@@ {id}: Cannot identify current Territory");
        }
        else
        {
            PluginLog.Log($"@@@@ {id}: now in {territory}");
        }
    }

    // public static unsafe void ShowItems()
    // {
    // var aetheryteSheet = _dataManager.Excel.GetSheet<Aetheryte>();
    // var terriSheet = _dataManager.Excel.GetSheet<TerritoryType>();
    // var tele = Telepo.Instance();
    // var list = tele->TeleportList;
    // for (ulong j = 0; j < list.Size(); j++)
    // {
    //     var l = list.Get(j);
    //     var aether = aetheryteSheet?.GetRow(l.AetheryteId);
    //     if (aether.Order != 0) continue;
    //     var aname = aether.AethernetName.Value.Name;
    //     var aname2 = aether.PlaceName.Value?.Name;
    //     var terr = terriSheet.GetRow(l.TerritoryId);
    //     var tname = terr.PlaceName.Value?.Name;
    //     PluginLog.Log($"@@@@ terr:{tname,-30} name2:{aname2,-30} ward:{l.Ward}  plot:{l.Plot}");
    // }
    //
    //
    // HousingManager mmm;
    //
    // var mgr = HousingManager.Instance();
    // PluginLog.Log($"@@@@ div:{mgr->GetCurrentDivision()}  ward:{mgr->GetCurrentWard()}  plot:{mgr->GetCurrentPlot()}  id:{mgr->GetCurrentHouseId()} in:{mgr->IsInside()}  room:{mgr->GetCurrentRoom()}  {mgr->HasHousePermissions()}");
    //
    // return;

    // var container = InventoryManager.Instance()->GetInventoryContainer(InventoryType.HousingInteriorPlacedItems2);
    //
    // var count = (int) container->Size;
    // PluginLog.Log($"@@@@ House Ext Count: {count}");
    // var itemSheet = _dataManager.Excel.GetSheet<Item>();
    // PluginLog.Log($"@@@@ item count: {itemSheet?.RowCount}");
    // for (var i = 0; i < count; i++)
    // {
    //     var item = container->GetInventorySlot(i);
    //     if (item->ItemID == 0) continue;
    //     var sheetItem = itemSheet?.GetRow(item->ItemID);
    //     var name = sheetItem?.Name.ToString() ?? "???";
    //     var sortCategory = sheetItem?.ItemUICategory.Value;
    //     PluginLog.Log($"@@@@@@ [{item->Slot}] {item->ItemID}: {name} ({item->Quantity})  {sheetItem?.Unknown19}");
    // }
    // }

    public string Name => "HousingInv";

    /// <summary>
    ///     Disposes all of the resources created by the plugin.
    /// </summary>
    /// <remarks>
    ///     In theory all of these objects are non-null so they all exist by this point in code, however, there is
    ///     a try/catch block around the initialization code which means that the constructor can fail before
    ///     reaching this point. That means that some of these objects are actually null at this point so we
    ///     add a null check to all of them just in case. If this were an application we could ignore them as the
    ///     system would clean up regardless, but as we are a DLL that is loaded into an application, the clean up
    ///     will not occur until that application (Dalamud) exits.
    /// </remarks>
    public void Dispose()
    {
        // ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        _configuration?.Save(); // Should be auto-saved but let's be sure

        _clientState.TerritoryChanged -= ClientStateOnTerritoryChanged;
        UnregisterCommands();
        _windowManager?.Dispose();
        _pluginIcon?.Dispose();
        // ReSharper restore ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    }
}
