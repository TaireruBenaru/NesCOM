/*
 * Created by SharpDevelop.
 * User: 510628
 * Date: 5-6-2020
 * Time: 17:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NesCom
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
						Debug.WriteLine("hello world");
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			
			//TODO: Unhardcode path.
			string path = @"C:\Users\510628\Desktop\SharpDevelop\Projects\NesCom\ROms\Super Mario Bros (E).nes";
			
			rom ROM = new rom();
			cpu CPU = new cpu();
			ram RAM = new ram();
			
			
			byte[] ROMByte = File.ReadAllBytes(path);
			ROM.Init(ROMByte);
			RAM.Init();
			CPU.PowerUp();
			CPU.RunROM(ROM, RAM);
			
			
			//foreach (byte Instruction in Instructions)
			//{
			//	CPU.LDAInstruction(Instruction);
			//}
			
			
		}
	}
}
