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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Dalamud.Logging;
using Lumina.Excel;

namespace HousingInv.Model;

#if DEBUG
/// <summary>
///     Helper class for displaying values from a Lumina excel sheet. This is for debugging only.
/// </summary>
/// <typeparam name="T">The sheet type to operate on.</typeparam>
public class LogExcelRowsWriter<T> : IEnumerable<LogExcelColumn<T>> where T : ExcelRow
{
    private readonly List<LogExcelColumn<T>> _columns = new();

    public IEnumerator<LogExcelColumn<T>> GetEnumerator()
    {
        return _columns.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    ///     Writes all of the rows from the given sheet to the Dalamud log.
    /// </summary>
    /// <param name="sheet">The sheet to process.</param>
    public void Write(ExcelSheet<T> sheet)
    {
        var builder = new StringBuilder();

        AppendHeader(builder);
        AppendSeparator(builder);
        for (var i = 0u; i < sheet.RowCount; i++) AppendRow(sheet, i, builder);
    }

    /// <summary>
    ///     Adds the given row's data to the log output.
    /// </summary>
    /// <param name="sheet">The sheet to process.</param>
    /// <param name="i">The row index.</param>
    /// <param name="builder">The builder to use to generate the output.</param>
    private void AppendRow(ExcelSheet<T> sheet, uint i, StringBuilder builder)
    {
        var row = sheet.GetRow(i);
        if (row == null)
            builder.Append("--null row--");
        else
            foreach (var t in _columns)
                t.AppendLValue(builder, row);

        PluginLog.Log(builder.ToString());
        builder.Clear();
    }

    /// <summary>
    ///     Writes the header separator lines to the log output.
    /// </summary>
    /// <param name="builder">The builder to use to generate the output.</param>
    private void AppendSeparator(StringBuilder builder)
    {
        foreach (var t in _columns) t.AppendSeparator(builder);
        PluginLog.Log(builder.ToString());
        builder.Clear();
    }

    /// <summary>
    ///     Writes the column headers to the log output.
    /// </summary>
    /// <param name="builder">The builder to use to generate the output.</param>
    private void AppendHeader(StringBuilder builder)
    {
        foreach (var t in _columns) t.AppendLabel(builder);
        PluginLog.Log(builder.ToString());
        builder.Clear();
    }

    /// <summary>
    ///     Adds the given data to the list of column definitions. Used by the data construction.
    /// </summary>
    /// <param name="label">The column label.</param>
    /// <param name="width">The column width. Positive values are right justified, negative values are left justified.</param>
    /// <param name="value">Function to get the column value from a sheet row.</param>
    public void Add(string label, int width, Func<T, object?> value)
    {
        _columns.Add(new LogExcelColumn<T>(label, width, value));
    }
}
#endif
