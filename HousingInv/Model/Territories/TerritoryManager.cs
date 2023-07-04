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
using System.Collections.Generic;
using Dalamud.Data;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace HousingInv.Model.Territories;

/// <summary>
///     Reads and parses the FFXIV internal data for a territories into a usable format.
/// </summary>
public class TerritoryManager : ITerritoryManager
{
    private readonly Dictionary<uint, Territory> _cache = new();
    private readonly ExcelSheet<TerritoryType> _territoryTypes;
    private bool _cacheFilled;

    public TerritoryManager(DataManager dataManager)
    {
        _territoryTypes = dataManager.Excel.GetSheet<TerritoryType>()
                       ?? throw new ApplicationException("Cannot read ExcelSheet<TerritoryType>");
    }

    /// <summary>
    ///     An <see cref="IEnumerable{T}" /> over all of the defined territories.
    /// </summary>
    public IEnumerable<Territory> Territories
    {
        get
        {
            FillAll();
            return _cache.Values;
        }
    }

    public Territory this[uint? index] => Get(index);

    /// <summary>
    ///     Returns the <see cref="Territory" /> for the given id.
    /// </summary>
    /// <param name="id">The territory id to query.</param>
    /// <returns>The given <see cref="Territory" /> or <see cref="Territory.Empty" /> if not found.</returns>
    public Territory Get(uint? id)
    {
        if (id == null) return Territory.Empty;
        return _cache.TryGetValue((uint) id, out var territory) ? territory : Make(_territoryTypes.GetRow((uint) id));
    }

    /// <summary>
    ///     Creates a <see cref="Territory" /> from the given Lumina data row.
    /// </summary>
    /// <param name="territoryTypeRow">The <see cref="TerritoryType" /> to parse.</param>
    /// <returns>The newly created <see cref="Territory" /> or <see cref="Territory.Empty" /> if cannot be parsed.</returns>
    private Territory Make(TerritoryType? territoryTypeRow)
    {
        if (territoryTypeRow == null) return Territory.Empty;
        if (territoryTypeRow.TerritoryIntendedUse == (uint) TerritoryUse.Main
         && territoryTypeRow.PlaceName.Value == null)
            return Territory.Empty;

        var name = territoryTypeRow.PlaceName.Value?.Name.ToString() ?? "?[name]?";
        var zone = territoryTypeRow.PlaceNameZone.Value?.Name.ToString() ?? "?[zone]?";
        var region = territoryTypeRow.PlaceNameRegion.Value?.Name.ToString() ?? "?[region]?";
        var territory = new Territory(territoryTypeRow.RowId,
                                      name,
                                      zone,
                                      region,
                                      (TerritoryUse) territoryTypeRow.TerritoryIntendedUse);
        _cache[territory.Id] = territory;

        return territory;
    }

    /// <summary>
    ///     Queries all of the territories to force them to be created and inserted into the cache.
    /// </summary>
    private void FillAll()
    {
        if (_cacheFilled) return;
        var count = _territoryTypes.RowCount;
        for (var i = 0u; i < count; i++) Get(i); // Called for side-effect of loading the cache
        _cacheFilled = true;
    }

#if DEBUG
    private static readonly LogExcelRowsWriter<TerritoryType> ColumnDefinitions = new()
    {
        {"Id", 5, a => a.RowId},
        {"Use", -5, a => a.TerritoryIntendedUse},
        {"Region", -30, a => a.PlaceNameRegion.Value?.Name},
        {"Zone", -30, a => a.PlaceNameZone.Value?.Name},
        {"Name", -30, a => a.PlaceName.Value?.Name},
    };

    /// <summary>
    ///     Lists all of the known territories to the console.
    /// </summary>
    public void ListAll()
    {
        ColumnDefinitions.Write(_territoryTypes);
    }
#endif
}
