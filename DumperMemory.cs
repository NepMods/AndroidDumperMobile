using System;
using System.Linq;
using System.Diagnostics;
using Memory;
using System.Collections.Generic;

namespace Memory
{
	public class DumperMemory
	{
		private int _pid = 0;
		private byte[] _metadata;
		private byte[] _il2cpp;
		private Int64 metaDataAddr;
		private Int64 il2cppAddr;
		
	    public int Pid { get { return _pid; } }
        public byte[] Metadata {get{return _metadata;}}
	    public byte[] Il2Cpp {get{return _il2cpp;}}
	
		public DumperMemory(string package)
		{
			int _pid = Ultis.GetProcessId(package);
			if (_pid == 0)
				throw new Exception("Not found pid");
		}


		public void setFixSanity(UInt32 Sanity)
		{
			byte[] values = BitConverter.GetBytes(Sanity);
			if (BitConverter.ToInt32(_metadata,0) != Sanity)
				Array.Copy(values,0,_metadata,0,values.Length);
		}
		
		public void SetFiles(string region_metadata,string region_il2cpp)
		{
			_metadata = Ultis.GetMemoryDD(_pid,region_metadata, out metaDataAddr);
			_il2cpp = Ultis.GetMemoryDD(_pid,region_il2cpp, out il2cppAddr);
		}
		
		static void setTextColor(string txt, ConsoleColor color)
		{

			Console.ForegroundColor = color;
			Console.WriteLine(txt);
			Console.ResetColor();
		}

		public static DumpInfo GetMemoryFiles(string package, string metadata_region, string binary_region)
		{
			DumpInfo dumpInfo = new DumpInfo()
			{
				Binary = null,
				Metadata = null,
				DumpAddress = 0
			};

			long maddress; long baddress;
			int pid = Ultis.GetProcessId(package);
			if (pid == 0)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("O pacote não foi encontrado!");
				Console.ResetColor();
				return dumpInfo;
			}

			dumpInfo.Metadata = Ultis.GetMemoryDD(pid, metadata_region, out maddress);
			setTextColor(" [OK] Metadata.", ConsoleColor.Green);
			dumpInfo.Binary = Ultis.GetMemoryDD(pid, binary_region, out baddress);
			setTextColor(" [OK] Binary.", ConsoleColor.Green);
			dumpInfo.DumpAddress = baddress;

			if (dumpInfo.Metadata != null)
			{
				dumpInfo.Metadata[0] = 0xAF; dumpInfo.Metadata[1] = 0x1B;
				dumpInfo.Metadata[2] = 0xB1; dumpInfo.Metadata[3] = 0xFA;
			}

			return dumpInfo;
		}

	}


}