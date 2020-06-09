/*
 * Created by SharpDevelop.
 * User: 510628
 * Date: 6-6-2020
 * Time: 08:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Diagnostics;

namespace NesCom
{
	/// <summary>
	/// Description of cpu.
	/// </summary>
	public class cpu
	{
		rom ROM;
		ram RAM;
		
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
			//Status = 0x24 (IRQ Disabled)
			//X, Y & A = 0x0
			//SP = 0xFD
			//$4017 = 0 (Frame IRQ Disabled)
			//$4015 = 0 (Sound Channels Disabled)
			//$4000-$400F = 0 (Sound Registers)
			
			Status_Register = (StatusFlags)0x24;
			X_Register = 0x0;
			Y_Register = 0x0;
			A_Register = 0x0;
			SP_Register = 0xFD;
	}
		
		public void RunROM(rom SelfROM, ram SelfRAM)
		{
			//Load ROM
			ROM = SelfROM;
			RAM = SelfRAM;
			PC_Register = 0x8000;
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
					case 0x0:
						BRKInstruction(CurrentByte);
						break;
					case 0x10:
						BPLInstruction(CurrentByte);
						break;
					case 0x18:
						CLCInstruction(CurrentByte);
						break;	
					case 0x20:
						JSRInstruction(CurrentByte);
						break;
					case 0x24:
						BITZePgInstruction(CurrentByte);
						break;
					case 0x30:
						BMIInstruction(CurrentByte);
						break;
					case 0x38:
						SECInstruction(CurrentByte);
						break;	
					case 0x45:
						EORZePgInstruction(CurrentByte);
						break;
					case 0x4C:
						JMPAbsInstruction(CurrentByte);
						break;
					case 0x50:
						BVCInstruction(CurrentByte);
						break;
					case 0x58:
						CLIInstruction(CurrentByte);
						break;
					case 0x60:
						RTSInstruction(CurrentByte);
						break;
					case 0x69:
						ADCImmInstruction(CurrentByte);
						break;
					case 0x70:
						BVSInstruction(CurrentByte);
						break;
					case 0x78:
						SEIInstruction(CurrentByte);
						break;
					case 0x85:
						StaZePgInstruction(CurrentByte);
						break;
					case 0x86:
						StxZePgInstruction(CurrentByte);
						break;
					case 0x8D:
						StaAbsInstruction(CurrentByte);
						break;
					case 0x8E:
						StxAbsInstruction(CurrentByte);
						break;
					case 0x90:
						BCCInstruction(CurrentByte);
						break;
					case 0x9A:
						TXSInstruction(CurrentByte);
						break;
					case 0xA0:
						LDYImmInstruction(CurrentByte);
						break;
					case 0xA2:
						LDXImmInstruction(CurrentByte);
						break;
					case 0xA5:
						LdaZePgInstruction(CurrentByte);
						break;
					case 0xA9:
						LDAImmInstruction(CurrentByte);
						break;
					case 0xAA:
						TAXInstruction(CurrentByte);
						break;
					case 0xAD:
						LdaAbsInstruction(CurrentByte);
						break;
					case 0xB0:
						BCSInstruction(CurrentByte);
						break;
					case 0xB8:
						CLVInstruction(CurrentByte);
						break;
					case 0xBD:
						LdaAbsXInstruction(CurrentByte);
						break;
					case 0xC0:
						CPYImmInstruction(CurrentByte);
						break;
					case 0xC9:
						CMPImmInstruction(CurrentByte);
						break;
					case 0xCA:
						DEXInstruction(CurrentByte);
						break;
					case 0xE0:
						CPXImmInstruction(CurrentByte);
						break;
					case 0xEA:
						NOPInstruction(CurrentByte);
						break;
					case 0xED:
						INXInstruction(CurrentByte);
						break;
					case 0xD0:
						BNEInstruction(CurrentByte);
						break;	
					case 0xD8:
						CLDInstruction(CurrentByte);
						break;	
					case 0xF0:
						BEQInstruction(CurrentByte);
						break;
					case 0xF8:
						SEDInstruction(CurrentByte);
						break;	
					
						
						
						
					default:
             			 Debug.WriteLine("OpCode not implemented!!!");
             			 throw new NotImplementedException("OpCode not implemented!!!");
				}
					
				PC_Register += InstructionLength;
				Debug.WriteLine("PC Register: " + PC_Register.ToString("X"));
			}
			
			
			
			
		}
		
		
		public void PushToStack16(UInt16 Data)
		{
			byte hi = (byte)((Data >> 8) & 0xFF);
			byte lo = (byte)(Data & 0xFF);
            PushToStack(hi);
            PushToStack(lo);
		}
		
		public void PushToStack(byte Data)
		{
            
			RAM.SetByte((0x100 | SP_Register), Data);
			unchecked
			{
				SP_Register--;
			}
		}
		
		public UInt16 PullFromStack16()
        {
            byte lo = PullFromStack();
            byte hi = PullFromStack();
            
            UInt16 Data = (UInt16)((hi << 8) | lo);
            
            return Data;
        }
		
		byte PullFromStack()
        {
            unchecked
			{
				SP_Register--;
			}
            byte data = RAM.GetByte((UInt16)(0x0100 | SP_Register));
            return data;
        }
		
		public void ADCImmInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			byte Fetched = ROM.GetByte(PC_Register+1);
			int Carry = Status_Register.HasFlag(StatusFlags.C) ? 1 : 0;
			
			byte Sum = (byte)(A_Register + Fetched + Carry);
			
			A_Register = Sum;
			
			if(A_Register == 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.Z;
			}
			
			if( (A_Register & (1 << 7-1)) != 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.N;
			}
			
			if((~(A_Register ^ Fetched) & (A_Register ^ Sum) & 0x80) != 0)
			{
				Status_Register |=(StatusFlags)StatusFlags.V;
			}
			
			if((A_Register + Fetched + Carry) > 0xFF)
			{
				Status_Register |=(StatusFlags)StatusFlags.C;
			}
			
			Debug.WriteLine("ADC #" + Fetched + " - Identifier byte: " + Instruction);
		}
		
		public void BRKInstruction(byte Instruction)
        {
		 	InstructionLength = 1;
            PushToStack16(PC_Register);
            Status_Register |= (StatusFlags)StatusFlags.B;
            PushToStack((byte)Status_Register);
            Status_Register &= (StatusFlags)StatusFlags.B;
            PC_Register = RAM.GetByte16(0xFFFE);
            Debug.WriteLine("BRK - Identifier byte: " + Instruction);
        }
		
		public void BPLInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			byte ByteData = ROM.GetByte(PC_Register+1);
			UInt16 MemoryAddress = (UInt16)(PC_Register + ByteData);
			
			if (SP_Register >= 0)
			{
				PC_Register = MemoryAddress;
			}
		Debug.WriteLine("BPL $" + (MemoryAddress + 2).ToString("X") + " - Identifier byte: " + Instruction);
		}
		
		public void BMIInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			byte ByteData = ROM.GetByte(PC_Register+1);
			UInt16 MemoryAddress = (UInt16)(PC_Register + ByteData);
			
			if (Status_Register.HasFlag(StatusFlags.V))
			{
				PC_Register = MemoryAddress;
			}
		Debug.WriteLine("BMI $" + (MemoryAddress + 2).ToString("X") + " - Identifier byte: " + Instruction);
		}
		
		public void BCSInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			byte ByteData = ROM.GetByte(PC_Register+1);
			UInt16 MemoryAddress = (UInt16)(PC_Register + ByteData);
			
			if (Status_Register.HasFlag(StatusFlags.C))
			{
				PC_Register = MemoryAddress;
			}
			Debug.WriteLine("BCS $" + (MemoryAddress + 2).ToString("X") + " - Identifier byte: " + Instruction);
		}
		
		public void BCCInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			byte ByteData = ROM.GetByte(PC_Register+1);
			UInt16 MemoryAddress = (UInt16)(PC_Register + ByteData);
			
			if (!Status_Register.HasFlag(StatusFlags.C))
			{
				PC_Register = MemoryAddress;
			}
			Debug.WriteLine("BCC $" + (MemoryAddress + 2).ToString("X") + " - Identifier byte: " + Instruction);
		}
		
		public void BNEInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			byte ByteData = ROM.GetByte(PC_Register+1);
			UInt16 MemoryAddress = (UInt16)(PC_Register + ByteData);
			
			if (!Status_Register.HasFlag(StatusFlags.Z))
			{
				PC_Register = MemoryAddress;
			}
			Debug.WriteLine("BNE $" + (MemoryAddress + 2).ToString("X") + " - Identifier byte: " + Instruction);
		}
		
		public void BEQInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			byte ByteData = ROM.GetByte(PC_Register+1);
			UInt16 MemoryAddress = (UInt16)(PC_Register + ByteData);
			
			if (Status_Register.HasFlag(StatusFlags.Z))
			{
				PC_Register = MemoryAddress;
			}
			Debug.WriteLine("BEQ $" + (MemoryAddress + 2).ToString("X") + " - Identifier byte: " + Instruction);
		}
		
		public void BVSInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			byte ByteData = ROM.GetByte(PC_Register+1);
			UInt16 MemoryAddress = (UInt16)(PC_Register + ByteData);
			
			if (Status_Register.HasFlag(StatusFlags.V))
			{
				PC_Register = MemoryAddress;
			}
			Debug.WriteLine("BVS $" + (MemoryAddress + 2).ToString("X") + " - Identifier byte: " + Instruction);
		}
		
		public void BVCInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			byte ByteData = ROM.GetByte(PC_Register+1);
			UInt16 MemoryAddress = (UInt16)(PC_Register + ByteData);
			
			if (!Status_Register.HasFlag(StatusFlags.V))
			{
				PC_Register = MemoryAddress;
			}
			Debug.WriteLine("BVC $" + (MemoryAddress + 2).ToString("X") + " - Identifier byte: " + Instruction);
		}
		
		public void LDAImmInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			//Load value into Accumulator Register
			byte Value = ROM.GetByte(PC_Register+1);
			A_Register = Value;
			
			if(A_Register == 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.Z;
			}
			if( (A_Register & (1 << 7-1)) != 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.N;
			}
			
			Debug.WriteLine("LDA #" + Value + " - Identifier byte: " + Instruction);
		}
		
		public void LdaAbsInstruction(byte Instruction)
		{
			InstructionLength = 3;
			
			//Get value from Memory and put it in the A Register
			byte[] ByteData = { ROM.GetByte(PC_Register+2), ROM.GetByte(PC_Register+1)};
			int MemoryAddress = BitConverter.ToUInt16(ByteData, 0);
			
			byte Value = RAM.GetByte(MemoryAddress);
			A_Register = (byte)Value;
			
			if(A_Register == 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.Z;
			}
			if( (A_Register & (1 << 7-1)) != 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.N;
			}
			
			Debug.WriteLine("LDA $" + BitConverter.ToString(ByteData).Replace("-", string.Empty) + " - Identifier byte: " + Instruction);
		}
		
		
		public void LdaZePgInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			//Put value in X Register in memory
			byte Address = ROM.GetByte(PC_Register+1);
			byte Value = RAM.GetByte(Address);
			
			A_Register = Value;
			
			if(A_Register == 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.Z;
			}
			if( (A_Register & (1 << 7-1)) != 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.N;
			}
			
			Debug.WriteLine("LDA ZP $" + Address + " - Identifier byte: " + Instruction);
		}
		
		public void LdaAbsXInstruction(byte Instruction)
		{
			InstructionLength = 3;
			
			//Get value from Memory and put it in the A Register
			byte[] ByteData = { ROM.GetByte(PC_Register+2), ROM.GetByte(PC_Register+1)};
			int MemoryAddress = BitConverter.ToUInt16(ByteData, 0);
			
			byte Value = RAM.GetByte(MemoryAddress + X_Register);
			A_Register = Value;
			
			if(A_Register == 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.Z;
			}
			if( (A_Register & (1 << 7-1)) != 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.N;
			}
			
			Debug.WriteLine("LDA $" + BitConverter.ToString(ByteData).Replace("-", string.Empty) + ", " + X_Register + "(" + MemoryAddress + X_Register + ")" + " - Identifier byte: " + Instruction);
		}
		
		public void LDXImmInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			//Load value into Accumulator Register
			byte Value = ROM.GetByte(PC_Register+1);
			X_Register = Value;
			
			if(X_Register == 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.Z;
			}
			if( (X_Register & (1 << 7-1)) != 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.N;
			}
			
			Debug.WriteLine("LDX # " + Value + " - Identifier byte: " + Instruction);
		}
		
		public void LDYImmInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			//Load value into Accumulator Register
			byte Value = ROM.GetByte(PC_Register+1);
			Y_Register = Value;
			
			if(Y_Register == 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.Z;
			}
			if((Y_Register & (1 << 7-1)) != 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.N;
			}
			
			Debug.WriteLine("LDY # " + Value + " - Identifier byte: " + Instruction);
		}
		
		public void TXSInstruction(byte Instruction)
		{
			InstructionLength = 1;
			SP_Register = X_Register;
			Debug.WriteLine("TXS - Identifier byte: " + Instruction);
		}
		
		public void SEIInstruction(byte Instruction)
		{
			InstructionLength = 1;
			Status_Register |= (StatusFlags)StatusFlags.I;
			Debug.WriteLine("SEI - Identifier byte: " + Instruction);
		}
		
		public void SECInstruction(byte Instruction)
		{
			InstructionLength = 1;
			Status_Register |= (StatusFlags)StatusFlags.C;
			Debug.WriteLine("SEC - Identifier byte: " + Instruction);
		}
		
		public void SEDInstruction(byte Instruction)
		{
			InstructionLength = 1;
			Status_Register |= (StatusFlags)StatusFlags.D;
			Debug.WriteLine("SED - Identifier byte: " + Instruction);
		}
		
		public void CLDInstruction(byte Instruction)
		{
			InstructionLength = 1;
			
			//Clear the Decimal Flag
			Status_Register &= (StatusFlags)StatusFlags.D;
			Debug.WriteLine("CLD - Identifier byte: " + Instruction);
		}
		
		public void CLCInstruction(byte Instruction)
		{
			InstructionLength = 1;
			
			//Clear the Carry Flag
			Status_Register &= (StatusFlags)StatusFlags.C;
			Debug.WriteLine("CLC - Identifier byte: " + Instruction);
		}
		
		public void CLIInstruction(byte Instruction)
		{
			InstructionLength = 1;
			
			//Clear the Interrupt Flag
			Status_Register &= (StatusFlags)StatusFlags.I;
			Debug.WriteLine("CLC - Identifier byte: " + Instruction);
		}
		
		public void CLVInstruction(byte Instruction)
		{
			InstructionLength = 1;
			
			//Clear the Overflow Flag
			Status_Register &= (StatusFlags)StatusFlags.C;
			Debug.WriteLine("CLC - Identifier byte: " + Instruction);
		}
		
		public void StaAbsInstruction(byte Instruction)
		{
			InstructionLength = 3;
			
			//Put value in A Register in memory
			byte[] ByteData = { ROM.GetByte(PC_Register+2), ROM.GetByte(PC_Register+1)};
			int MemoryAddress = BitConverter.ToInt16(ByteData, 0);
			RAM.SetByte(MemoryAddress, A_Register);
			
			Debug.WriteLine("STA $" + BitConverter.ToString(ByteData).Replace("-", string.Empty) + " - Identifier byte: " + Instruction);
		}
		
		public void StaZePgInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			//Put value in X Register in memory
			byte Address = ROM.GetByte(PC_Register+1);
			RAM.SetByte(Address, A_Register);
			
			Debug.WriteLine("STA $" + Address + " - Identifier byte: " + Instruction);
		}
		
		public void StxAbsInstruction(byte Instruction)
		{
			InstructionLength = 3;
			
			//Put value in X Register in memory
			byte[] ByteData = { ROM.GetByte(PC_Register+2), ROM.GetByte(PC_Register+1)};
			int MemoryAddress = BitConverter.ToInt16(ByteData, 0);
			RAM.SetByte(MemoryAddress, X_Register);
			
			Debug.WriteLine("STX $" + BitConverter.ToString(ByteData).Replace("-", string.Empty) + " - Identifier byte: " + Instruction);
		}
		
		public void StxZePgInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			//Put value in X Register in memory
			byte Address = ROM.GetByte(PC_Register+1);
			RAM.SetByte(Address, X_Register);
			
			Debug.WriteLine("STX $" + Address + " - Identifier byte: " + Instruction);
		}
		
		public void JMPAbsInstruction(byte Instruction)
		{
			InstructionLength = 0;
			
			byte[] ByteData = { ROM.GetByte(PC_Register+2), ROM.GetByte(PC_Register+1)};
			string temp = "0x" + BitConverter.ToString(ByteData).Replace("-", string.Empty);
			UInt16 MemoryAddress = Convert.ToUInt16(temp, 16);
			PC_Register = (UInt16)MemoryAddress;
						
			Debug.WriteLine("JMP $" + BitConverter.ToString(ByteData).Replace("-", string.Empty) + " - Identifier byte: " + Instruction);
		}
		
		public void JSRInstruction(byte Instruction)
		{
			InstructionLength = 0;
			
			byte[] ByteData = { ROM.GetByte(PC_Register+2), ROM.GetByte(PC_Register+1)};
			string temp = "0x" + BitConverter.ToString(ByteData).Replace("-", string.Empty);
			UInt16 MemoryAddress = Convert.ToUInt16(temp, 16);
			PushToStack16((UInt16)(PC_Register - 1));
			PC_Register = MemoryAddress;
			Debug.WriteLine("JSR $" + BitConverter.ToString(ByteData).Replace("-", string.Empty) + "- Identifier byte: " + Instruction);
		}
		
		public void RTSInstruction(byte Instruction)
		{
			InstructionLength = 0;
			
			UInt16 StackValue = (UInt16)(PullFromStack16() + 1);
			PC_Register = StackValue;

			Debug.WriteLine("RTS - Identifier byte: " + Instruction);
		}
		
		public void INXInstruction(byte Instruction)
		{
			InstructionLength = 1;
			X_Register++;
			
			if(X_Register == 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.Z;
			}
			if((X_Register & (1 << 7-1)) != 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.N;
			}
			
			Debug.WriteLine("INX - Identifier byte: " + Instruction);
		}
		
		public void DEXInstruction(byte Instruction)
		{
			InstructionLength = 1;
			X_Register--;
			
			if(X_Register == 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.Z;
			}
			if((X_Register & (1 << 7-1)) != 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.N;
			}
			
			Debug.WriteLine("DEX - Identifier byte: " + Instruction);
		}
		
		public void TAXInstruction(byte Instruction)
		{
			InstructionLength = 1;
			X_Register = A_Register;
			
			if(X_Register == 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.Z;
			}
			if((X_Register & (1 << 7-1)) != 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.N;
			}
			Debug.WriteLine("TAX - Identifier byte: " + Instruction);
		}
		
		public void BITZePgInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			byte Value = ROM.GetByte(PC_Register+1);
			string temp = "0x" + Value.ToString("X");
			UInt16 MemoryAddress = Convert.ToUInt16(temp, 16);
			byte Data = RAM.GetByte(MemoryAddress);
			
			
			if((Data & (1 << 7-1)) != 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.N;
			}
			else if((Data & (1 << 7-1)) == 0)
			{
				Status_Register &= (StatusFlags)StatusFlags.N;
			}
			
			if((Data & (1 << 6-1)) != 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.V;
			}
			else if((Data & (1 << 6-1)) == 0)
			{
				Status_Register &= (StatusFlags)StatusFlags.V;
			}
			
			if((Data & A_Register) == 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.Z;
			}
			
			Debug.WriteLine("BIT $" + Value.ToString("X") + "- Identifier byte: " + Instruction);
		}
			
		
		public void EORZePgInstruction(byte Instruction)
		{
			InstructionLength = 2;
			
			//Load value into Accumulator Register
			byte Value = ROM.GetByte(PC_Register+1);
			A_Register = (byte)(A_Register ^ Value);
			
			if(A_Register == 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.Z;
			}
			if( (A_Register & (1 << 7-1)) != 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.N;
			}
			
			Debug.WriteLine("EOR #" + Value + " - Identifier byte: " + Instruction);
		}
		
		public void ORAZIndrXInstruction(byte Instruction)
		{
			
			//TODO: Implement ORA (Zero Page, Indirect, X)
			InstructionLength = 2;
			
			//Load value into Accumulator Register
			byte Value = ROM.GetByte(PC_Register+1);
			A_Register = (byte)(A_Register | Value);
			
			if(A_Register == 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.Z;
			}
			if( (A_Register & (1 << 7-1)) != 0)
			{
				Status_Register |= (StatusFlags)StatusFlags.N;
			}
			
			Debug.WriteLine("EOR #" + Value + " - Identifier byte: " + Instruction);
		}
		
		public void NOPInstruction(byte Instruction)
		{
			InstructionLength = 1;
			
			Debug.WriteLine("NOP - Identifier byte: " + Instruction);
		}
		
		public void CMPImmInstruction(byte Instruction)
		{
			InstructionLength = 2;
			byte Value = ROM.GetByte(PC_Register+1);
			
			if (A_Register > Value)
			{
				Status_Register |= (StatusFlags)StatusFlags.C;
			}
			
			if(A_Register == Value)
			{
				Status_Register |= (StatusFlags)StatusFlags.Z;
			}
			else
			{
				
			}
						
			Debug.WriteLine("CMP #" + Value + " - Identifier byte: " + Instruction);
		}
		
		public void CPXImmInstruction(byte Instruction)
		{
			InstructionLength = 4;
			byte Value = ROM.GetByte(PC_Register+1);
			
			if (X_Register > Value)
			{
				Status_Register |= (StatusFlags)StatusFlags.C;
			}
			
			if(X_Register == Value)
			{
				Status_Register |= (StatusFlags)StatusFlags.Z;
			}
						
			Debug.WriteLine("CPX #" + Value + " - Identifier byte: " + Instruction);
		}
		
		public void CPYImmInstruction(byte Instruction)
		{
			InstructionLength = 2;
			byte Value = ROM.GetByte(PC_Register+1);
			
			if (Y_Register > Value)
			{
				Status_Register |= (StatusFlags)StatusFlags.C;
			}
			
			if(Y_Register == Value)
			{
				Status_Register |= (StatusFlags)StatusFlags.Z;
			}
			else
			{
				
			}
						
			Debug.WriteLine("CPY #" + Value + " - Identifier byte: " + Instruction);
		}
		
	}
}