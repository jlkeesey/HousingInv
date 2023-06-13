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

using HousingInv.Model.Aetherytes;
using HousingInv.Model.Territories;

namespace HousingInv.Model.Teleports;

public abstract class TeleportLocation
{
    protected TeleportLocation(Aetheryte aetheryte, Territory territory, string name, string? longName = null)
    {
        Aetheryte = aetheryte;
        Territory = territory;
        Name = name;
        LongName = longName ?? Name;
    }

    public Aetheryte Aetheryte { get; }
    public Territory Territory { get; }

    public string LongName { get; }

    public string Name { get; }

    public override string ToString()
    {
        return LongName;
    }
}

public abstract class HousingLocation : TeleportLocation
{
    protected HousingLocation(Aetheryte aetheryte, Territory territory, string name, string? longName = null) :
        base(aetheryte, territory, name, longName)
    {
    }
}

public class FreeCompanyLocation : HousingLocation
{
    public FreeCompanyLocation(Aetheryte aetheryte, Territory territory) : base(aetheryte, territory, "Free Company")
    {
    }
}

public class PrivateHouseLocation : HousingLocation
{
    public PrivateHouseLocation(Aetheryte aetheryte, Territory territory) : base(aetheryte, territory, "Your house")
    {
    }
}

public class ApartmentLocation : HousingLocation
{
    public ApartmentLocation(Aetheryte aetheryte, Territory territory, int room) : base(aetheryte,
        territory,
        $"{territory.Name} ({room})",
        $"Your apartment at {territory.Name}, room {room}")
    {
        Room = room;
    }

    public int Room { get; }
}

public class SharedHouseLocation : HousingLocation
{
    public SharedHouseLocation(Aetheryte aetheryte, Territory territory, int ward, int plot) : base(aetheryte,
        territory,
        $"{territory.Name}, ({ward}, {plot})",
        $"Shared Estate at {territory.Name} (Ward {ward}, Plot {plot})")
    {
        Ward = ward;
        Plot = plot;
    }

    public int Ward { get; }
    public int Plot { get; }
}

public class AetheryteLocation : TeleportLocation
{
    public AetheryteLocation(Aetheryte aetheryte, Territory territory) : base(aetheryte,
                                                                              territory,
                                                                              aetheryte.Name,
                                                                              aetheryte.LongName)
    {
    }
}
