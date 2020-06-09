/*
 * Created by SharpDevelop.
 * User: 510628
 * Date: 6-6-2020
 * Time: 16:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;

namespace NesCom
{
	/// <summary>
	/// Description of ram.
	/// </summary>
	public class ram
	{
		int KB = 1024;
		public Byte[] Memory;
		
		public void Init()
		{
			Memory = new byte[65536];
			
			for (int i = 0; i < 255; i++) 
			{
  				Memory[256 + i] = 0xFF;
			}
		}
		
		public byte GetByte(int Position)
		{
			//Gets byte at position
			return Memory[Position];
		}
		
		public UInt16 GetByte16(UInt16 Position)
		{
			//Gets byte at position
			byte[] Value = { Memory[Position], Memory[Position+1] };
			UInt16 ByteValue = BitConverter.ToUInt16(Value, 0);
			return ByteValue;
		}
		
		public void SetByte(int Position, byte Value)
		{
			//Sets byte at position
			Memory[Position] = Value;
			return;
		}
	}
}
