/*
 * Created by SharpDevelop.
 * User: 510628
 * Date: 6-6-2020
 * Time: 08:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;

namespace NesCom
{
	/// <summary>
	/// Description of cpu.
	/// </summary>
	public class cpu
	{
		public int ProgramCounter = 0;
		public bool IsRunning = true;
		int InstructionLength;
		rom ROM;
		
		
		public void RunROM(rom SelfROM)
		{
			//Load ROM
			ROM = SelfROM;
							ProgramCounter = ROM.HeaderSize;
			//Run ROM
			IsRunning = true;
			byte CurrentByte;
			while (IsRunning)
			{
				//Get byte at Program Counter
				CurrentByte = ROM.GetByte(ProgramCounter);
				
				//Turn Byte into Instruction
				switch(CurrentByte)
				{
					case 0x78:
						SEIInstruction(CurrentByte);
						break;
					case 0xA9:
						LDAInstruction(CurrentByte);
						break;
					case 0xD8:
						SEIInstruction(CurrentByte);
						break;	
						
						
						
					default:
             			 Debug.WriteLine("OpCode not implemented!!!");
             			 throw new NotImplementedException("OpCode not implemented!!!");
              			 break;
				}
					
				ProgramCounter += InstructionLength;
			}
			
			
			
			
		}
		
		
		public void LDAInstruction(byte Instruction)
		{
			InstructionLength = 2;
			Debug.WriteLine("LDA # - Identifier byte: " + Instruction);
		}
		
		public void SEIInstruction(byte Instruction)
		{
			InstructionLength = 1;
			Debug.WriteLine("SEI - Identifier byte: " + Instruction);
		}
		
		public void CLDInstruction(byte Instruction)
		{
			InstructionLength = 1;
			Debug.WriteLine("CLD # - Identifier byte: " + Instruction);
		}
	}
}
