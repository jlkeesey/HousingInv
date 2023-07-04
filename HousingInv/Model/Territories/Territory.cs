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

namespace HousingInv.Model.Territories;

/// <summary>
///     These are the use types that I have figured out so far.
/// </summary>
public enum TerritoryUse
{
    // ReSharper disable IdentifierTypo
    Main = 0,
    Land = 1,
    Inn = 2,
    Dungeon = 3,
    VariantDungeon = 4,
    Gaol = 5,
    AllianceRaid = 8,
    Trial = 10,
    ResidentialZone = 13,
    Residence = 14,
    Interior = 15,
    NormalRaid1 = 16,
    NormalRaid2 = 17,
    Firmament = 21,
    Sanctum = 22,
    Diadem = 26,
    Fold = 27,
    Barracks = 30,
    DeepDungeon = 31,
    EventLocale = 32,
    TreasureHunt = 33,
    Eureka = 41,
    // ReSharper restore IdentifierTypo
}

/// <summary>
///     An area of the game where a character can be e.g. Mist, a dungeon, or your inn room.
/// </summary>
public class Territory : IComparable<Territory>, IComparable, IEquatable<Territory>
{
    public static readonly Territory Empty = new(0, "?[name]?", "?[zone]?", "?[region]?", TerritoryUse.Main);

    public Territory(uint id, string name, string zone, string region, TerritoryUse territoryUse)
    {
        Id = id;
        Name = name;
        Zone = zone;
        Region = region;
        TerritoryUse = territoryUse;
    }

    /// <summary>
    ///     The territory id of this territory.
    /// </summary>
    public uint Id { get; }

    /// <summary>
    ///     The region of this territory such as <c>La Noscea</c>. This will often be the same as the <see cref="Zone" /> This
    ///     can be blank for certain territories such a the Gaol that is no where.
    /// </summary>
    public string Region { get; }

    /// <summary>
    ///     The zone of this territory such as <c>La Noscea</c>. This can be blank for certain territories such a the Gaol that
    ///     is no where.
    /// </summary>
    public string Zone { get; }

    /// <summary>
    ///     The name of this territory such as <c>Lower La Noscea</c>.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// What this territory is used for.
    /// </summary>
    public TerritoryUse TerritoryUse { get; }

    public override string ToString()
    {
        return $"{Region}:{Zone}:{Name} for {TerritoryUse}";
    }

    public static bool MatchResidential(Territory lhs, Territory rhs)
    {
        if (lhs.Zone != rhs.Zone || lhs.Region != rhs.Region) return false;
        return lhs.TerritoryUse is TerritoryUse.ResidentialZone or TerritoryUse.Residence
            && rhs.TerritoryUse is TerritoryUse.ResidentialZone or TerritoryUse.Residence;
    }

    #region Comparable

    public int CompareTo(Territory? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var nameComparison = string.Compare(Name, other.Name, StringComparison.Ordinal);
        if (nameComparison != 0) return nameComparison;
        var zoneComparison = string.Compare(Zone, other.Zone, StringComparison.Ordinal);
        if (zoneComparison != 0) return zoneComparison;
        var regionComparison = string.Compare(Region, other.Region, StringComparison.Ordinal);
        if (regionComparison != 0) return regionComparison;
        return TerritoryUse.CompareTo(other.TerritoryUse);
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is Territory other
                   ? CompareTo(other)
                   : throw new ArgumentException($"Object must be of type {nameof(Territory)}");
    }

    public static bool operator <(Territory? left, Territory? right)
    {
        return Comparer<Territory>.Default.Compare(left, right) < 0;
    }

    public static bool operator >(Territory? left, Territory? right)
    {
        return Comparer<Territory>.Default.Compare(left, right) > 0;
    }

    public static bool operator <=(Territory? left, Territory? right)
    {
        return Comparer<Territory>.Default.Compare(left, right) <= 0;
    }

    public static bool operator >=(Territory? left, Territory? right)
    {
        return Comparer<Territory>.Default.Compare(left, right) >= 0;
    }

    #endregion

    #region Equality

    public bool Equals(Territory? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (MatchResidential(this, other))
        {
            return true;
        }
        return Zone == other.Zone && Region == other.Region && TerritoryUse == other.TerritoryUse;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Territory) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Zone, Region, (int) TerritoryUse);
    }

    public static bool operator ==(Territory? left, Territory? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Territory? left, Territory? right)
    {
        return !Equals(left, right);
    }

    #endregion
}

// ReSharper disable CommentTypo
/*
12:42:34.883 | INF [HousingInv]   Row Use             Region                         Zone                           Name                          
12:42:34.883 | INF [HousingInv] ----- ---             ------------------------------ ------------------------------ ------------------------------
12:42:34.883 | INF [HousingInv]   134 Land            La Noscea                      La Noscea                      Middle La Noscea              
12:42:34.883 | INF [HousingInv]   135 Land            La Noscea                      La Noscea                      Lower La Noscea               
12:42:34.883 | INF [HousingInv]   137 Land            La Noscea                      La Noscea                      Eastern La Noscea             
12:42:34.883 | INF [HousingInv]   138 Land            La Noscea                      La Noscea                      Western La Noscea             
12:42:34.883 | INF [HousingInv]   139 Land            La Noscea                      La Noscea                      Upper La Noscea               
12:42:34.883 | INF [HousingInv]   180 Land            La Noscea                      La Noscea                      Outer La Noscea               
12:42:34.883 | INF [HousingInv]   250 Land            La Noscea                      ウルヴズジェイル                       Wolves' Den Pier              
12:42:34.883 | INF [HousingInv]   148 Land            The Black Shroud               The Black Shroud               Central Shroud                
12:42:34.883 | INF [HousingInv]   152 Land            The Black Shroud               The Black Shroud               East Shroud                   
12:42:34.883 | INF [HousingInv]   153 Land            The Black Shroud               The Black Shroud               South Shroud                  
12:42:34.883 | INF [HousingInv]   154 Land            The Black Shroud               The Black Shroud               North Shroud                  
12:42:34.883 | INF [HousingInv]   140 Land            Thanalan                       Thanalan                       Western Thanalan              
12:42:34.883 | INF [HousingInv]   141 Land            Thanalan                       Thanalan                       Central Thanalan              
12:42:34.883 | INF [HousingInv]   145 Land            Thanalan                       Thanalan                       Eastern Thanalan              
12:42:34.883 | INF [HousingInv]   146 Land            Thanalan                       Thanalan                       Southern Thanalan             
12:42:34.883 | INF [HousingInv]   147 Land            Thanalan                       Thanalan                       Northern Thanalan             
12:42:34.883 | INF [HousingInv]   155 Land            Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.883 | INF [HousingInv]   397 Land            Coerthas                       Coerthas                       Coerthas Western Highlands    
12:42:34.883 | INF [HousingInv]   401 Land            Abalathia's Spine              Abalathia's Spine              The Sea of Clouds             
12:42:34.883 | INF [HousingInv]   402 Land            Abalathia's Spine              Abalathia's Spine              Azys Lla                      
12:42:34.883 | INF [HousingInv]   398 Land            Dravania                       Dravania                       The Dravanian Forelands       
12:42:34.883 | INF [HousingInv]   400 Land            Dravania                       Dravania                       The Churning Mists            
12:42:34.883 | INF [HousingInv]   612 Land            Gyr Abania                     Gyr Abania                     The Fringes                   
12:42:34.883 | INF [HousingInv]   620 Land            Gyr Abania                     Gyr Abania                     The Peaks                     
12:42:34.883 | INF [HousingInv]   621 Land            Gyr Abania                     Gyr Abania                     The Lochs                     
12:42:34.883 | INF [HousingInv]   613 Land            Othard                         Othard                         The Ruby Sea                  
12:42:34.883 | INF [HousingInv]   614 Land            Othard                         Othard                         Yanxia                        
12:42:34.883 | INF [HousingInv]   622 Land            Othard                         Othard                         The Azim Steppe               
12:42:34.883 | INF [HousingInv]   957 Land            Ilsabard                       Thavnair                       Thavnair                      
12:42:34.883 | INF [HousingInv]   958 Land            Ilsabard                       Garlemald                      Garlemald                     
12:42:34.883 | INF [HousingInv]   813 Land            Norvrandt                      Norvrandt                      Lakeland                      
12:42:34.883 | INF [HousingInv]   814 Land            Norvrandt                      Norvrandt                      Kholusia                      
12:42:34.883 | INF [HousingInv]   815 Land            Norvrandt                      Norvrandt                      Amh Araeng                    
12:42:34.884 | INF [HousingInv]   816 Land            Norvrandt                      Norvrandt                      Il Mheg                       
12:42:34.884 | INF [HousingInv]   817 Land            Norvrandt                      Norvrandt                      The Rak'tika Greatwood        
12:42:34.884 | INF [HousingInv]   818 Land            Norvrandt                      Norvrandt                      The Tempest                   
12:42:34.884 | INF [HousingInv]   156 Land            Mor Dhona                      Mor Dhona                      Mor Dhona                     
12:42:34.884 | INF [HousingInv]   956 Land            The Northern Empty             Labyrinthos                    Labyrinthos                   
12:42:34.884 | INF [HousingInv]   959 Land            The Sea of Stars               The Sea of Stars               Mare Lamentorum               
12:42:34.884 | INF [HousingInv]   960 Land            The Sea of Stars               The Sea of Stars               Ultima Thule                  
12:42:34.884 | INF [HousingInv]   961 Land            The World Unsundered           The World Unsundered           Elpis                         
12:42:34.884 | INF [HousingInv]   399 Land            Dravania                       Dravania                       The Dravanian Hinterlands     
12:42:34.884 | INF [HousingInv]   177 Inn             La Noscea                      Limsa Lominsa                  Mizzenmast Inn                
12:42:34.884 | INF [HousingInv]   178 Inn             Thanalan                       Ul'dah                         The Hourglass                 
12:42:34.884 | INF [HousingInv]   179 Inn             The Black Shroud               Gridania                       The Roost                     
12:42:34.884 | INF [HousingInv]   429 Inn             Coerthas                       Ishgard                        Cloud Nine                    
12:42:34.884 | INF [HousingInv]   629 Inn             Hingashi                       Kugane                         Bokairo Inn                   
12:42:34.884 | INF [HousingInv]   843 Inn             Norvrandt                      The Crystarium                 The Pendants Personal Suite   
12:42:34.884 | INF [HousingInv]   990 Inn             The Northern Empty             Old Sharlayan                  Andron                        
12:42:34.884 | INF [HousingInv]   159 Dungeon         La Noscea                      La Noscea                      The Wanderer's Palace         
12:42:34.884 | INF [HousingInv]   160 Dungeon         La Noscea                      La Noscea                      Pharos Sirius                 
12:42:34.884 | INF [HousingInv]   162 Dungeon         Thanalan                       Thanalan                       Halatali                      
12:42:34.884 | INF [HousingInv]   163 Dungeon         Thanalan                       Thanalan                       The Sunken Temple of Qarn     
12:42:34.884 | INF [HousingInv]   167 Dungeon         The Black Shroud               The Black Shroud               Amdapor Keep                  
12:42:34.884 | INF [HousingInv]   170 Dungeon         Thanalan                       Thanalan                       Cutter's Cry                  
12:42:34.884 | INF [HousingInv]   171 Dungeon         Coerthas                       Coerthas                       Dzemael Darkhold              
12:42:34.884 | INF [HousingInv]   172 Dungeon         Coerthas                       Coerthas                       Aurum Vale                    
12:42:34.884 | INF [HousingInv]   188 Dungeon         La Noscea                      La Noscea                      The Wanderer's Palace         
12:42:34.884 | INF [HousingInv]   189 Dungeon         The Black Shroud               The Black Shroud               Amdapor Keep                  
12:42:34.884 | INF [HousingInv]   190 Dungeon         The Black Shroud               The Black Shroud               Central Shroud                
12:42:34.884 | INF [HousingInv]   191 Dungeon         The Black Shroud               The Black Shroud               East Shroud                   
12:42:34.884 | INF [HousingInv]   192 Dungeon         The Black Shroud               The Black Shroud               South Shroud                  
12:42:34.884 | INF [HousingInv]   214 Dungeon         La Noscea                      La Noscea                      Middle La Noscea              
12:42:34.884 | INF [HousingInv]   215 Dungeon         Thanalan                       Thanalan                       Western Thanalan              
12:42:34.884 | INF [HousingInv]   216 Dungeon         Thanalan                       Thanalan                       Central Thanalan              
12:42:34.884 | INF [HousingInv]   219 Dungeon         The Black Shroud               The Black Shroud               Central Shroud                
12:42:34.884 | INF [HousingInv]   220 Dungeon         The Black Shroud               The Black Shroud               South Shroud                  
12:42:34.884 | INF [HousingInv]   221 Dungeon         La Noscea                      La Noscea                      Upper La Noscea               
12:42:34.884 | INF [HousingInv]   222 Dungeon         La Noscea                      La Noscea                      Lower La Noscea               
12:42:34.884 | INF [HousingInv]   223 Dungeon         Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.884 | INF [HousingInv]   298 Dungeon         Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.884 | INF [HousingInv]   299 Dungeon         Mor Dhona                      Mor Dhona                      Mor Dhona                     
12:42:34.884 | INF [HousingInv]   300 Dungeon         Mor Dhona                      Mor Dhona                      Mor Dhona                     
12:42:34.884 | INF [HousingInv]   349 Dungeon         Thanalan                       Thanalan                       Copperbell Mines              
12:42:34.884 | INF [HousingInv]   350 Dungeon         The Black Shroud               The Black Shroud               Haukke Manor                  
12:42:34.884 | INF [HousingInv]   360 Dungeon         Thanalan                       Thanalan                       Halatali                      
12:42:34.884 | INF [HousingInv]   361 Dungeon         La Noscea                      La Noscea                      Hullbreaker Isle              
12:42:34.884 | INF [HousingInv]   362 Dungeon         La Noscea                      La Noscea                      Brayflox's Longstop           
12:42:34.884 | INF [HousingInv]   363 Dungeon         The Black Shroud               The Black Shroud               The Lost City of Amdapor      
12:42:34.884 | INF [HousingInv]   365 Dungeon         Coerthas                       Coerthas                       Stone Vigil                   
12:42:34.884 | INF [HousingInv]   366 Dungeon         Coerthas                       Coerthas                       Griffin Crossing              
12:42:34.884 | INF [HousingInv]   367 Dungeon         Thanalan                       Thanalan                       The Sunken Temple of Qarn     
12:42:34.884 | INF [HousingInv]   373 Dungeon         The Black Shroud               The Black Shroud               The Tam-Tara Deepcroft        
12:42:34.884 | INF [HousingInv]   387 Dungeon         La Noscea                      La Noscea                      Sastasha                      
12:42:34.884 | INF [HousingInv]   396 Dungeon         The Black Shroud               The Black Shroud               Amdapor Keep                  
12:42:34.884 | INF [HousingInv]   420 Dungeon         Abalathia's Spine              Abalathia's Spine              Neverreap                     
12:42:34.884 | INF [HousingInv]   430 Dungeon         Abalathia's Spine              Abalathia's Spine              The Fractal Continuum         
12:42:34.884 | INF [HousingInv]   434 Dungeon         Coerthas                       Coerthas                       Dusk Vigil                    
12:42:34.884 | INF [HousingInv]   510 Dungeon         La Noscea                      La Noscea                      Pharos Sirius                 
12:42:34.884 | INF [HousingInv]   511 Dungeon         Dravania                       Dravania                       Saint Mocianne's Arboretum    
12:42:34.884 | INF [HousingInv]   519 Dungeon         The Black Shroud               The Black Shroud               The Lost City of Amdapor      
12:42:34.884 | INF [HousingInv]   557 Dungeon         La Noscea                      La Noscea                      Hullbreaker Isle              
12:42:34.884 | INF [HousingInv]   578 Dungeon         Dravania                       Dravania                       The Great Gubal Library       
12:42:34.884 | INF [HousingInv]   616 Dungeon         Othard                         Othard                         Shisui of the Violet Tides    
12:42:34.884 | INF [HousingInv]   617 Dungeon         Dravania                       Dravania                       Sohm Al                       
12:42:34.884 | INF [HousingInv]   623 Dungeon         Othard                         Othard                         Bardam's Mettle               
12:42:34.884 | INF [HousingInv]   626 Dungeon         La Noscea                      La Noscea                      The Sirensong Sea             
12:42:34.884 | INF [HousingInv]   660 Dungeon         Othard                         Othard                         Doma Castle                   
12:42:34.884 | INF [HousingInv]   661 Dungeon         Gyr Abania                     Gyr Abania                     Castrum Abania                
12:42:34.884 | INF [HousingInv]   662 Dungeon         Hingashi                       Kugane                         Kugane Castle                 
12:42:34.884 | INF [HousingInv]   663 Dungeon         Gyr Abania                     Gyr Abania                     The Temple of the Fist        
12:42:34.884 | INF [HousingInv]   689 Dungeon         Gyr Abania                     Gyr Abania                     Ala Mhigo                     
12:42:34.884 | INF [HousingInv]   731 Dungeon         Gyr Abania                     Gyr Abania                     The Drowned City of Skalla    
12:42:34.884 | INF [HousingInv]   742 Dungeon         Othard                         Othard                         Hells' Lid                    
12:42:34.884 | INF [HousingInv]   743 Dungeon         Abalathia's Spine              Abalathia's Spine              The Fractal Continuum         
12:42:34.884 | INF [HousingInv]   768 Dungeon         Othard                         Othard                         The Swallow's Compass         
12:42:34.884 | INF [HousingInv]   788 Dungeon         Dravania                       Dravania                       Saint Mocianne's Arboretum    
12:42:34.884 | INF [HousingInv]   789 Dungeon         Othard                         Othard                         The Burn                      
12:42:34.884 | INF [HousingInv]   793 Dungeon         Gyr Abania                     Gyr Abania                     The Ghimlyt Dark              
12:42:34.884 | INF [HousingInv]   821 Dungeon         Norvrandt                      Norvrandt                      Dohn Mheg                     
12:42:34.884 | INF [HousingInv]   822 Dungeon         Norvrandt                      Norvrandt                      Mt. Gulg                      
12:42:34.884 | INF [HousingInv]   823 Dungeon         Norvrandt                      Norvrandt                      The Qitana Ravel              
12:42:34.884 | INF [HousingInv]   831 Dungeon         Thanalan                       ゴールドソーサー                       The Manderville Tables        
12:42:34.884 | INF [HousingInv]   832 Dungeon         Thanalan                       ゴールドソーサー                       The Gold Saucer               
12:42:34.884 | INF [HousingInv]   836 Dungeon         Norvrandt                      Norvrandt                      Malikah's Well                
12:42:34.884 | INF [HousingInv]   837 Dungeon         Norvrandt                      Norvrandt                      Holminster Switch             
12:42:34.884 | INF [HousingInv]   838 Dungeon         Norvrandt                      Norvrandt                      Amaurot                       
12:42:34.884 | INF [HousingInv]   840 Dungeon         Norvrandt                      Norvrandt                      The Twinning                  
12:42:34.884 | INF [HousingInv]   841 Dungeon         Norvrandt                      Norvrandt                      Akadaemia Anyder              
12:42:34.884 | INF [HousingInv]   884 Dungeon         Norvrandt                      Norvrandt                      The Grand Cosmos              
12:42:34.884 | INF [HousingInv]   898 Dungeon         Norvrandt                      Norvrandt                      Anamnesis Anyder              
12:42:34.884 | INF [HousingInv]   916 Dungeon         Norvrandt                      Norvrandt                      The Heroes' Gauntlet          
12:42:34.884 | INF [HousingInv]   933 Dungeon         Dravania                       Dravania                       Matoya's Relict               
12:42:34.884 | INF [HousingInv]   938 Dungeon         Thanalan                       Thanalan                       Paglth'an                     
12:42:34.884 | INF [HousingInv]   952 Dungeon         Ilsabard                       Thavnair                       The Tower of Zot              
12:42:34.884 | INF [HousingInv]   969 Dungeon         Ilsabard                       Thavnair                       The Tower of Babil            
12:42:34.884 | INF [HousingInv]   970 Dungeon         Ilsabard                       Thavnair                       Vanaspati                     
12:42:34.884 | INF [HousingInv]   973 Dungeon         The Sea of Stars               The Sea of Stars               The Dead Ends                 
12:42:34.884 | INF [HousingInv]   974 Dungeon         The World Unsundered           The World Unsundered           Ktisis Hyperboreia            
12:42:34.884 | INF [HousingInv]   976 Dungeon         The Sea of Stars               The Sea of Stars               Smileton                      
12:42:34.884 | INF [HousingInv]   978 Dungeon         The Northern Empty             Labyrinthos                    The Aitiascope                
12:42:34.884 | INF [HousingInv]   986 Dungeon         The Sea of Stars               The Sea of Stars               The Stigma Dreamscape         
12:42:34.884 | INF [HousingInv]  1036 Dungeon         La Noscea                      La Noscea                      Sastasha                      
12:42:34.884 | INF [HousingInv]  1037 Dungeon         The Black Shroud               The Black Shroud               The Tam-Tara Deepcroft        
12:42:34.884 | INF [HousingInv]  1038 Dungeon         Thanalan                       Thanalan                       Copperbell Mines              
12:42:34.884 | INF [HousingInv]  1039 Dungeon         The Black Shroud               The Black Shroud               The Thousand Maws of Toto-Rak 
12:42:34.884 | INF [HousingInv]  1040 Dungeon         The Black Shroud               The Black Shroud               Haukke Manor                  
12:42:34.884 | INF [HousingInv]  1041 Dungeon         La Noscea                      La Noscea                      Brayflox's Longstop           
12:42:34.884 | INF [HousingInv]  1042 Dungeon         Coerthas                       Coerthas                       Stone Vigil                   
12:42:34.884 | INF [HousingInv]  1043 Dungeon         Thanalan                       Thanalan                       Castrum Meridianum            
12:42:34.884 | INF [HousingInv]  1044 Dungeon         Thanalan                       Thanalan                       The Praetorium                
12:42:34.884 | INF [HousingInv]  1050 Dungeon         Ilsabard                       Thavnair                       Alzadaal's Legacy             
12:42:34.884 | INF [HousingInv]  1062 Dungeon         Coerthas                       Coerthas                       Snowcloak                     
12:42:34.884 | INF [HousingInv]  1063 Dungeon         Mor Dhona                      Mor Dhona                      The Keeper of the Lake        
12:42:34.884 | INF [HousingInv]  1064 Dungeon         Dravania                       Dravania                       Sohm Al                       
12:42:34.884 | INF [HousingInv]  1065 Dungeon         Dravania                       Dravania                       The Aery                      
12:42:34.884 | INF [HousingInv]  1066 Dungeon         Coerthas                       Ishgard                        The Vault                     
12:42:34.884 | INF [HousingInv]  1069 VariantDungeon  Thanalan                       Thanalan                       The Sil'dihn Subterrane       
12:42:34.884 | INF [HousingInv]   176 Gaol                                                                          Mordion Gaol                  
12:42:34.884 | INF [HousingInv]   181 6               La Noscea                      Limsa Lominsa                  Limsa Lominsa                 
12:42:34.884 | INF [HousingInv]   182 6               Thanalan                       Ul'dah                         Ul'dah - Steps of Nald        
12:42:34.884 | INF [HousingInv]   183 6               The Black Shroud               Gridania                       New Gridania                  
12:42:34.884 | INF [HousingInv]   276 7                                                                             Hall of Summoning             
12:42:34.884 | INF [HousingInv]   331 7               Coerthas                       Coerthas                       The Howling Eye               
12:42:34.884 | INF [HousingInv]   569 7               Coerthas                       Coerthas                       Steps of Faith                
12:42:34.884 | INF [HousingInv]   680 7               La Noscea                      La Noscea                      The Misery                    
12:42:34.884 | INF [HousingInv]   683 7               Gyr Abania                     Gyr Abania                     The First Altar of Djanan Qhat
12:42:34.884 | INF [HousingInv]   727 7               Gyr Abania                     Gyr Abania                     The Royal Menagerie           
12:42:34.884 | INF [HousingInv]   740 7               Gyr Abania                     Gyr Abania                     The Royal Menagerie           
12:42:34.884 | INF [HousingInv]   833 7               Coerthas                       Coerthas                       The Howling Eye               
12:42:34.884 | INF [HousingInv]   889 7               Norvrandt                      Norvrandt                      Lyhe Mheg                     
12:42:34.884 | INF [HousingInv]   890 7               Norvrandt                      Norvrandt                      Lyhe Mheg                     
12:42:34.884 | INF [HousingInv]   891 7               Norvrandt                      Norvrandt                      Lyhe Mheg                     
12:42:34.884 | INF [HousingInv]   892 7               Norvrandt                      Norvrandt                      Lyhe Mheg                     
12:42:34.884 | INF [HousingInv]   915 7               ???                                                           Gangos                        
12:42:34.884 | INF [HousingInv]   965 7               Norvrandt                      Norvrandt                      The Empty                     
12:42:34.884 | INF [HousingInv]   988 7               Ilsabard                       Radz-at-Han                                                  
12:42:34.884 | INF [HousingInv]   989 7               The Northern Empty             Labyrinthos                                                  
12:42:34.884 | INF [HousingInv]   151 Raid            Mor Dhona                      Mor Dhona                      The World of Darkness         
12:42:34.884 | INF [HousingInv]   174 Raid            Mor Dhona                      Mor Dhona                      Labyrinth of the Ancients     
12:42:34.884 | INF [HousingInv]   372 Raid            Mor Dhona                      Mor Dhona                      Syrcus Tower                  
12:42:34.884 | INF [HousingInv]   508 Raid            Abalathia's Spine              Abalathia's Spine              Void Ark                      
12:42:34.884 | INF [HousingInv]   556 Raid            Mor Dhona                      Mor Dhona                      The Weeping City of Mhach     
12:42:34.884 | INF [HousingInv]   627 Raid            Abalathia's Spine              Abalathia's Spine              Dun Scaith                    
12:42:34.884 | INF [HousingInv]   734 Raid            ???                                                           The Royal City of Rabanastre  
12:42:34.884 | INF [HousingInv]   776 Raid            ???                                                           The Ridorana Lighthouse       
12:42:34.884 | INF [HousingInv]   826 Raid            ???                                                           The Orbonne Monastery         
12:42:34.884 | INF [HousingInv]   882 Raid            Norvrandt                      Norvrandt                      The Copied Factory            
12:42:34.884 | INF [HousingInv]   917 Raid            Norvrandt                      Norvrandt                      The Puppets' Bunker           
12:42:34.884 | INF [HousingInv]   966 Raid            Norvrandt                      Norvrandt                      The Tower at Paradigm's Breach
12:42:34.884 | INF [HousingInv]  1054 Raid            ???                                                           Aglaia                        
12:42:34.884 | INF [HousingInv]   225 9               The Black Shroud               The Black Shroud               Central Shroud                
12:42:34.884 | INF [HousingInv]   226 9               The Black Shroud               The Black Shroud               Central Shroud                
12:42:34.884 | INF [HousingInv]   227 9               The Black Shroud               The Black Shroud               Central Shroud                
12:42:34.884 | INF [HousingInv]   228 9               The Black Shroud               The Black Shroud               North Shroud                  
12:42:34.884 | INF [HousingInv]   229 9               The Black Shroud               The Black Shroud               South Shroud                  
12:42:34.884 | INF [HousingInv]   230 9               The Black Shroud               The Black Shroud               Central Shroud                
12:42:34.884 | INF [HousingInv]   231 9               The Black Shroud               The Black Shroud               South Shroud                  
12:42:34.884 | INF [HousingInv]   232 9               The Black Shroud               The Black Shroud               South Shroud                  
12:42:34.884 | INF [HousingInv]   233 9               The Black Shroud               The Black Shroud               Central Shroud                
12:42:34.884 | INF [HousingInv]   234 9               The Black Shroud               The Black Shroud               East Shroud                   
12:42:34.884 | INF [HousingInv]   235 9               The Black Shroud               The Black Shroud               South Shroud                  
12:42:34.884 | INF [HousingInv]   236 9               The Black Shroud               The Black Shroud               South Shroud                  
12:42:34.884 | INF [HousingInv]   237 9               The Black Shroud               The Black Shroud               Central Shroud                
12:42:34.884 | INF [HousingInv]   238 9               The Black Shroud               Gridania                       Old Gridania                  
12:42:34.884 | INF [HousingInv]   239 9               The Black Shroud               The Black Shroud               Central Shroud                
12:42:34.884 | INF [HousingInv]   240 9               The Black Shroud               The Black Shroud               North Shroud                  
12:42:34.884 | INF [HousingInv]   248 9               Thanalan                       Thanalan                       Central Thanalan              
12:42:34.884 | INF [HousingInv]   249 9               La Noscea                      La Noscea                      Lower La Noscea               
12:42:34.884 | INF [HousingInv]   251 9               Thanalan                       Ul'dah                         Ul'dah - Steps of Nald        
12:42:34.884 | INF [HousingInv]   252 9               La Noscea                      La Noscea                      Middle La Noscea              
12:42:34.884 | INF [HousingInv]   253 9               Thanalan                       Thanalan                       Central Thanalan              
12:42:34.884 | INF [HousingInv]   254 9               Thanalan                       Ul'dah                         Ul'dah - Steps of Nald        
12:42:34.884 | INF [HousingInv]   255 9               Thanalan                       Thanalan                       Western Thanalan              
12:42:34.884 | INF [HousingInv]   256 9               Thanalan                       Thanalan                       Eastern Thanalan              
12:42:34.884 | INF [HousingInv]   257 9               Thanalan                       Thanalan                       Eastern Thanalan              
12:42:34.884 | INF [HousingInv]   258 9               Thanalan                       Thanalan                       Central Thanalan              
12:42:34.884 | INF [HousingInv]   259 9               Thanalan                       Ul'dah                         Ul'dah - Steps of Nald        
12:42:34.884 | INF [HousingInv]   260 9               Thanalan                       Thanalan                       Southern Thanalan             
12:42:34.884 | INF [HousingInv]   261 9               Thanalan                       Thanalan                       Southern Thanalan             
12:42:34.884 | INF [HousingInv]   262 9               La Noscea                      La Noscea                      Lower La Noscea               
12:42:34.884 | INF [HousingInv]   263 9               La Noscea                      La Noscea                      Western La Noscea             
12:42:34.884 | INF [HousingInv]   264 9               La Noscea                      La Noscea                      Lower La Noscea               
12:42:34.884 | INF [HousingInv]   265 9               La Noscea                      La Noscea                      Lower La Noscea               
12:42:34.884 | INF [HousingInv]   266 9               Thanalan                       Thanalan                       Eastern Thanalan              
12:42:34.884 | INF [HousingInv]   267 9               Thanalan                       Thanalan                       Western Thanalan              
12:42:34.884 | INF [HousingInv]   268 9               Thanalan                       Thanalan                       Eastern Thanalan              
12:42:34.884 | INF [HousingInv]   269 9               Thanalan                       Thanalan                       Western Thanalan              
12:42:34.884 | INF [HousingInv]   270 9               Thanalan                       Thanalan                       Central Thanalan              
12:42:34.884 | INF [HousingInv]   271 9               Thanalan                       Thanalan                       Central Thanalan              
12:42:34.884 | INF [HousingInv]   272 9               La Noscea                      La Noscea                      Middle La Noscea              
12:42:34.885 | INF [HousingInv]   273 9               Thanalan                       Thanalan                       Western Thanalan              
12:42:34.885 | INF [HousingInv]   274 9               Thanalan                       Ul'dah                         Ul'dah - Steps of Nald        
12:42:34.885 | INF [HousingInv]   275 9               Thanalan                       Thanalan                       Eastern Thanalan              
12:42:34.885 | INF [HousingInv]   277 9               The Black Shroud               The Black Shroud               East Shroud                   
12:42:34.885 | INF [HousingInv]   278 9               Thanalan                       Thanalan                       Western Thanalan              
12:42:34.885 | INF [HousingInv]   279 9               La Noscea                      La Noscea                      Lower La Noscea               
12:42:34.885 | INF [HousingInv]   280 9               La Noscea                      La Noscea                      Western La Noscea             
12:42:34.885 | INF [HousingInv]   285 9               La Noscea                      La Noscea                      Middle La Noscea              
12:42:34.885 | INF [HousingInv]   286 9               La Noscea                      La Noscea                      Rhotano Sea                   
12:42:34.885 | INF [HousingInv]   287 9               La Noscea                      La Noscea                      Lower La Noscea               
12:42:34.885 | INF [HousingInv]   288 9               La Noscea                      La Noscea                      Rhotano Sea                   
12:42:34.885 | INF [HousingInv]   289 9               The Black Shroud               The Black Shroud               East Shroud                   
12:42:34.885 | INF [HousingInv]   290 9               The Black Shroud               The Black Shroud               East Shroud                   
12:42:34.885 | INF [HousingInv]   291 9               The Black Shroud               The Black Shroud               South Shroud                  
12:42:34.885 | INF [HousingInv]   301 9               Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.885 | INF [HousingInv]   302 9               Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.885 | INF [HousingInv]   303 9               The Black Shroud               The Black Shroud               East Shroud                   
12:42:34.885 | INF [HousingInv]   304 9               Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.885 | INF [HousingInv]   305 9               Mor Dhona                      Mor Dhona                      Mor Dhona                     
12:42:34.885 | INF [HousingInv]   306 9               Thanalan                       Thanalan                       Southern Thanalan             
12:42:34.885 | INF [HousingInv]   307 9               La Noscea                      La Noscea                      Lower La Noscea               
12:42:34.885 | INF [HousingInv]   308 9               Mor Dhona                      Mor Dhona                      Mor Dhona                     
12:42:34.885 | INF [HousingInv]   309 9               Mor Dhona                      Mor Dhona                      Mor Dhona                     
12:42:34.885 | INF [HousingInv]   310 9               La Noscea                      La Noscea                      Eastern La Noscea             
12:42:34.885 | INF [HousingInv]   311 9               La Noscea                      La Noscea                      Eastern La Noscea             
12:42:34.885 | INF [HousingInv]   312 9               Thanalan                       Thanalan                       Southern Thanalan             
12:42:34.885 | INF [HousingInv]   313 9               Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.885 | INF [HousingInv]   314 9               Thanalan                       Thanalan                       Central Thanalan              
12:42:34.885 | INF [HousingInv]   315 9               Mor Dhona                      Mor Dhona                      Mor Dhona                     
12:42:34.885 | INF [HousingInv]   316 9               Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.885 | INF [HousingInv]   317 9               The Black Shroud               The Black Shroud               South Shroud                  
12:42:34.885 | INF [HousingInv]   318 9               Thanalan                       Thanalan                       Southern Thanalan             
12:42:34.885 | INF [HousingInv]   319 9               The Black Shroud               The Black Shroud               Central Shroud                
12:42:34.885 | INF [HousingInv]   320 9               The Black Shroud               The Black Shroud               Central Shroud                
12:42:34.885 | INF [HousingInv]   321 9               The Black Shroud               The Black Shroud               North Shroud                  
12:42:34.885 | INF [HousingInv]   322 9               Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.885 | INF [HousingInv]   323 9               Thanalan                       Thanalan                       Southern Thanalan             
12:42:34.885 | INF [HousingInv]   324 9               The Black Shroud               The Black Shroud               North Shroud                  
12:42:34.885 | INF [HousingInv]   325 9               La Noscea                      La Noscea                      Outer La Noscea               
12:42:34.885 | INF [HousingInv]   326 9               Mor Dhona                      Mor Dhona                      Mor Dhona                     
12:42:34.885 | INF [HousingInv]   327 9               La Noscea                      La Noscea                      Eastern La Noscea             
12:42:34.885 | INF [HousingInv]   328 9               La Noscea                      La Noscea                      Upper La Noscea               
12:42:34.885 | INF [HousingInv]   329 9               La Noscea                      La Noscea                      The Wanderer's Palace         
12:42:34.885 | INF [HousingInv]   330 9               La Noscea                      La Noscea                      Western La Noscea             
12:42:34.885 | INF [HousingInv]   379 9               Mor Dhona                      Mor Dhona                      Mor Dhona                     
12:42:34.885 | INF [HousingInv]   404 9               La Noscea                      Limsa Lominsa                  Limsa Lominsa Lower Decks     
12:42:34.885 | INF [HousingInv]   405 9               La Noscea                      La Noscea                      Western La Noscea             
12:42:34.885 | INF [HousingInv]   406 9               La Noscea                      La Noscea                      Western La Noscea             
12:42:34.885 | INF [HousingInv]   407 9               La Noscea                      La Noscea                      Rhotano Sea                   
12:42:34.885 | INF [HousingInv]   408 9               La Noscea                      La Noscea                      Eastern La Noscea             
12:42:34.885 | INF [HousingInv]   409 9               La Noscea                      Limsa Lominsa                  Limsa Lominsa Upper Decks     
12:42:34.885 | INF [HousingInv]   410 9               Thanalan                       Thanalan                       Northern Thanalan             
12:42:34.885 | INF [HousingInv]   411 9               La Noscea                      La Noscea                      Eastern La Noscea             
12:42:34.885 | INF [HousingInv]   412 9               La Noscea                      La Noscea                      Upper La Noscea               
12:42:34.885 | INF [HousingInv]   413 9               La Noscea                      La Noscea                      Western La Noscea             
12:42:34.885 | INF [HousingInv]   414 9               La Noscea                      La Noscea                      Eastern La Noscea             
12:42:34.885 | INF [HousingInv]   415 9               La Noscea                      La Noscea                      Lower La Noscea               
12:42:34.885 | INF [HousingInv]   453 9               La Noscea                      La Noscea                      Western La Noscea             
12:42:34.885 | INF [HousingInv]   454 9               La Noscea                      La Noscea                      Upper La Noscea               
12:42:34.885 | INF [HousingInv]   455 9               Abalathia's Spine              Abalathia's Spine              The Sea of Clouds             
12:42:34.885 | INF [HousingInv]   456 9               Coerthas                       Ishgard                        Ruling Chamber                
12:42:34.885 | INF [HousingInv]   457 9               Coerthas                       Coerthas                       Akh Afah Amphitheatre         
12:42:34.885 | INF [HousingInv]   458 9               Coerthas                       Ishgard                        Foundation                    
12:42:34.885 | INF [HousingInv]   459 9               Abalathia's Spine              Abalathia's Spine              Azys Lla                      
12:42:34.885 | INF [HousingInv]   460 9               Thanalan                       Thanalan                       Halatali                      
12:42:34.885 | INF [HousingInv]   461 9               Abalathia's Spine              Abalathia's Spine              The Sea of Clouds             
12:42:34.885 | INF [HousingInv]   464 9               Dravania                       Dravania                       The Dravanian Forelands       
12:42:34.885 | INF [HousingInv]   465 9               Thanalan                       Thanalan                       Eastern Thanalan              
12:42:34.885 | INF [HousingInv]   466 9               La Noscea                      La Noscea                      Upper La Noscea               
12:42:34.885 | INF [HousingInv]   467 9               Coerthas                       Coerthas                       Coerthas Western Highlands    
12:42:34.885 | INF [HousingInv]   468 9               Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.885 | INF [HousingInv]   469 9               Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.885 | INF [HousingInv]   470 9               Coerthas                       Coerthas                       Coerthas Western Highlands    
12:42:34.885 | INF [HousingInv]   471 9               La Noscea                      La Noscea                      Eastern La Noscea             
12:42:34.885 | INF [HousingInv]   472 9               Coerthas                       Coerthas                       Coerthas Western Highlands    
12:42:34.885 | INF [HousingInv]   473 9               The Black Shroud               The Black Shroud               South Shroud                  
12:42:34.885 | INF [HousingInv]   474 9               La Noscea                      Limsa Lominsa                  Limsa Lominsa Upper Decks     
12:42:34.885 | INF [HousingInv]   475 9               Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.885 | INF [HousingInv]   476 9               Dravania                       Dravania                       The Dravanian Hinterlands     
12:42:34.885 | INF [HousingInv]   477 9               Coerthas                       Coerthas                       Coerthas Western Highlands    
12:42:34.885 | INF [HousingInv]   479 9               Coerthas                       Coerthas                       Coerthas Western Highlands    
12:42:34.885 | INF [HousingInv]   480 9               Mor Dhona                      Mor Dhona                      Mor Dhona                     
12:42:34.885 | INF [HousingInv]   481 9               Dravania                       Dravania                       The Dravanian Forelands       
12:42:34.885 | INF [HousingInv]   482 9               Dravania                       Dravania                       The Dravanian Forelands       
12:42:34.885 | INF [HousingInv]   483 9               Thanalan                       Thanalan                       Northern Thanalan             
12:42:34.885 | INF [HousingInv]   484 9               La Noscea                      La Noscea                      Lower La Noscea               
12:42:34.885 | INF [HousingInv]   485 9               Dravania                       Dravania                       The Dravanian Hinterlands     
12:42:34.885 | INF [HousingInv]   486 9               La Noscea                      La Noscea                      Outer La Noscea               
12:42:34.885 | INF [HousingInv]   487 9               Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.885 | INF [HousingInv]   488 9               Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.885 | INF [HousingInv]   489 9               Coerthas                       Coerthas                       Coerthas Western Highlands    
12:42:34.885 | INF [HousingInv]   490 9               La Noscea                      La Noscea                      Hullbreaker Isle              
12:42:34.885 | INF [HousingInv]   491 9               Thanalan                       Thanalan                       Southern Thanalan             
12:42:34.885 | INF [HousingInv]   492 9               Abalathia's Spine              Abalathia's Spine              The Sea of Clouds             
12:42:34.885 | INF [HousingInv]   493 9               Coerthas                       Coerthas                       Coerthas Western Highlands    
12:42:34.885 | INF [HousingInv]   494 9               Thanalan                       Thanalan                       Eastern Thanalan              
12:42:34.885 | INF [HousingInv]   495 9               La Noscea                      La Noscea                      Lower La Noscea               
12:42:34.885 | INF [HousingInv]   496 9               Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.885 | INF [HousingInv]   497 9               Coerthas                       Coerthas                       Coerthas Western Highlands    
12:42:34.885 | INF [HousingInv]   498 9               Coerthas                       Coerthas                       Coerthas Western Highlands    
12:42:34.885 | INF [HousingInv]   499 9               Coerthas                       Ishgard                        The Pillars                   
12:42:34.885 | INF [HousingInv]   500 9               Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.885 | INF [HousingInv]   501 9               Dravania                       Dravania                       The Churning Mists            
12:42:34.885 | INF [HousingInv]   502 9               Mor Dhona                      Mor Dhona                      Carteneau Flats: Borderland Ruins
12:42:34.885 | INF [HousingInv]   503 9               Dravania                       Dravania                       The Dravanian Hinterlands     
12:42:34.885 | INF [HousingInv]   513 9               Coerthas                       Ishgard                        The Vault                     
12:42:34.885 | INF [HousingInv]   634 9               Othard                         Othard                         Yanxia                        
12:42:34.885 | INF [HousingInv]   640 9               Gyr Abania                     Gyr Abania                     The Fringes                   
12:42:34.885 | INF [HousingInv]   647 9               Gyr Abania                     Gyr Abania                     The Fringes                   
12:42:34.885 | INF [HousingInv]   648 9               Gyr Abania                     Gyr Abania                     The Fringes                   
12:42:34.885 | INF [HousingInv]   657 9               Othard                         Othard                         The Ruby Sea                  
12:42:34.885 | INF [HousingInv]   658 9               Gyr Abania                     Gyr Abania                     The Interdimensional Rift     
12:42:34.885 | INF [HousingInv]   659 9               Gyr Abania                     Rhalgr's Reach                 Rhalgr's Reach                
12:42:34.885 | INF [HousingInv]   664 9               Hingashi                       Kugane                         Kugane                        
12:42:34.885 | INF [HousingInv]   666 9               Thanalan                       Ul'dah                         Ul'dah - Steps of Thal        
12:42:34.885 | INF [HousingInv]   667 9               Hingashi                       Kugane                         Kugane                        
12:42:34.885 | INF [HousingInv]   668 9               Thanalan                       Thanalan                       Eastern Thanalan              
12:42:34.885 | INF [HousingInv]   669 9               Thanalan                       Thanalan                       Southern Thanalan             
12:42:34.885 | INF [HousingInv]   670 9               Gyr Abania                     Gyr Abania                     The Fringes                   
12:42:34.885 | INF [HousingInv]   671 9               Gyr Abania                     Gyr Abania                     The Fringes                   
12:42:34.885 | INF [HousingInv]   672 9               Mor Dhona                      Mor Dhona                      Mor Dhona                     
12:42:34.885 | INF [HousingInv]   673 9               Dravania                       Dravania                       Sohm Al                       
12:42:34.885 | INF [HousingInv]   675 9               La Noscea                      La Noscea                      Western La Noscea             
12:42:34.885 | INF [HousingInv]   676 9               Dravania                       Dravania                       The Great Gubal Library       
12:42:34.885 | INF [HousingInv]   678 9               Gyr Abania                     Gyr Abania                     The Fringes                   
12:42:34.885 | INF [HousingInv]   685 9               Othard                         Othard                         Yanxia                        
12:42:34.885 | INF [HousingInv]   686 9               Gyr Abania                     Gyr Abania                     The Lochs                     
12:42:34.885 | INF [HousingInv]   687 9               Gyr Abania                     Gyr Abania                     The Lochs                     
12:42:34.885 | INF [HousingInv]   699 9               Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.885 | INF [HousingInv]   700 9               Coerthas                       Coerthas                       Foundation                    
12:42:34.885 | INF [HousingInv]   701 9               La Noscea                      La Noscea                      Seal Rock                     
12:42:34.885 | INF [HousingInv]   702 9               Abalathia's Spine              Abalathia's Spine              Aetherochemical Research Facility
12:42:34.885 | INF [HousingInv]   703 9               Gyr Abania                     Gyr Abania                     The Fringes                   
12:42:34.885 | INF [HousingInv]   704 9               The Black Shroud               The Black Shroud               Dalamud's Shadow              
12:42:34.885 | INF [HousingInv]   721 9               The Black Shroud               The Black Shroud               Amdapor Keep                  
12:42:34.885 | INF [HousingInv]   726 9               Othard                         Othard                         The Ruby Sea                  
12:42:34.885 | INF [HousingInv]   757 9               Othard                         Othard                         The Ruby Sea                  
12:42:34.885 | INF [HousingInv]   760 9               Gyr Abania                     Gyr Abania                     The Fringes                   
12:42:34.885 | INF [HousingInv]   781 9               Othard                         Othard                         Reisen Temple Road            
12:42:34.885 | INF [HousingInv]   839 9               The Black Shroud               The Black Shroud               East Shroud                   
12:42:34.885 | INF [HousingInv]   861 9               Norvrandt                      Norvrandt                      Lakeland                      
12:42:34.885 | INF [HousingInv]   862 9               Norvrandt                      Norvrandt                      Lakeland                      
12:42:34.885 | INF [HousingInv]   863 9               Norvrandt                      Norvrandt                      Eulmore                       
12:42:34.885 | INF [HousingInv]   864 9               Norvrandt                      Norvrandt                      Kholusia                      
12:42:34.885 | INF [HousingInv]   865 9               The Black Shroud               Gridania                       Old Gridania                  
12:42:34.885 | INF [HousingInv]   866 9               Coerthas                       Coerthas                       Coerthas Western Highlands    
12:42:34.885 | INF [HousingInv]   867 9               La Noscea                      La Noscea                      Eastern La Noscea             
12:42:34.885 | INF [HousingInv]   868 9               Gyr Abania                     Gyr Abania                     The Peaks                     
12:42:34.885 | INF [HousingInv]   869 9               Norvrandt                      Norvrandt                      Il Mheg                       
12:42:34.885 | INF [HousingInv]   870 9               Norvrandt                      Norvrandt                      Kholusia                      
12:42:34.885 | INF [HousingInv]   871 9               Norvrandt                      Norvrandt                      The Rak'tika Greatwood        
12:42:34.885 | INF [HousingInv]   872 9               Norvrandt                      Norvrandt                      Amh Araeng                    
12:42:34.885 | INF [HousingInv]   142 Trial           Thanalan                       Thanalan                       Halatali                      
12:42:34.885 | INF [HousingInv]   281 Trial           La Noscea                      La Noscea                      The Whorleater                
12:42:34.885 | INF [HousingInv]   292 Trial           Thanalan                       Thanalan                       Bowl of Embers                
12:42:34.885 | INF [HousingInv]   293 Trial           La Noscea                      La Noscea                      The Navel                     
12:42:34.885 | INF [HousingInv]   294 Trial           Coerthas                       Coerthas                       The Howling Eye               
12:42:34.885 | INF [HousingInv]   295 Trial           Thanalan                       Thanalan                       Bowl of Embers                
12:42:34.885 | INF [HousingInv]   296 Trial           La Noscea                      La Noscea                      The Navel                     
12:42:34.885 | INF [HousingInv]   297 Trial           Coerthas                       Coerthas                       The Howling Eye               
12:42:34.885 | INF [HousingInv]   348 Trial           Thanalan                       Thanalan                       Porta Decumana                
12:42:34.885 | INF [HousingInv]   353 Trial           Othard                         Othard                         Kugane Ohashi                 
12:42:34.885 | INF [HousingInv]   354 Trial           Norvrandt                      Norvrandt                      The Dancing Plague            
12:42:34.885 | INF [HousingInv]   359 Trial           La Noscea                      La Noscea                      The Whorleater                
12:42:34.885 | INF [HousingInv]   364 Trial           The Black Shroud               The Black Shroud               Thornmarch                    
12:42:34.885 | INF [HousingInv]   368 Trial           Coerthas                       Coerthas                       The Weeping Saint             
12:42:34.885 | INF [HousingInv]   369 Trial           Thanalan                       Thanalan                       Hall of the Bestiarii         
12:42:34.885 | INF [HousingInv]   374 Trial           The Black Shroud               The Black Shroud               The Striking Tree             
12:42:34.885 | INF [HousingInv]   375 Trial           The Black Shroud               The Black Shroud               The Striking Tree             
12:42:34.885 | INF [HousingInv]   377 Trial           Coerthas                       Coerthas                       Akh Afah Amphitheatre         
12:42:34.885 | INF [HousingInv]   378 Trial           Coerthas                       Coerthas                       Akh Afah Amphitheatre         
12:42:34.885 | INF [HousingInv]   394 Trial           The Black Shroud               The Black Shroud               South Shroud                  
12:42:34.885 | INF [HousingInv]   426 Trial           Mor Dhona                      Mor Dhona                      The Chrysalis                 
12:42:34.885 | INF [HousingInv]   432 Trial           Dravania                       Dravania                       Thok ast Thok                 
12:42:34.885 | INF [HousingInv]   436 Trial           Abalathia's Spine              Abalathia's Spine              The Limitless Blue            
12:42:34.885 | INF [HousingInv]   437 Trial           Abalathia's Spine              Abalathia's Spine              Singularity Reactor           
12:42:34.885 | INF [HousingInv]   446 Trial           Dravania                       Dravania                       Thok ast Thok                 
12:42:34.885 | INF [HousingInv]   447 Trial           Abalathia's Spine              Abalathia's Spine              The Limitless Blue            
12:42:34.885 | INF [HousingInv]   448 Trial           Abalathia's Spine              Abalathia's Spine              Singularity Reactor           
12:42:34.885 | INF [HousingInv]   509 Trial           Ilsabard                       Thavnair                       The Gilded Araya              
12:42:34.885 | INF [HousingInv]   517 Trial           Abalathia's Spine              Abalathia's Spine              Containment Bay S1T7          
12:42:34.885 | INF [HousingInv]   524 Trial           Abalathia's Spine              Abalathia's Spine              Containment Bay S1T7          
12:42:34.885 | INF [HousingInv]   559 Trial           Coerthas                       Coerthas                       Steps of Faith                
12:42:34.885 | INF [HousingInv]   566 Trial           Coerthas                       Coerthas                       Steps of Faith                
12:42:34.885 | INF [HousingInv]   576 Trial           Abalathia's Spine              Abalathia's Spine              Containment Bay P1T6          
12:42:34.885 | INF [HousingInv]   577 Trial           Abalathia's Spine              Abalathia's Spine              Containment Bay P1T6          
12:42:34.885 | INF [HousingInv]   637 Trial           Abalathia's Spine              Abalathia's Spine              Containment Bay Z1T9          
12:42:34.885 | INF [HousingInv]   638 Trial           Abalathia's Spine              Abalathia's Spine              Containment Bay Z1T9          
12:42:34.885 | INF [HousingInv]   674 Trial           Othard                         Othard                         The Blessed Treasury          
12:42:34.885 | INF [HousingInv]   677 Trial           Othard                         Othard                         The Blessed Treasury          
12:42:34.885 | INF [HousingInv]   679 Trial           Gyr Abania                     Gyr Abania                     The Royal Airship Landing     
12:42:34.885 | INF [HousingInv]   719 Trial           Gyr Abania                     Gyr Abania                     Emanation                     
12:42:34.885 | INF [HousingInv]   720 Trial           Gyr Abania                     Gyr Abania                     Emanation                     
12:42:34.885 | INF [HousingInv]   730 Trial           Gyr Abania                     Gyr Abania                     Transparency                  
12:42:34.885 | INF [HousingInv]   746 Trial           Othard                         Othard                         The Jade Stoa                 
12:42:34.886 | INF [HousingInv]   758 Trial           Othard                         Othard                         The Jade Stoa                 
12:42:34.886 | INF [HousingInv]   761 Trial           Othard                         Othard                         The Great Hunt                
12:42:34.886 | INF [HousingInv]   762 Trial           Othard                         Othard                         The Great Hunt                
12:42:34.886 | INF [HousingInv]   778 Trial           Othard                         Othard                         Castrum Fluminis              
12:42:34.886 | INF [HousingInv]   779 Trial           Othard                         Othard                         Castrum Fluminis              
12:42:34.886 | INF [HousingInv]   806 Trial           Othard                         Othard                         Kugane Ohashi                 
12:42:34.886 | INF [HousingInv]   810 Trial           Othard                         Othard                         Hells' Kier                   
12:42:34.886 | INF [HousingInv]   811 Trial           Othard                         Othard                         Hells' Kier                   
12:42:34.886 | INF [HousingInv]   824 Trial           Othard                         Othard                         The Wreath of Snakes          
12:42:34.886 | INF [HousingInv]   825 Trial           Othard                         Othard                         The Wreath of Snakes          
12:42:34.886 | INF [HousingInv]   845 Trial           Norvrandt                      Norvrandt                      The Dancing Plague            
12:42:34.886 | INF [HousingInv]   846 Trial           Norvrandt                      Norvrandt                      The Crown of the Immaculate   
12:42:34.886 | INF [HousingInv]   847 Trial           Norvrandt                      Norvrandt                      The Dying Gasp                
12:42:34.886 | INF [HousingInv]   848 Trial           Norvrandt                      Norvrandt                      The Crown of the Immaculate   
12:42:34.886 | INF [HousingInv]   858 Trial           Norvrandt                      Norvrandt                      The Dancing Plague            
12:42:34.886 | INF [HousingInv]   885 Trial           Norvrandt                      Norvrandt                      The Dying Gasp                
12:42:34.886 | INF [HousingInv]   897 Trial           Gyr Abania                     Gyr Abania                     Cinder Drift                  
12:42:34.886 | INF [HousingInv]   912 Trial           Gyr Abania                     Gyr Abania                     Cinder Drift                  
12:42:34.886 | INF [HousingInv]   913 Trial           ???                                                           Transmission Control          
12:42:34.886 | INF [HousingInv]   922 Trial           Norvrandt                      Norvrandt                      The Seat of Sacrifice         
12:42:34.886 | INF [HousingInv]   923 Trial           Norvrandt                      Norvrandt                      The Seat of Sacrifice         
12:42:34.886 | INF [HousingInv]   934 Trial           Thanalan                       Thanalan                       Castrum Marinum Drydocks      
12:42:34.886 | INF [HousingInv]   935 Trial           Thanalan                       Thanalan                       Castrum Marinum Drydocks      
12:42:34.886 | INF [HousingInv]   950 Trial           ???                                                           G-Savior Deck                 
12:42:34.886 | INF [HousingInv]   951 Trial           ???                                                           G-Savior Deck                 
12:42:34.886 | INF [HousingInv]   992 Trial           The Sea of Stars               The Sea of Stars               The Dark Inside               
12:42:34.886 | INF [HousingInv]   993 Trial           The Sea of Stars               The Sea of Stars               The Dark Inside               
12:42:34.886 | INF [HousingInv]   995 Trial           The Northern Empty             Labyrinthos                    The Mothercrystal             
12:42:34.886 | INF [HousingInv]   996 Trial           The Northern Empty             Labyrinthos                    The Mothercrystal             
12:42:34.886 | INF [HousingInv]   997 Trial           The Sea of Stars               The Sea of Stars               The Final Day                 
12:42:34.886 | INF [HousingInv]   998 Trial           The Sea of Stars               The Sea of Stars               The Final Day                 
12:42:34.886 | INF [HousingInv]  1045 Trial           Thanalan                       Thanalan                       Bowl of Embers                
12:42:34.886 | INF [HousingInv]  1046 Trial           La Noscea                      La Noscea                      The Navel                     
12:42:34.886 | INF [HousingInv]  1047 Trial           Coerthas                       Coerthas                       The Howling Eye               
12:42:34.886 | INF [HousingInv]  1048 Trial           Thanalan                       Thanalan                       Porta Decumana                
12:42:34.886 | INF [HousingInv]  1067 Trial           The Black Shroud               The Black Shroud               Thornmarch                    
12:42:34.886 | INF [HousingInv]   246 12              Thanalan                       Thanalan                       IC-04 Main Bridge             
12:42:34.886 | INF [HousingInv]   247 12              La Noscea                      La Noscea                      Ragnarok Main Bridge          
12:42:34.886 | INF [HousingInv]   338 12              Thanalan                       Thanalan                       Eorzean Subterrane            
12:42:34.886 | INF [HousingInv]   370 12              The Black Shroud               The Black Shroud               Main Bridge                   
12:42:34.886 | INF [HousingInv]   505 12              Dravania                       Dravania                       Alexander                     
12:42:34.886 | INF [HousingInv]   507 12              Abalathia's Spine              Abalathia's Spine              Central Azys Lla              
12:42:34.886 | INF [HousingInv]   553 12              Dravania                       Dravania                       Alexander                     
12:42:34.886 | INF [HousingInv]   570 12              The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.886 | INF [HousingInv]   588 12              Dravania                       Dravania                       Heart of the Creator          
12:42:34.886 | INF [HousingInv]   724 12              Gyr Abania                     Gyr Abania                     The Interdimensional Rift     
12:42:34.886 | INF [HousingInv]   756 12              Gyr Abania                     Gyr Abania                     The Interdimensional Rift     
12:42:34.886 | INF [HousingInv]   780 12              Othard                         Othard                         Heaven-on-High                
12:42:34.886 | INF [HousingInv]   807 12              Gyr Abania                     Gyr Abania                     The Interdimensional Rift     
12:42:34.886 | INF [HousingInv]   812 12              Gyr Abania                     Gyr Abania                     The Interdimensional Rift     
12:42:34.886 | INF [HousingInv]   857 12              Norvrandt                      Norvrandt                      The Core                      
12:42:34.886 | INF [HousingInv]   641 ResidentialZone Hingashi                       Kugane                         Shirogane                     
12:42:34.886 | INF [HousingInv]   340 ResidentialZone The Black Shroud               The Black Shroud               The Lavender Beds             
12:42:34.886 | INF [HousingInv]   339 ResidentialZone La Noscea                      La Noscea                      Mist                          
12:42:34.886 | INF [HousingInv]   341 ResidentialZone Thanalan                       Thanalan                       The Goblet                    
12:42:34.886 | INF [HousingInv]   979 ResidentialZone Coerthas                       Ishgard                        Empyreum                      
12:42:34.886 | INF [HousingInv]   282 Residence       La Noscea                      La Noscea                      Private Cottage - Mist        
12:42:34.886 | INF [HousingInv]   283 Residence       La Noscea                      La Noscea                      Private House - Mist          
12:42:34.886 | INF [HousingInv]   284 Residence       La Noscea                      La Noscea                      Private Mansion - Mist        
12:42:34.886 | INF [HousingInv]   342 Residence       The Black Shroud               The Black Shroud               Private Cottage - The Lavender Beds
12:42:34.886 | INF [HousingInv]   343 Residence       The Black Shroud               The Black Shroud               Private House - The Lavender Beds
12:42:34.886 | INF [HousingInv]   344 Residence       The Black Shroud               The Black Shroud               Private Mansion - The Lavender Beds
12:42:34.886 | INF [HousingInv]   345 Residence       Thanalan                       Thanalan                       Private Cottage - The Goblet  
12:42:34.886 | INF [HousingInv]   346 Residence       Thanalan                       Thanalan                       Private House - The Goblet    
12:42:34.886 | INF [HousingInv]   347 Residence       Thanalan                       Thanalan                       Private Mansion - The Goblet  
12:42:34.886 | INF [HousingInv]   384 Residence       La Noscea                      La Noscea                      Private Chambers - Mist       
12:42:34.886 | INF [HousingInv]   385 Residence       The Black Shroud               The Black Shroud               Private Chambers - The Lavender Beds
12:42:34.886 | INF [HousingInv]   386 Residence       Thanalan                       Thanalan                       Private Chambers - The Goblet 
12:42:34.886 | INF [HousingInv]   423 Residence       La Noscea                      La Noscea                      Company Workshop - Mist       
12:42:34.886 | INF [HousingInv]   424 Residence       Thanalan                       Thanalan                       Company Workshop - The Goblet 
12:42:34.886 | INF [HousingInv]   425 Residence       The Black Shroud               The Black Shroud               Company Workshop - The Lavender Beds
12:42:34.886 | INF [HousingInv]   573 Residence       La Noscea                      La Noscea                      Topmast Apartment Lobby       
12:42:34.886 | INF [HousingInv]   574 Residence       The Black Shroud               The Black Shroud               Lily Hills Apartment Lobby    
12:42:34.886 | INF [HousingInv]   575 Residence       Thanalan                       Thanalan                       Sultana's Breath Apartment Lobby
12:42:34.886 | INF [HousingInv]   608 Residence       La Noscea                      La Noscea                      Topmast Apartment             
12:42:34.886 | INF [HousingInv]   609 Residence       The Black Shroud               The Black Shroud               Lily Hills Apartment          
12:42:34.886 | INF [HousingInv]   610 Residence       Thanalan                       Thanalan                       Sultana's Breath Apartment    
12:42:34.886 | INF [HousingInv]   649 Residence       Hingashi                       Kugane                         Private Cottage - Shirogane   
12:42:34.886 | INF [HousingInv]   650 Residence       Hingashi                       Kugane                         Private House - Shirogane     
12:42:34.886 | INF [HousingInv]   651 Residence       Hingashi                       Kugane                         Private Mansion - Shirogane   
12:42:34.886 | INF [HousingInv]   652 Residence       Hingashi                       Kugane                         Private Chambers - Shirogane  
12:42:34.886 | INF [HousingInv]   653 Residence       Hingashi                       Kugane                         Company Workshop - Shirogane  
12:42:34.886 | INF [HousingInv]   654 Residence       Hingashi                       Kugane                         Kobai Goten Apartment Lobby   
12:42:34.886 | INF [HousingInv]   655 Residence       Hingashi                       Kugane                         Kobai Goten Apartment         
12:42:34.886 | INF [HousingInv]   980 Residence       Coerthas                       Ishgard                        Private Cottage - Empyreum    
12:42:34.886 | INF [HousingInv]   981 Residence       Coerthas                       Ishgard                        Private House - Empyreum      
12:42:34.886 | INF [HousingInv]   982 Residence       Coerthas                       Ishgard                        Private Mansion - Empyreum    
12:42:34.886 | INF [HousingInv]   983 Residence       Coerthas                       Ishgard                        Private Chambers - Empyreum   
12:42:34.886 | INF [HousingInv]   984 Residence       Coerthas                       Ishgard                        Company Workshop - Empyreum   
12:42:34.886 | INF [HousingInv]   985 Residence       Coerthas                       Ishgard                        Ingleside Apartment Lobby     
12:42:34.886 | INF [HousingInv]   999 Residence       Coerthas                       Ishgard                        Ingleside Apartment           
12:42:34.886 | INF [HousingInv]   198 Interior        La Noscea                      Limsa Lominsa                  Command Room                  
12:42:34.886 | INF [HousingInv]   204 Interior        The Black Shroud               Gridania                       Seat of the First Bow         
12:42:34.886 | INF [HousingInv]   205 Interior        The Black Shroud               Gridania                       Lotus Stand                   
12:42:34.886 | INF [HousingInv]   210 Interior        Thanalan                       Ul'dah                         Heart of the Sworn            
12:42:34.886 | INF [HousingInv]   212 Interior        Thanalan                       Thanalan                       The Waking Sands              
12:42:34.886 | INF [HousingInv]   335 Interior        Mor Dhona                      Mor Dhona                      Mor Dhona                     
12:42:34.886 | INF [HousingInv]   351 Interior        Mor Dhona                      Mor Dhona                      The Rising Stones             
12:42:34.886 | INF [HousingInv]   395 Interior        Coerthas                       Coerthas                       Intercessory                  
12:42:34.886 | INF [HousingInv]   427 Interior        Coerthas                       Ishgard                        Saint Endalim's Scholasticate 
12:42:34.886 | INF [HousingInv]   428 Interior        Coerthas                       Ishgard                        Seat of the Lord Commander    
12:42:34.886 | INF [HousingInv]   433 Interior        Coerthas                       Ishgard                        Fortemps Manor                
12:42:34.886 | INF [HousingInv]   439 Interior        Coerthas                       Ishgard                        The Lightfeather Proving Grounds
12:42:34.886 | INF [HousingInv]   440 Interior        Coerthas                       Ishgard                        Ruling Chamber                
12:42:34.886 | INF [HousingInv]   462 Interior        Dravania                       Dravania                       Sacrificial Chamber           
12:42:34.886 | INF [HousingInv]   463 Interior        Dravania                       Dravania                       Matoya's Cave                 
12:42:34.886 | INF [HousingInv]   567 Interior        Abalathia's Spine              Abalathia's Spine              The Parrock                   
12:42:34.886 | INF [HousingInv]   568 Interior        Abalathia's Spine              Abalathia's Spine              Leofard's Chambers            
12:42:34.886 | INF [HousingInv]   636 Interior        Mor Dhona                      Mor Dhona                      Omega Control                 
12:42:34.886 | INF [HousingInv]   639 Interior        Hingashi                       Kugane                         Ruby Bazaar Offices           
12:42:34.886 | INF [HousingInv]   681 Interior        Othard                         Othard                         The House of the Fierce       
12:42:34.886 | INF [HousingInv]   682 Interior        Othard                         Othard                         The Doman Enclave             
12:42:34.886 | INF [HousingInv]   735 Interior        ???                                                           The Prima Vista Tiring Room   
12:42:34.886 | INF [HousingInv]   736 Interior        ???                                                           The Prima Vista Bridge        
12:42:34.886 | INF [HousingInv]   737 Interior        Gyr Abania                     Gyr Abania                     Royal Palace                  
12:42:34.886 | INF [HousingInv]   738 Interior        Gyr Abania                     Gyr Abania                     The Resonatorium              
12:42:34.886 | INF [HousingInv]   739 Interior        Othard                         Othard                         The Doman Enclave             
12:42:34.886 | INF [HousingInv]   744 Interior        Othard                         Othard                         Kienkan                       
12:42:34.886 | INF [HousingInv]   764 Interior        Othard                         Othard                         Reisen Temple                 
12:42:34.886 | INF [HousingInv]   786 Interior        Othard                         Othard                         Castrum Fluminis              
12:42:34.886 | INF [HousingInv]   787 Interior        ???                                                           The Ridorana Cataract         
12:42:34.886 | INF [HousingInv]   808 Interior        Gyr Abania                     Gyr Abania                     The Interdimensional Rift     
12:42:34.886 | INF [HousingInv]   828 Interior        ???                                                           The Prima Vista Tiring Room   
12:42:34.886 | INF [HousingInv]   829 Interior        Gyr Abania                     Gyr Abania                     Eorzean Alliance Headquarters 
12:42:34.886 | INF [HousingInv]   842 Interior        Mor Dhona                      Mor Dhona                      The Syrcus Trench             
12:42:34.886 | INF [HousingInv]   844 Interior        Norvrandt                      The Crystarium                 The Ocular                    
12:42:34.886 | INF [HousingInv]   877 Interior        Norvrandt                      Norvrandt                      Lakeland                      
12:42:34.886 | INF [HousingInv]   878 Interior        Norvrandt                      Norvrandt                      The Empty                     
12:42:34.886 | INF [HousingInv]   880 Interior        Norvrandt                      Norvrandt                      The Crown of the Immaculate   
12:42:34.886 | INF [HousingInv]   881 Interior        Norvrandt                      Norvrandt                      The Dying Gasp                
12:42:34.886 | INF [HousingInv]   895 Interior        Norvrandt                      Norvrandt                      Excavation Tunnels            
12:42:34.886 | INF [HousingInv]   896 Interior        Norvrandt                      Norvrandt                      The Copied Factory            
12:42:34.886 | INF [HousingInv]   918 Interior        Norvrandt                      Norvrandt                      Anamnesis Anyder              
12:42:34.886 | INF [HousingInv]   919 Interior        ???                                                           Terncliff                     
12:42:34.886 | INF [HousingInv]   928 Interior        Norvrandt                      Norvrandt                      The Puppets' Bunker           
12:42:34.886 | INF [HousingInv]   931 Interior        Norvrandt                      Norvrandt                      The Seat of Sacrifice         
12:42:34.886 | INF [HousingInv]   964 Interior        ???                                                           The Last Trace                
12:42:34.886 | INF [HousingInv]   967 Interior        Thanalan                       Thanalan                       Castrum Marinum Drydocks      
12:42:34.886 | INF [HousingInv]   971 Interior        Thanalan                       Thanalan                       Lemures Headquarters          
12:42:34.886 | INF [HousingInv]   987 Interior        The Northern Empty             Old Sharlayan                  Main Hall                     
12:42:34.886 | INF [HousingInv]   991 Interior        ???                                                           G-Savior Deck                 
12:42:34.886 | INF [HousingInv]  1001 Interior        Coerthas                       Ishgard                        Strategy Room                 
12:42:34.886 | INF [HousingInv]  1024 Interior        Ilsabard                       Thavnair                       The Nethergate                
12:42:34.886 | INF [HousingInv]  1025 Interior        The World Unsundered           The World Unsundered           The Gates of Pandæmonium      
12:42:34.886 | INF [HousingInv]  1026 Interior        The Sea of Stars               The Sea of Stars               Beyond the Stars              
12:42:34.886 | INF [HousingInv]  1028 Interior        The Sea of Stars               The Sea of Stars               The Dark Inside               
12:42:34.886 | INF [HousingInv]  1029 Interior        The Sea of Stars               The Sea of Stars               The Final Day                 
12:42:34.886 | INF [HousingInv]  1030 Interior        The Northern Empty             Labyrinthos                    The Mothercrystal             
12:42:34.886 | INF [HousingInv]  1053 Interior        Thanalan                       Thanalan                       The Porta Decumana            
12:42:34.886 | INF [HousingInv]  1056 Interior        Ilsabard                       Thavnair                       Alzadaal's Legacy             
12:42:34.886 | INF [HousingInv]  1057 Interior        The Northern Empty             Old Sharlayan                  Restricted Archives           
12:42:34.886 | INF [HousingInv]  1061 Interior        ???                                                           The Omphalos                  
12:42:34.886 | INF [HousingInv]   193 16              Thanalan                       Thanalan                       IC-06 Central Decks           
12:42:34.886 | INF [HousingInv]   194 16              Thanalan                       Thanalan                       IC-06 Regeneration Grid       
12:42:34.886 | INF [HousingInv]   241 16              La Noscea                      La Noscea                      Upper Aetheroacoustic Exploratory Site
12:42:34.886 | INF [HousingInv]   243 16              La Noscea                      La Noscea                      The Ragnarok                  
12:42:34.886 | INF [HousingInv]   245 16              La Noscea                      La Noscea                      Ragnarok Central Core         
12:42:34.886 | INF [HousingInv]   355 16              The Black Shroud               The Black Shroud               Dalamud's Shadow              
12:42:34.886 | INF [HousingInv]   356 16              The Black Shroud               The Black Shroud               The Outer Coil                
12:42:34.886 | INF [HousingInv]   357 16              The Black Shroud               The Black Shroud               Central Decks                 
12:42:34.886 | INF [HousingInv]   380 16              The Black Shroud               The Black Shroud               Dalamud's Shadow              
12:42:34.886 | INF [HousingInv]   381 16              The Black Shroud               The Black Shroud               The Outer Coil                
12:42:34.886 | INF [HousingInv]   382 16              The Black Shroud               The Black Shroud               Central Decks                 
12:42:34.886 | INF [HousingInv]   442 16              Dravania                       Dravania                       The Fist of the Father        
12:42:34.886 | INF [HousingInv]   443 16              Dravania                       Dravania                       The Cuff of the Father        
12:42:34.886 | INF [HousingInv]   444 16              Dravania                       Dravania                       The Arm of the Father         
12:42:34.886 | INF [HousingInv]   449 16              Dravania                       Dravania                       The Fist of the Father        
12:42:34.886 | INF [HousingInv]   450 16              Dravania                       Dravania                       The Cuff of the Father        
12:42:34.886 | INF [HousingInv]   451 16              Dravania                       Dravania                       The Arm of the Father         
12:42:34.886 | INF [HousingInv]   520 16              Dravania                       Dravania                       The Fist of the Son           
12:42:34.886 | INF [HousingInv]   521 16              Dravania                       Dravania                       The Cuff of the Son           
12:42:34.886 | INF [HousingInv]   522 16              Dravania                       Dravania                       The Arm of the Son            
12:42:34.886 | INF [HousingInv]   529 16              Dravania                       Dravania                       The Fist of the Son           
12:42:34.886 | INF [HousingInv]   531 16              Dravania                       Dravania                       The Arm of the Son            
12:42:34.886 | INF [HousingInv]   580 16              Dravania                       Dravania                       Eyes of the Creator           
12:42:34.886 | INF [HousingInv]   581 16              Dravania                       Dravania                       Breath of the Creator         
12:42:34.886 | INF [HousingInv]   582 16              Dravania                       Dravania                       Heart of the Creator          
12:42:34.886 | INF [HousingInv]   584 16              Dravania                       Dravania                       Eyes of the Creator           
12:42:34.886 | INF [HousingInv]   585 16              Dravania                       Dravania                       Breath of the Creator         
12:42:34.886 | INF [HousingInv]   586 16              Dravania                       Dravania                       Heart of the Creator          
12:42:34.886 | INF [HousingInv]   195 17              Thanalan                       Thanalan                       IC-06 Main Bridge             
12:42:34.886 | INF [HousingInv]   196 17              Thanalan                       Thanalan                       The Burning Heart             
12:42:34.886 | INF [HousingInv]   242 17              La Noscea                      La Noscea                      Lower Aetheroacoustic Exploratory Site
12:42:34.886 | INF [HousingInv]   244 17              La Noscea                      La Noscea                      Ragnarok Drive Cylinder       
12:42:34.886 | INF [HousingInv]   358 17              The Black Shroud               The Black Shroud               The Holocharts                
12:42:34.886 | INF [HousingInv]   383 17              The Black Shroud               The Black Shroud               The Holocharts                
12:42:34.886 | INF [HousingInv]   445 17              Dravania                       Dravania                       The Burden of the Father      
12:42:34.886 | INF [HousingInv]   452 17              Dravania                       Dravania                       The Burden of the Father      
12:42:34.886 | INF [HousingInv]   523 17              Dravania                       Dravania                       The Burden of the Son         
12:42:34.886 | INF [HousingInv]   530 17              Dravania                       Dravania                       The Cuff of the Son           
12:42:34.886 | INF [HousingInv]   532 17              Dravania                       Dravania                       The Burden of the Son         
12:42:34.886 | INF [HousingInv]   583 17              Dravania                       Dravania                       Soul of the Creator           
12:42:34.886 | INF [HousingInv]   587 17              Dravania                       Dravania                       Soul of the Creator           
12:42:34.886 | INF [HousingInv]   691 17              Gyr Abania                     Gyr Abania                     Deltascape V1.0               
12:42:34.886 | INF [HousingInv]   692 17              Gyr Abania                     Gyr Abania                     Deltascape V2.0               
12:42:34.886 | INF [HousingInv]   693 17              Gyr Abania                     Gyr Abania                     Deltascape V3.0               
12:42:34.886 | INF [HousingInv]   694 17              Gyr Abania                     Gyr Abania                     Deltascape V4.0               
12:42:34.886 | INF [HousingInv]   695 17              Gyr Abania                     Gyr Abania                     Deltascape V1.0               
12:42:34.886 | INF [HousingInv]   696 17              Gyr Abania                     Gyr Abania                     Deltascape V2.0               
12:42:34.886 | INF [HousingInv]   697 17              Gyr Abania                     Gyr Abania                     Deltascape V3.0               
12:42:34.886 | INF [HousingInv]   698 17              Gyr Abania                     Gyr Abania                     Deltascape V4.0               
12:42:34.886 | INF [HousingInv]   733 17              ???                                                           The Binding Coil of Bahamut   
12:42:34.886 | INF [HousingInv]   748 17              Gyr Abania                     Gyr Abania                     Sigmascape V1.0               
12:42:34.886 | INF [HousingInv]   749 17              Gyr Abania                     Gyr Abania                     Sigmascape V2.0               
12:42:34.886 | INF [HousingInv]   750 17              Gyr Abania                     Gyr Abania                     Sigmascape V3.0               
12:42:34.886 | INF [HousingInv]   751 17              Gyr Abania                     Gyr Abania                     Sigmascape V4.0               
12:42:34.886 | INF [HousingInv]   752 17              Gyr Abania                     Gyr Abania                     Sigmascape V1.0               
12:42:34.886 | INF [HousingInv]   753 17              Gyr Abania                     Gyr Abania                     Sigmascape V2.0               
12:42:34.886 | INF [HousingInv]   754 17              Gyr Abania                     Gyr Abania                     Sigmascape V3.0               
12:42:34.886 | INF [HousingInv]   755 17              Gyr Abania                     Gyr Abania                     Sigmascape V4.0               
12:42:34.886 | INF [HousingInv]   777 17              ???                                                           Ultimacy                      
12:42:34.886 | INF [HousingInv]   798 17              Gyr Abania                     Gyr Abania                     Psiscape V1.0                 
12:42:34.886 | INF [HousingInv]   799 17              Gyr Abania                     Gyr Abania                     Psiscape V2.0                 
12:42:34.886 | INF [HousingInv]   800 17              Gyr Abania                     Gyr Abania                     The Interdimensional Rift     
12:42:34.886 | INF [HousingInv]   801 17              Gyr Abania                     Gyr Abania                     The Interdimensional Rift     
12:42:34.886 | INF [HousingInv]   802 17              Gyr Abania                     Gyr Abania                     Psiscape V1.0                 
12:42:34.886 | INF [HousingInv]   803 17              Gyr Abania                     Gyr Abania                     Psiscape V2.0                 
12:42:34.886 | INF [HousingInv]   804 17              Gyr Abania                     Gyr Abania                     The Interdimensional Rift     
12:42:34.886 | INF [HousingInv]   805 17              Gyr Abania                     Gyr Abania                     The Interdimensional Rift     
12:42:34.886 | INF [HousingInv]   849 17              Norvrandt                      Norvrandt                      The Core                      
12:42:34.886 | INF [HousingInv]   850 17              Norvrandt                      Norvrandt                      The Halo                      
12:42:34.886 | INF [HousingInv]   851 17              Norvrandt                      Norvrandt                      The Nereus Trench             
12:42:34.886 | INF [HousingInv]   852 17              Norvrandt                      Norvrandt                      Atlas Peak                    
12:42:34.886 | INF [HousingInv]   853 17              Norvrandt                      Norvrandt                      The Core                      
12:42:34.886 | INF [HousingInv]   854 17              Norvrandt                      Norvrandt                      The Halo                      
12:42:34.887 | INF [HousingInv]   855 17              Norvrandt                      Norvrandt                      The Nereus Trench             
12:42:34.887 | INF [HousingInv]   856 17              Norvrandt                      Norvrandt                      Atlas Peak                    
12:42:34.887 | INF [HousingInv]   887 17              Dravania                       Dravania                       Liminal Space                 
12:42:34.887 | INF [HousingInv]   902 17              Norvrandt                      Norvrandt                      The Gandof Thunder Plains     
12:42:34.887 | INF [HousingInv]   903 17              Norvrandt                      Norvrandt                      Ashfall                       
12:42:34.887 | INF [HousingInv]   904 17              Norvrandt                      Norvrandt                      The Halo                      
12:42:34.887 | INF [HousingInv]   905 17              Norvrandt                      Norvrandt                      Great Glacier                 
12:42:34.887 | INF [HousingInv]   906 17              Norvrandt                      Norvrandt                      The Gandof Thunder Plains     
12:42:34.887 | INF [HousingInv]   907 17              Norvrandt                      Norvrandt                      Ashfall                       
12:42:34.887 | INF [HousingInv]   908 17              Norvrandt                      Norvrandt                      The Halo                      
12:42:34.887 | INF [HousingInv]   909 17              Norvrandt                      Norvrandt                      Great Glacier                 
12:42:34.887 | INF [HousingInv]   942 17              Norvrandt                      Norvrandt                      Sphere of Naught              
12:42:34.887 | INF [HousingInv]   943 17              Norvrandt                      Norvrandt                      Laxan Loft                    
12:42:34.887 | INF [HousingInv]   944 17              Norvrandt                      Norvrandt                      Bygone Gaol                   
12:42:34.887 | INF [HousingInv]   945 17              Norvrandt                      Norvrandt                      The Garden of Nowhere         
12:42:34.887 | INF [HousingInv]   946 17              Norvrandt                      Norvrandt                      Sphere of Naught              
12:42:34.887 | INF [HousingInv]   947 17              Norvrandt                      Norvrandt                      Laxan Loft                    
12:42:34.887 | INF [HousingInv]   948 17              Norvrandt                      Norvrandt                      Bygone Gaol                   
12:42:34.887 | INF [HousingInv]   949 17              Norvrandt                      Norvrandt                      The Garden of Nowhere         
12:42:34.887 | INF [HousingInv]   968 17              Coerthas                       Coerthas                       Medias Res                    
12:42:34.887 | INF [HousingInv]  1002 17              The World Unsundered           The World Unsundered           The Gates of Pandæmonium      
12:42:34.887 | INF [HousingInv]  1003 17              The World Unsundered           The World Unsundered           The Gates of Pandæmonium      
12:42:34.887 | INF [HousingInv]  1004 17              The World Unsundered           The World Unsundered           The Stagnant Limbo            
12:42:34.887 | INF [HousingInv]  1005 17              The World Unsundered           The World Unsundered           The Stagnant Limbo            
12:42:34.887 | INF [HousingInv]  1006 17              The World Unsundered           The World Unsundered           The Fervid Limbo              
12:42:34.887 | INF [HousingInv]  1007 17              The World Unsundered           The World Unsundered           The Fervid Limbo              
12:42:34.887 | INF [HousingInv]  1008 17              The World Unsundered           The World Unsundered           The Sanguine Limbo            
12:42:34.887 | INF [HousingInv]  1009 17              The World Unsundered           The World Unsundered           The Sanguine Limbo            
12:42:34.887 | INF [HousingInv]   376 18              Mor Dhona                      ウルヴズジェイル                       Carteneau Flats: Borderland Ruins
12:42:34.887 | INF [HousingInv]   431 18              La Noscea                      ウルヴズジェイル                       Seal Rock                     
12:42:34.887 | INF [HousingInv]   554 18              Coerthas                       ウルヴズジェイル                       The Fields of Glory           
12:42:34.887 | INF [HousingInv]   888 18              Othard                         ウルヴズジェイル                       Onsal Hakair                  
12:42:34.887 | INF [HousingInv]   388 19              Thanalan                       ゴールドソーサー                       Chocobo Square                
12:42:34.887 | INF [HousingInv]   389 20              La Noscea                      ゴールドソーサー                       Chocobo Square                
12:42:34.887 | INF [HousingInv]   390 20              Thanalan                       ゴールドソーサー                       Chocobo Square                
12:42:34.887 | INF [HousingInv]   391 20              The Black Shroud               ゴールドソーサー                       Chocobo Square                
12:42:34.887 | INF [HousingInv]   417 20              Thanalan                       ゴールドソーサー                       Chocobo Square                
12:42:34.887 | INF [HousingInv]   886 Firmament       Coerthas                       Ishgard                        The Firmament                 
12:42:34.887 | INF [HousingInv]   392 Sanctum         The Black Shroud               The Black Shroud               Sanctum of the Twelve         
12:42:34.887 | INF [HousingInv]   393 Sanctum         The Black Shroud               The Black Shroud               Sanctum of the Twelve         
12:42:34.887 | INF [HousingInv]   144 23              Thanalan                       ゴールドソーサー                       The Gold Saucer               
12:42:34.887 | INF [HousingInv]   506 25              Thanalan                       ゴールドソーサー                       Chocobo Square                
12:42:34.887 | INF [HousingInv]   589 25              Thanalan                       ゴールドソーサー                       Chocobo Square                
12:42:34.887 | INF [HousingInv]   590 25              Thanalan                       ゴールドソーサー                       Chocobo Square                
12:42:34.887 | INF [HousingInv]   591 25              Thanalan                       ゴールドソーサー                       Chocobo Square                
12:42:34.887 | INF [HousingInv]   512 Diadem          Abalathia's Spine              Abalathia's Spine              The Diadem                    
12:42:34.887 | INF [HousingInv]   514 Diadem          Abalathia's Spine              Abalathia's Spine              The Diadem                    
12:42:34.887 | INF [HousingInv]   515 Diadem          Abalathia's Spine              Abalathia's Spine              The Diadem                    
12:42:34.887 | INF [HousingInv]   656 Diadem          Abalathia's Spine              Abalathia's Spine              The Diadem                    
12:42:34.887 | INF [HousingInv]   537 Fold            La Noscea                      La Noscea                      The Fold                      
12:42:34.887 | INF [HousingInv]   538 Fold            La Noscea                      La Noscea                      The Fold                      
12:42:34.887 | INF [HousingInv]   539 Fold            La Noscea                      La Noscea                      The Fold                      
12:42:34.887 | INF [HousingInv]   540 Fold            La Noscea                      La Noscea                      The Fold                      
12:42:34.887 | INF [HousingInv]   541 Fold            La Noscea                      La Noscea                      The Fold                      
12:42:34.887 | INF [HousingInv]   542 Fold            La Noscea                      La Noscea                      The Fold                      
12:42:34.887 | INF [HousingInv]   543 Fold            La Noscea                      La Noscea                      The Fold                      
12:42:34.887 | INF [HousingInv]   544 Fold            La Noscea                      La Noscea                      The Fold                      
12:42:34.887 | INF [HousingInv]   545 Fold            La Noscea                      La Noscea                      The Fold                      
12:42:34.887 | INF [HousingInv]   546 Fold            La Noscea                      La Noscea                      The Fold                      
12:42:34.887 | INF [HousingInv]   547 Fold            La Noscea                      La Noscea                      The Fold                      
12:42:34.887 | INF [HousingInv]   548 Fold            La Noscea                      La Noscea                      The Fold                      
12:42:34.887 | INF [HousingInv]   549 Fold            La Noscea                      La Noscea                      The Fold                      
12:42:34.887 | INF [HousingInv]   550 Fold            La Noscea                      La Noscea                      The Fold                      
12:42:34.887 | INF [HousingInv]   551 Fold            La Noscea                      La Noscea                      The Fold                      
12:42:34.887 | INF [HousingInv]   552 Fold            La Noscea                      La Noscea                      Western La Noscea             
12:42:34.887 | INF [HousingInv]   149 28              La Noscea                      ウルヴズジェイル                       The Feasting Grounds          
12:42:34.887 | INF [HousingInv]  1032 28              ???                            ウルヴズジェイル                       The Palaistra                 
12:42:34.887 | INF [HousingInv]  1033 28              ???                            ウルヴズジェイル                       The Volcanic Heart            
12:42:34.887 | INF [HousingInv]  1034 28              ???                            ウルヴズジェイル                       Cloud Nine                    
12:42:34.887 | INF [HousingInv]   403 29              Gyr Abania                     Gyr Abania                     Ala Mhigo                     
12:42:34.887 | INF [HousingInv]   533 29              Coerthas                       Coerthas                       Coerthas Central Highlands    
12:42:34.887 | INF [HousingInv]   560 29              Abalathia's Spine              Abalathia's Spine              Aetherochemical Research Facility
12:42:34.887 | INF [HousingInv]   592 29              Thanalan                       Thanalan                       Bowl of Embers                
12:42:34.887 | INF [HousingInv]   633 29              Mor Dhona                      Mor Dhona                      Carteneau Flats: Borderland Ruins
12:42:34.887 | INF [HousingInv]   665 29              Hingashi                       Kugane                         Kugane                        
12:42:34.887 | INF [HousingInv]   684 29              Gyr Abania                     Gyr Abania                     The Lochs                     
12:42:34.887 | INF [HousingInv]   688 29              Othard                         Othard                         The Azim Steppe               
12:42:34.887 | INF [HousingInv]   690 29              Gyr Abania                     Gyr Abania                     The Interdimensional Rift     
12:42:34.887 | INF [HousingInv]   705 29              Thanalan                       Thanalan                       Ul'dah - Steps of Thal        
12:42:34.887 | INF [HousingInv]   706 29              Thanalan                       Thanalan                       Ul'dah - Steps of Thal        
12:42:34.887 | INF [HousingInv]   707 29              Mor Dhona                      Mor Dhona                      The Weeping City of Mhach     
12:42:34.887 | INF [HousingInv]   708 29              La Noscea                      La Noscea                      Rhotano Sea                   
12:42:34.887 | INF [HousingInv]   709 29              Coerthas                       Coerthas                       Coerthas Western Highlands    
12:42:34.887 | INF [HousingInv]   710 29              Hingashi                       Kugane                         Kugane                        
12:42:34.887 | INF [HousingInv]   711 29              Othard                         Othard                         The Ruby Sea                  
12:42:34.887 | INF [HousingInv]   713 29              Othard                         Othard                         The Azim Steppe               
12:42:34.887 | INF [HousingInv]   714 29              Othard                         Othard                         Bardam's Mettle               
12:42:34.887 | INF [HousingInv]   715 29              Dravania                       Dravania                       The Churning Mists            
12:42:34.887 | INF [HousingInv]   716 29              Gyr Abania                     Gyr Abania                     The Peaks                     
12:42:34.887 | INF [HousingInv]   717 29              La Noscea                      La Noscea                      Wolves' Den Pier              
12:42:34.887 | INF [HousingInv]   718 29              Othard                         Othard                         The Azim Steppe               
12:42:34.887 | INF [HousingInv]   722 29              The Black Shroud               The Black Shroud               The Lost City of Amdapor      
12:42:34.887 | INF [HousingInv]   723 29              Othard                         Othard                         The Azim Steppe               
12:42:34.887 | INF [HousingInv]   769 29              Othard                         Othard                         The Burn                      
12:42:34.887 | INF [HousingInv]   797 29              Othard                         Othard                         The Azim Steppe               
12:42:34.887 | INF [HousingInv]   830 29              Gyr Abania                     Gyr Abania                     The Ghimlyt Dark              
12:42:34.887 | INF [HousingInv]   834 29              Coerthas                       Coerthas                       The Howling Eye               
12:42:34.887 | INF [HousingInv]   859 29              Norvrandt                      Norvrandt                      The Confessional of Toupasa the Elder
12:42:34.887 | INF [HousingInv]   860 29              Norvrandt                      Norvrandt                      Amh Araeng                    
12:42:34.887 | INF [HousingInv]   873 29              Norvrandt                      Norvrandt                      The Dancing Plague            
12:42:34.887 | INF [HousingInv]   874 29              Norvrandt                      Norvrandt                      The Rak'tika Greatwood        
12:42:34.887 | INF [HousingInv]   875 29              Norvrandt                      Norvrandt                      The Rak'tika Greatwood        
12:42:34.887 | INF [HousingInv]   876 29              Norvrandt                      Norvrandt                      The Nabaath Mines             
12:42:34.887 | INF [HousingInv]   893 29              ???                                                           The Imperial Palace           
12:42:34.887 | INF [HousingInv]   894 29              Norvrandt                      Norvrandt                      Lyhe Mheg                     
12:42:34.887 | INF [HousingInv]   911 29              ???                                                           Cid's Memory                  
12:42:34.887 | INF [HousingInv]   914 29              Norvrandt                      Norvrandt                      Trial's Threshold             
12:42:34.887 | INF [HousingInv]   925 29              ???                                                           Terncliff Bay                 
12:42:34.887 | INF [HousingInv]   926 29              ???                                                           Terncliff Bay                 
12:42:34.887 | INF [HousingInv]   932 29              Norvrandt                      Norvrandt                      The Tempest                   
12:42:34.887 | INF [HousingInv]   954 29              La Noscea                      La Noscea                      The Navel                     
12:42:34.887 | INF [HousingInv]   955 29              ???                                                           The Last Trace                
12:42:34.887 | INF [HousingInv]   977 29              Mor Dhona                      Mor Dhona                      Carteneau Flats: Borderland Ruins
12:42:34.887 | INF [HousingInv]  1010 29              Ilsabard                       Thavnair                       Magna Glacies                 
12:42:34.887 | INF [HousingInv]  1011 29              Ilsabard                       Garlemald                      Garlemald                     
12:42:34.887 | INF [HousingInv]  1012 29              Ilsabard                       Thavnair                       Magna Glacies                 
12:42:34.887 | INF [HousingInv]  1013 29              The Sea of Stars               The Sea of Stars               Beyond the Stars              
12:42:34.887 | INF [HousingInv]  1014 29              The World Unsundered           The World Unsundered           Elpis                         
12:42:34.887 | INF [HousingInv]  1015 29              The Black Shroud               The Black Shroud               Central Shroud                
12:42:34.887 | INF [HousingInv]  1016 29              La Noscea                      La Noscea                      Sastasha                      
12:42:34.887 | INF [HousingInv]  1017 29              Othard                         Othard                         The Swallow's Compass         
12:42:34.887 | INF [HousingInv]  1018 29              Coerthas                       Ishgard                        The Vault                     
12:42:34.887 | INF [HousingInv]  1019 29              Gyr Abania                     Gyr Abania                     The Peaks                     
12:42:34.887 | INF [HousingInv]  1020 29              Thanalan                       Thanalan                       Cutter's Cry                  
12:42:34.887 | INF [HousingInv]  1021 29              Coerthas                       Coerthas                       Dusk Vigil                    
12:42:34.887 | INF [HousingInv]  1022 29              Dravania                       Dravania                       Saint Mocianne's Arboretum    
12:42:34.887 | INF [HousingInv]  1023 29              Dravania                       Dravania                       The Dravanian Forelands       
12:42:34.887 | INF [HousingInv]  1049 29              Thanalan                       Thanalan                       Western Thanalan              
12:42:34.887 | INF [HousingInv]  1051 29              Ilsabard                       Thavnair                       The Tower of Babil            
12:42:34.887 | INF [HousingInv]  1052 29              Thanalan                       Thanalan                       The Porta Decumana            
12:42:34.887 | INF [HousingInv]  1068 29              Coerthas                       Coerthas                       Steps of Faith                
12:42:34.887 | INF [HousingInv]   534 Barracks        The Black Shroud               Gridania                       Twin Adder Barracks           
12:42:34.887 | INF [HousingInv]   535 Barracks        Thanalan                       Ul'dah                         Flame Barracks                
12:42:34.887 | INF [HousingInv]   536 Barracks        La Noscea                      Limsa Lominsa                  Maelstrom Barracks            
12:42:34.887 | INF [HousingInv]   561 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   562 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   563 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   564 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   565 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   593 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   594 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   595 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   596 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   597 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   598 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   599 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   600 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   601 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   602 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   603 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   604 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   605 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   606 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   607 DeepDungeon     The Black Shroud               The Black Shroud               The Palace of the Dead        
12:42:34.887 | INF [HousingInv]   770 DeepDungeon     Othard                         Othard                         Heaven-on-High                
12:42:34.887 | INF [HousingInv]   771 DeepDungeon     Othard                         Othard                         Heaven-on-High                
12:42:34.887 | INF [HousingInv]   772 DeepDungeon     Othard                         Othard                         Heaven-on-High                
12:42:34.887 | INF [HousingInv]   773 DeepDungeon     Othard                         Othard                         Heaven-on-High                
12:42:34.887 | INF [HousingInv]   774 DeepDungeon     Othard                         Othard                         Heaven-on-High                
12:42:34.887 | INF [HousingInv]   775 DeepDungeon     Othard                         Othard                         Heaven-on-High                
12:42:34.887 | INF [HousingInv]   782 DeepDungeon     Othard                         Othard                         Heaven-on-High                
12:42:34.887 | INF [HousingInv]   783 DeepDungeon     Othard                         Othard                         Heaven-on-High                
12:42:34.887 | INF [HousingInv]   784 DeepDungeon     Othard                         Othard                         Heaven-on-High                
12:42:34.887 | INF [HousingInv]   785 DeepDungeon     Othard                         Othard                         Heaven-on-High                
12:42:34.887 | INF [HousingInv]   504 EventLocale     Thanalan                       Thanalan                       The Eighteenth Floor          
12:42:34.887 | INF [HousingInv]   611 EventLocale     Thanalan                       Ul'dah                         Frondale's Home for Friendless Foundlings
12:42:34.887 | INF [HousingInv]   809 EventLocale     The Black Shroud               The Black Shroud               Haunted Manor                 
12:42:34.887 | INF [HousingInv]   558 TreasureHunt    Coerthas                       Coerthas                       The Aquapolis                 
12:42:34.887 | INF [HousingInv]   712 TreasureHunt    ???                                                           The Lost Canals of Uznair     
12:42:34.887 | INF [HousingInv]   725 TreasureHunt    ???                                                           The Lost Canals of Uznair     
12:42:34.887 | INF [HousingInv]   794 TreasureHunt    ???                                                           The Shifting Altars of Uznair 
12:42:34.887 | INF [HousingInv]   879 TreasureHunt    Norvrandt                      Norvrandt                      The Dungeons of Lyhe Ghiah    
12:42:34.887 | INF [HousingInv]   924 TreasureHunt    Norvrandt                      Norvrandt                      The Shifting Oubliettes of Lyhe Ghiah
12:42:34.887 | INF [HousingInv]  1000 TreasureHunt    The Sea of Stars               The Sea of Stars               The Excitatron 6000           
12:42:34.887 | INF [HousingInv]   571 34              The Black Shroud               The Black Shroud               Haunted Manor                 
12:42:34.887 | INF [HousingInv]   741 34              The Black Shroud               The Black Shroud               Sanctum of the Twelve         
12:42:34.887 | INF [HousingInv]   994 34              The Black Shroud               The Black Shroud               The Phantoms' Feast           
12:42:34.887 | INF [HousingInv]   579 35              Thanalan                       ゴールドソーサー                       The Battlehall                
12:42:34.887 | INF [HousingInv]  1058 37              ???                            ウルヴズジェイル                       The Palaistra                 
12:42:34.887 | INF [HousingInv]  1059 37              ???                            ウルヴズジェイル                       The Volcanic Heart            
12:42:34.887 | INF [HousingInv]  1060 37              ???                            ウルヴズジェイル                       Cloud Nine                    
12:42:34.887 | INF [HousingInv]   624 38              Abalathia's Spine              Abalathia's Spine              The Diadem                    
12:42:34.887 | INF [HousingInv]   625 38              Abalathia's Spine              Abalathia's Spine              The Diadem                    
12:42:34.887 | INF [HousingInv]   729 39              Dravania                       ウルヴズジェイル                       Astragalos                    
12:42:34.887 | INF [HousingInv]   791 39              Thanalan                       ウルヴズジェイル                       Hidden Gorge                  
12:42:34.887 | INF [HousingInv]   728 40              ???                                                           Mordion Gaol                  
12:42:34.887 | INF [HousingInv]   921 40              Thanalan                       Ul'dah                         Frondale's Home for Friendless Foundlings
12:42:34.887 | INF [HousingInv]   732 Eureka          ???                                                           Eureka Anemos                 
12:42:34.887 | INF [HousingInv]   763 Eureka          ???                                                           Eureka Pagos                  
12:42:34.887 | INF [HousingInv]   795 Eureka          ???                                                           Eureka Pyros                  
12:42:34.887 | INF [HousingInv]   827 Eureka          ???                                                           Eureka Hydatos                
12:42:34.887 | INF [HousingInv]   790 43              Thanalan                       Ul'dah                         Ul'dah - Steps of Nald        
12:42:34.887 | INF [HousingInv]   792 44              Thanalan                       ゴールドソーサー                       The Fall of Belah'dia         
12:42:34.887 | INF [HousingInv]   899 44              La Noscea                      ゴールドソーサー                       The Falling City of Nym       
12:42:34.887 | INF [HousingInv]   796 45              Thanalan                       Ul'dah                         Blue Sky                      
12:42:34.887 | INF [HousingInv]   900 46              The High Seas                                                 The Endeavor                  
12:42:34.887 | INF [HousingInv]   901 47              Abalathia's Spine              Abalathia's Spine              The Diadem                    
12:42:34.887 | INF [HousingInv]   929 47              Abalathia's Spine              Abalathia's Spine              The Diadem                    
12:42:34.887 | INF [HousingInv]   939 47              Abalathia's Spine              Abalathia's Spine              The Diadem                    
12:42:34.887 | INF [HousingInv]   920 48              ???                                                           Bozjan Southern Front         
12:42:34.887 | INF [HousingInv]   975 48              ???                                                           Zadnor                        
12:42:34.887 | INF [HousingInv]  1055 49              ???                                                           Unnamed Island                
12:42:34.887 | INF [HousingInv]   940 50              Thanalan                       ゴールドソーサー                       The Battlehall                
12:42:34.887 | INF [HousingInv]   941 51              Thanalan                       ゴールドソーサー                       The Battlehall                
12:42:34.887 | INF [HousingInv]   936 52              ???                                                           Delubrum Reginae              
12:42:34.887 | INF [HousingInv]   937 53              ???                                                           Delubrum Reginae              
12:42:34.887 | INF [HousingInv]  1027 54              The Sea of Stars               The Sea of Stars               Ultima Thule                  
12:42:34.887 | INF [HousingInv]  1031 54              The World Unsundered           The World Unsundered           Propylaion                    
{ } [ Send ]

 */
// ReSharper restore CommentTypo
