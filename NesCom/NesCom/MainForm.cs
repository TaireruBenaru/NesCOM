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
			
			 
			
			
			
			
			
			
		}
		
		
		private void OpenROM(object sender, EventArgs e)
        {
            // Show the dialog and get result.
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
            	string path = openFileDialog1.FileName;
            	
            				rom ROM = new rom();
			cpu CPU = new cpu();
			ram RAM = new ram();
				byte[] ROMByte = File.ReadAllBytes(path);
			
			
			int num1 = 0x8000;
			int num2 = 0xc000;
			
			
			ROM.Init(ROMByte, RAM);
			RAM.Init();
			Buffer.BlockCopy(ROM.ROMBytes, 0x10, RAM.Memory, num1, 0x4000);
			if(ROM.NumOfPRGBlocks == 1)
			{
				Buffer.BlockCopy(ROM.ROMBytes, 0x10, RAM.Memory, num2, 0x4000);
			}
			Debug.WriteLine(RAM.Memory[0x8000]);
			Debug.WriteLine(RAM.Memory[0xC5F5]);
			CPU.PowerUp();
			CPU.RunROM(ROM, RAM);
            	
            }
            Console.WriteLine(result); // <-- For debugging use.
        }
	}
}
