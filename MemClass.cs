using System;
using System.Linq;
using System.Collections.Generic;

namespace Memory
{
	public class MapInfo
	{
		#region Private
		private string pPathName = string.Empty;
		private string pName = string.Empty;
		private long saddr = 0;
		private long eaddr = 0;
		#endregion

		public string Package { get { return pName; } }
		public string Path { get { return pPathName; } }
		public long StartAddress { get { return saddr; } }
		public long EndAddress { get { return eaddr; } }

		public MapInfo(string package, string path, long startAddress, long endAddress)
		{
			pName = package;
			pPathName = path;
			saddr = startAddress;
			eaddr = endAddress;
		}

		public override string ToString()
		{
			return string.Format("Name:{0}\nPath:{1}\nStartAddress:0x{2}\nEndAddress:0x{3}\nSize:{4}", pName, pPathName, saddr.ToString("X2"), eaddr.ToString("X2"), eaddr - saddr);
		}
	}

	public class MapsInfo : List<MapInfo>
	{
		public long Size { get { return this.Last().EndAddress - this.First().StartAddress; } }
		public long StartAddress {get{return this.First().StartAddress;}}
		public long EndAddress {get{return this.Last().StartAddress;}}
	}
	
	public struct DumpInfo
	{
		public byte[] Metadata {get;set;}
		public byte[] Binary {get;set;}
		public long DumpAddress {get;set;}
	}
}