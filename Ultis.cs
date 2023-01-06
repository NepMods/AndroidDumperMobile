using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Memory
{
	public class Ultis
	{
		public static string GetProcessName(string ProcessName)
		{
			int sIndex = ProcessName.IndexOf("/");
			int eIndex = ProcessName.LastIndexOf("/");
			return ProcessName.Substring(eIndex + 1, ProcessName.Length - eIndex - 1);
		}

		public static string GetProcessPath(string ProcessName)
		{
			int sIndex = ProcessName.IndexOf("/");
			int eIndex = ProcessName.LastIndexOf("/");
			return ProcessName.Substring(sIndex + 1, ProcessName.Length - sIndex - 1);
		}

		public static Process RunShellRoot(string cmd)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo("su", $"-c  \u0022{cmd}\u0022");
			startInfo.RedirectStandardOutput = true;
			startInfo.UseShellExecute = false;
			return Process.Start(startInfo);
		}

		public static int GetProcessId(string package)
		{

			Process process = RunShellRoot($"pidof {package}");
			string result = process.StandardOutput.ReadLine();
			if (result == string.Empty)
			{
				return 0;
			}
			return int.Parse(result);
		}

		public static MapsInfo parseMaps(int pid, string libname)
		{

			MapsInfo mapsInfos = new MapsInfo();
			var line = string.Empty;
			Process Shell = RunShellRoot($"cat /proc/{pid}/maps");
			var StandardOutput = Shell.StandardOutput;
			while ((line = StandardOutput.ReadLine()) != null)
			{
				if (line.Contains(libname))
				{
					string name = Ultis.GetProcessName(line);
					string path = Ultis.GetProcessPath(line);
					string[] address = line.Split("-");
					long saddress = (long)Convert.ToUInt64(address[0], 16);
					long eaddress = (long)Convert.ToUInt64(address[1].Split(" ")[0], 16);
					var mapInfo = new MapInfo(name, path,
					saddress, eaddress);
					mapsInfos.Add(mapInfo);
				}
			}
			return mapsInfos;
		}

		public static byte[] GetMemoryDD(int pid, string region, out long DumpAddress)
		{
			byte[] result = null;
			MapsInfo mapInfos = parseMaps(pid, region);
			string tmp_file = "/data/local/tmp/dump.bin";
			string line = string.Empty;
			var output = RunShellRoot($"dd if=/proc/{pid}/mem of={tmp_file} bs=1024 count={mapInfos.Size / 1024} skip={mapInfos.StartAddress / 1024}").StandardOutput;
			while((line = output.ReadLine()) != null){
				Console.WriteLine(line);
			}
			if (File.Exists(tmp_file))
			{
				result = File.ReadAllBytes(tmp_file);
				File.Delete(tmp_file);
			}
			DumpAddress = mapInfos.StartAddress;
			return result;
		}

	}
}