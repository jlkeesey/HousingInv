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
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using HousingInv.Localization;
using HousingInv.Model.Aetherytes;
using HousingInv.Model.FC;
using HousingInv.Model.Houses;
using HousingInv.Model.Location;
using HousingInv.Model.Players;
using HousingInv.Model.Servers;
using HousingInv.Model.Teleports;
using HousingInv.Model.Territories;
using HousingInv.Properties;
using HousingInv.System;
using HousingInv.Windows;
using ImGuiScene;
using Ninject;

// ReSharper disable UnusedType.Global

namespace HousingInv;

/// <summary>
///     The entry point for this plugin.
/// </summary>
public sealed class HousingInv : IDalamudPlugin
{
    private readonly Configuration _configuration;
    private readonly StandardKernel _kernel;

    public HousingInv([RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
                      [RequiredVersion("1.0")] ChatGui chatGui,
                      [RequiredVersion("1.0")] ClientState clientState,
                      [RequiredVersion("1.0")] CommandManager commandManager,
                      [RequiredVersion("1.0")] DataManager dataManager)
    {
        try
        {
            _configuration = Configuration.Load(pluginInterface);

            _kernel = new StandardKernel();
            // Add system objects as transient so they are not disposed.
            _kernel.Bind<DalamudPluginInterface>().ToConstant(pluginInterface).InTransientScope();
            _kernel.Bind<ChatGui>().ToConstant(chatGui).InTransientScope();
            _kernel.Bind<ClientState>().ToConstant(clientState).InTransientScope();
            _kernel.Bind<CommandManager>().ToConstant(commandManager).InTransientScope();
            _kernel.Bind<DataManager>().ToConstant(dataManager).InTransientScope();

            // Add the plugin objects/classes as singletons so they are dispose. Could be any scope other than transient.
            _kernel.Bind<Configuration>().ToConstant(_configuration).InSingletonScope();
            _kernel.Bind<ILogger>().To<Logger>().InSingletonScope();
            _kernel.Bind<Loc>().ToSelf().InSingletonScope();

            _kernel.Bind<IServerManager>().To<ServerManager>().InSingletonScope();
            _kernel.Bind<ITerritoryManager>().To<TerritoryManager>().InSingletonScope();
            _kernel.Bind<ILocator>().To<Locator>().InSingletonScope();
            _kernel.Bind<IAetheryteManager>().To<AetheryteManager>().InSingletonScope();
            _kernel.Bind<ITeleportLocationManager>().To<TeleportLocationManager>().InSingletonScope();
            _kernel.Bind<IFreeCompanyManager>().To<FreeCompanyManager>().InSingletonScope();
            _kernel.Bind<IMyself>().To<Myself>().InSingletonScope();
            _kernel.Bind<IHouseManager>().To<HouseManager>().InSingletonScope();
            _kernel.Bind<ConfigWindow>().ToSelf().InSingletonScope();
            _kernel.Bind<ICommands>().To<Commands>().InSingletonScope();
            _kernel.Bind<FileDialogManager>().To<FileDialogManager>().InSingletonScope();
            _kernel.Bind<JlkWindowManager>().ToSelf().InSingletonScope();

            var pluginIcon = pluginInterface.UiBuilder.LoadImage(Resources.Icon);
            _kernel.Bind<TextureWrap>().ToConstant(pluginIcon).Named("pluginIcon");
            _kernel.Bind<string>().ToConstant(Name).Named("nameSpace");

            // Create the objects that make up the plugin
            _kernel.Get<JlkWindowManager>();
            _kernel.Get<ICommands>().RegisterCommands();

            PluginLog.Log($"@@@@ Config dir '{pluginInterface.ConfigDirectory}'");
            PluginLog.Log($"@@@@ Config file '{pluginInterface.ConfigFile}'");
            PluginLog.Log($"@@@@ Config loc dir '{pluginInterface.GetPluginLocDirectory()}'");
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    // ReSharper disable once UnusedMember.Global
    public static string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;

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

        PluginLog.Log("@@@  Before kernel dispose");
        _kernel.Dispose();
        PluginLog.Log("@@@  Done disposing");
        // ReSharper restore ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    }
}

public interface IPluginIcon : TextureWrap
{
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
