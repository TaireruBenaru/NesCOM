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
		rom ROM;
		
		//Status Register (Store a single Byte)
		public StatusFlags Status_Register; //Status Register
		
		//Counter Registers (Store a single Byte)
		public UInt16 PC_Register; //Program Counter Register
		public byte SP_Register; //Stack Pointer Register
		
		
		//Data Registers (Store a single Byte)
		public  byte X_Register; //X Register
		public  byte Y_Register; //Y Register
		public  byte A_Register; //A Register
		
		public bool IsRunning = true;
		
		byte InstructionLength;
		
		[Flags]
		public enum StatusFlags
		{
			C = (1 << 0), //Carry Bit
			Z = (1 << 1), //Zero
			I = (1 << 2), //Disable Interrupts
			D = (1 << 3), //Decimal Mode
			B = (1 << 4), //Break
			U = (1 << 5), //Unused
			V = (1 << 6), //Overflow
			N = (1 << 7) //Negative
		};
		
		
		public void PowerUp()
		{
			//Set the initial value of CPU Registers.
			//Status = 0x34 (IRQ Disabled)
			//X, Y & A = 0x0
			//SP = 0xFD
			//$4017 = 0 (Frame IRQ Disabled)
			//$4015 = 0 (Sound Channels Disabled)
			//$4000-$400F = 0 (Sound Registers)
			
			Status_Register = (StatusFlags)0x34;
			X_Register = 0x0;
			Y_Register = 0x0;
			A_Register = 0x0;
			SP_Register = 0xFD;
	}
		
		public void RunROM(rom SelfROM)
		{
			//Load ROM
			ROM = SelfROM;
			PC_Register = ROM.HeaderSize;
			//Run ROM
			IsRunning = true;
			byte CurrentByte;
			while (IsRunning)
			{
				//Get byte at Program Counter
				CurrentByte = ROM.GetByte(PC_Register);
				
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
						CLDInstruction(CurrentByte);
						break;	
						
						
						
					default:
             			 Debug.WriteLine("OpCode not implemented!!!");
             			 throw new NotImplementedException("OpCode not implemented!!!");
              			 break;
				}
					
				PC_Register += InstructionLength;
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
