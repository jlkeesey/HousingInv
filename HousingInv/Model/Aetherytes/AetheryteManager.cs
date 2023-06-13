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
using HousingInv.Model.Territories;
using Lumina.Excel;

namespace HousingInv.Model.Aetherytes;

public class AetheryteManager
{
    private readonly ExcelSheet<Lumina.Excel.GeneratedSheets.Aetheryte> _aetheryteSheet;
    private readonly Dictionary<uint, Aetheryte> _cache = new();
    private readonly TerritoryManager _territoryManager;
    private bool _cacheFilled;

    public AetheryteManager(DataManager dataManager, TerritoryManager territoryManager)
    {
        _aetheryteSheet = dataManager.Excel.GetSheet<Lumina.Excel.GeneratedSheets.Aetheryte>()
                       ?? throw new ApplicationException("Cannot read ExcelSheet<Aetheryte>");
        _territoryManager = territoryManager;
    }

    /// <summary>
    ///     An <see cref="IEnumerable{T}" /> over all of the defined aetherytes.
    /// </summary>
    public IEnumerable<Aetheryte> Aetherytes
    {
        get
        {
            FillAll();
            return _cache.Values;
        }
    }

    public Aetheryte this[uint? index] => Get(index);

    public Aetheryte Get(uint? id)
    {
        if (id == null) return Aetheryte.Empty;
        return _cache.TryGetValue((uint) id, out var aetheryte) ? aetheryte : Make(_aetheryteSheet.GetRow((uint) id));
    }

    private Aetheryte Make(Lumina.Excel.GeneratedSheets.Aetheryte? aetheryteRow)
    {
        if (aetheryteRow == null) return Aetheryte.Empty;

        var name = aetheryteRow.PlaceName.Value?.Name ?? "?[name]?";
        var aethernetName = aetheryteRow.AethernetName.Value?.Name ?? "?[aethernetName]?";
        var territory = _territoryManager[aetheryteRow.Territory.Value?.RowId ?? uint.MaxValue];
        var aetheryte = new Aetheryte(aetheryteRow.RowId,
                                      name,
                                      aethernetName,
                                      aetheryteRow.IsAetheryte,
                                      aetheryteRow.AethernetGroup,
                                      territory,
                                      aetheryteRow.Order);
        _cache[aetheryteRow.RowId] = aetheryte;

        return aetheryte;
    }

    /// <summary>
    ///     Queries all of the aetherytes to force them to be created and inserted into the cache.
    /// </summary>
    private void FillAll()
    {
        if (_cacheFilled) return;
        var count = _aetheryteSheet.RowCount;
        for (var i = 0u; i < count; i++) Get(i); // Called for side-effect of loading the cache
        _cacheFilled = true;
    }

#if DEBUG
    private static readonly LogExcelRowsWriter<Lumina.Excel.GeneratedSheets.Aetheryte> ColumnDefinitions = new()
    {
        {"Id", 5, a => a.RowId},
        {"IsAetheryte", -5, a => a.IsAetheryte},
        {"Group", -5, a => a.AethernetGroup},
        {"Name", -30, a => a.PlaceName.Value?.Name},
        {"Aether Name", -30, a => a.AethernetName.Value?.Name},
    };

    /// <summary>
    ///     Lists all of the known aetherytes to the console.
    /// </summary>
    public void ListAll()
    {
        ColumnDefinitions.Write(_aetheryteSheet);
    }
#endif
}
