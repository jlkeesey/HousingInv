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
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using HousingInv.Localization;
using HousingInv.Model.Aetherytes;
using HousingInv.Model.FC;
using HousingInv.Model.Houses;
using HousingInv.Model.Teleports;
using HousingInv.Model.Territories;
using ImGuiNET;
using ImGuiScene;
using static System.String;

// ReSharper disable InvertIf

namespace HousingInv.Windows;

/// <summary>
///     Defines the configuration editing window.
/// </summary>
public sealed partial class ConfigWindow : Window, IDisposable
{
    private const string Title = "HousingInv Configuration";
    private readonly AetheryteManager _aetheryteManager;

    private readonly TextureWrap? _chatterImage;
    private readonly Configuration _configuration;
    private readonly FreeCompanyManager _freeCompanyManager;
    private readonly HouseManager _houseManager;
    private readonly Loc _loc;
    private readonly TeleportLocationManager _teleportLocationManager;
    private readonly TerritoryManager _territoryManager;

    /// <summary>
    ///     Constructs the configuration editing window.
    /// </summary>
    /// <param name="config"></param>
    /// <param name="chatterImage">The Chatter plugin icon.</param>
    /// <param name="territoryManager"></param>
    /// <param name="aetheryteManager"></param>
    /// <param name="houseManager"></param>
    /// <param name="loc"></param>
    /// <param name="teleportLocationManager"></param>
    /// <param name="freeCompanyManager"></param>
    public ConfigWindow(Configuration config,
                        TextureWrap? chatterImage,
                        TerritoryManager territoryManager,
                        AetheryteManager aetheryteManager,
                        TeleportLocationManager teleportLocationManager,
                        FreeCompanyManager freeCompanyManager,
                        HouseManager houseManager,
                        Loc loc) : base(Title)
    {
        _configuration = config;
        _chatterImage = chatterImage;
        _territoryManager = territoryManager;
        _aetheryteManager = aetheryteManager;
        _teleportLocationManager = teleportLocationManager;
        _freeCompanyManager = freeCompanyManager;
        _houseManager = houseManager;
        _loc = loc;

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(450, 520), MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };

        Size = new Vector2(800, 520);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public void Dispose()
    {
    }

    /// <summary>
    ///     Draws this window.
    /// </summary>
    public override void Draw()
    {
        if (_chatterImage != null)
        {
            ImGui.Image(_chatterImage.ImGuiHandle, new Vector2(64, 64));
            VerticalSpace(5.0f);
        }

        if (ImGui.BeginTabBar("tabBar", ImGuiTabBarFlags.None))
        {
            if (ImGui.BeginTabItem(MsgTabGeneral))
            {
                DrawGeneralTab();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }

    /// <summary>
    ///     Adds vertical space to the output.
    /// </summary>
    /// <param name="space">The amount of extra space to add in <c>ImGUI</c> units.</param>
    private static void VerticalSpace(float space = 3.0f)
    {
        ImGui.Dummy(new Vector2(0.0f, space));
    }

    /// <summary>
    ///     Draws the general settings tab. This is where all the settings that affect the entire plugin are edited.
    /// </summary>
    private void DrawGeneralTab()
    {
        LongInputField(MsgLabelFileNamePrefix,
                       ref _configuration.Temp,
                       50,
                       "##fileNamePrefix",
                       MsgLabelFileNamePrefixHelp);

        if (ImGui.Button("Territory")) _territoryManager.ListAll();
        ImGui.SameLine();
        if (ImGui.Button("Aetheryte")) _aetheryteManager.ListAll();
        ImGui.SameLine();
        if (ImGui.Button("Teleport")) _teleportLocationManager.ListAll();
        ImGui.SameLine();
        if (ImGui.Button("FC")) _freeCompanyManager.LogFc();
        ImGui.SameLine();
        if (ImGui.Button("House")) _houseManager.LogHere();

        VerticalSpace();
    }

    /// <summary>
    ///     Creates an input field for a long value such that the label is not on the same line as the input field.
    /// </summary>
    /// <param name="label">The text label for this field.</param>
    /// <param name="value">The field value. This must be a ref value.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <param name="id">The optional id for the field. This is only necessary if the label is not unique.</param>
    /// <param name="help">The optional help text displayed when hovering over the help button.</param>
    /// <param name="extra">Function to add extra parts to the end of the widget.</param>
    /// <param name="extraWidth">The width of the extra element(s).</param>
    private static void LongInputField(string label,
                                       ref string value,
                                       uint maxLength = 100,
                                       string? id = null,
                                       string? help = null,
                                       Action? extra = null,
                                       int extraWidth = 0)
    {
        ImGui.Text(label);
        HelpMarker(help);

        ImGui.SetNextItemWidth(extraWidth == 0 ? -1 : -extraWidth);
        ImGui.InputText(id ?? label, ref value, maxLength);
        if (extra != null)
        {
            ImGui.SameLine();
            extra();
        }
    }

    /// <summary>
    ///     Adds a help button that shows the given help text when hovered over.
    /// </summary>
    /// <param name="description">
    ///     The description to show. If this is <c>null</c>, empty, or all whitespace, nothing is
    ///     created.
    /// </param>
    /// <param name="sameLine"><c>true</c> if this should be on the same line as the previous item.</param>
    private static void HelpMarker(string? description, bool sameLine = true)
    {
        var text = description?.Trim() ?? Empty;
        if (IsNullOrWhiteSpace(text)) return;
        if (sameLine) ImGui.SameLine();
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.4f, 0.4f, 0.55f, 1.0f));
        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.Text($"{(char) FontAwesomeIcon.QuestionCircle}");
        ImGui.PopFont();
        ImGui.PopStyleColor();
        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled)) DrawTooltip(text);
    }

    /// <summary>
    ///     Creates a tooltip box with the given content text which will be wrapped as necessary.
    /// </summary>
    /// <param name="description">The contents of the tooltip box. If <c>null</c> or empty the tooltip box is not created.</param>
    private static void DrawTooltip(string? description)
    {
        var text = description?.Trim() ?? Empty;
        if (IsNullOrWhiteSpace(text)) return;
        ImGui.BeginTooltip();
        ImGui.PushTextWrapPos(ImGui.GetFontSize() * 20.0f);
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 0.3f, 1.0f));
        ImGui.TextUnformatted(text);
        ImGui.PopStyleColor();
        ImGui.PopTextWrapPos();
        ImGui.EndTooltip();
    }
}
