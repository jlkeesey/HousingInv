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
using System.Text;
using Lumina.Excel;

namespace HousingInv.Model;

#if DEBUG
/// <summary>
///     Defines the column to display and its label.
/// </summary>
/// <typeparam name="T"></typeparam>
public class LogExcelColumn<T> where T : ExcelRow
{
    /// <summary>
    ///     Constructs a column definition.
    /// </summary>
    /// <param name="label">The column label.</param>
    /// <param name="width">The column width. Positive values are right justified, negative values are left justified.</param>
    /// <param name="value">Function to get the column value from a sheet row.</param>
    public LogExcelColumn(string label, int width, Func<T, object?> value)
    {
        Label = label;
        Width = Math.Sign(width) * Math.Max(Label.Length, Math.Abs(width));
        Value = value;
    }

    private string Label { get; }
    private int Width { get; }
    private Func<T, object?> Value { get; }

    /// <summary>
    ///     Returns the value of this column from the given row, padded either left or right based on the width's sign.
    /// </summary>
    /// <param name="row">The row to extract the column from.</param>
    /// <returns>The justified column value. Positive values are right justified, negative values are left justified.</returns>
    private string PaddedValue(T row)
    {
        var value = Value(row)?.ToString() ?? "";
        return Width < 0 ? value.PadRight(-Width) : value.PadLeft(Width);
    }

    /// <summary>
    ///     Adds the column label to the given builder. Separates this value from the previous by a single space.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder" /> to add the label to.</param>
    public void AppendLabel(StringBuilder builder)
    {
        if (builder.Length > 0) builder.Append(' ');

        builder.Append(Label.PadRight(Math.Abs(Width)));
    }

    /// <summary>
    ///     Adds the column label separator line to the given builder. Separates this value from the previous by a single
    ///     space.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder" /> to add the label to.</param>
    public void AppendSeparator(StringBuilder builder)
    {
        if (builder.Length > 0) builder.Append(' ');

        builder.Append(string.Empty.PadRight(Math.Abs(Width), '-'));
    }

    /// <summary>
    ///     Adds the justified value for the column to the given builder. Separates this value from the previous by a single
    ///     space.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder" /> to add the label to.</param>
    /// <param name="row">The sheet row to extract the value from.</param>
    public void AppendLValue(StringBuilder builder, T row)
    {
        if (builder.Length > 0) builder.Append(' ');

        builder.Append(PaddedValue(row));
    }
}
#endif
