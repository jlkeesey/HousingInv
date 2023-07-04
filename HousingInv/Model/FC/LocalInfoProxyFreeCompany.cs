using FFXIVClientStructs.Attributes;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.Interop.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HousingInv.Model.FC;

[StructLayout(LayoutKind.Explicit, Size = 0x6E8)]
public unsafe struct LocalInfoProxyFreeCompany
{
    [FieldOffset(0x00)] public InfoProxyInterface InfoProxyInterface;
    [FieldOffset(0x20)] public void* Unk20; //Low address probably high in hierarchy
    [FieldOffset(0x30)] public ulong ID;
    [FieldOffset(0x46)] public ushort HomeWorldID;
    [FieldOffset(0x42)] public byte What01;
    [FieldOffset(0x43)] public byte What02;
    [FieldOffset(0x44)] public byte What03;
    [FieldOffset(0x45)] public byte What04;
    [FieldOffset(0x50)] public byte What05;
    [FieldOffset(0x52)] public byte What06;
    [FieldOffset(0x54)] public byte What07;
    [FieldOffset(0x67)] public byte What08;
    [FieldOffset(0x68)] public byte What09;
    [FieldOffset(0x69)] public GrandCompany GrandCompany;
    [FieldOffset(0x6B)] public byte Rank;
    [FieldOffset(0x70)] public CrestData Crest;
    [FieldOffset(0x78)] public ushort OnlineMembers;
    [FieldOffset(0x7A)] public ushort TotalMembers;
    [FieldOffset(0x7C)] public fixed byte Name[22];
    [FieldOffset(0x93)] public fixed byte Master[60];
    [FieldOffset(0xD0)] public Utf8String UnkD0;
    [FieldOffset(0x138)] public byte ActiveListItemNum; //0=Topics, 1 = Members, ....
    [FieldOffset(0x139)] public byte MemberTabIndex;
    [FieldOffset(0x13E)] public byte InfoTabIndex;
    [FixedSizeArray<InfoProxyFreeCompany.RankData>(14)]
    [FieldOffset(0x178)] public fixed byte RankArray[14 * 0x58];
    //668 after
}
