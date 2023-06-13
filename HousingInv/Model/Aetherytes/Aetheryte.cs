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

using HousingInv.Model.Territories;

namespace HousingInv.Model.Aetherytes;

public class Aetheryte
{
    public static readonly Aetheryte Empty = new(0,
                                                 "?[name]?",
                                                 "?[aethernetName]?",
                                                 false,
                                                 -1,
                                                 Territory.Empty,
                                                 uint.MaxValue);

    public Aetheryte(uint id,
                     string name,
                     string aethernetName,
                     bool isMain,
                     int group,
                     Territory territory,
                     uint order)
    {
        Id = id;
        Name = name;
        AethernetName = aethernetName;
        IsMain = isMain;
        Group = group;
        Territory = territory;
        Order = order;
    }

    public string LongName => Territory == Territory.Empty ? Name : $"{Name} in {Territory.Name}";

    public bool IsMain { get; set; }

    public uint Id { get; }

    public string Name { get; }
    public string AethernetName { get; }
    public Territory Territory { get; }
    public int Group { get; }
    public uint Order { get; }
}
