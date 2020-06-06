/*
 * Created by SharpDevelop.
 * User: 510628
 * Date: 6-6-2020
 * Time: 16:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

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
			Memory = new byte[KB * 2];
		}
		
		public byte GetByte(int Position)
		{
			//Gets byte at position
			return Memory[Position];
		}
		
		public void SetByte(int Position, byte Value)
		{
			//Gets byte at position
			Memory[Position] = Value;
			return;
		}
	}
}
