/*
 * Created by SharpDevelop.
 * User: 510628
 * Date: 6-6-2020
 * Time: 09:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;

namespace NesCom
{
	/// <summary>
	/// Description of rom.
	/// </summary>
	public class rom
	{
		public byte HeaderSize = 16;
		int KBSize = 1024;
		public byte NumOfCHRBlocks;
		public byte NumOfPRGBlocks;
		public byte[] ROMBytes;
		public byte[] PRGBytes;
			
		public void Init(byte[] ROMData)
		{
			 NumOfPRGBlocks = ROMData[4];
			 NumOfCHRBlocks = ROMData[5];
			 ROMBytes = ROMData;
			 PRGBytes = ROMData.Skip(16).Take(HeaderSize * KBSize * NumOfPRGBlocks).ToArray();
		}
		
		public byte GetByte(int ProgramCounter)
		{
			//Gets byte at position
			return ROMBytes[ProgramCounter];
		}
	}
}
