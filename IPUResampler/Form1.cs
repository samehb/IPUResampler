// This Project is developed by Sameh Barakat

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace IPUResampler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ProcessBtn_Click(object sender, EventArgs e)
        {
            FileStream IPSFile;
            BinaryReader IPSFileReader;
            FileStream IPUFile;
            BinaryWriter IPUFileWriter;
            FileStream MSVFile;
            BinaryWriter MSVFileWriter;


            StatusLabel.Text = "";

            try
            {

                for (int j = 0; j < IPUStreamsListBox.Items.Count; j++)
                {
                    string IPSFileName = IPUStreamsListBox.Items[j].ToString();
                    string IPSFileExt = IPUStreamsListBox.Items[j].ToString().Split('.')[1];

                    IPSFile = new FileStream(IPSFileName, FileMode.Open, FileAccess.Read);
                    IPSFileReader = new BinaryReader(IPSFile);

                    if (IPSFileExt.ToLower() == "ips")
                    {

                        IPUFile = new FileStream(IPSFileName + ".ipu", FileMode.Create, FileAccess.Write);
                        IPUFileWriter = new BinaryWriter(IPUFile);

                        MSVFile = new FileStream(IPSFileName + ".msv", FileMode.Create, FileAccess.Write);
                        MSVFileWriter = new BinaryWriter(MSVFile);

                        IPSFileReader.ReadUInt32();
                        uint IPUStreamSize = IPSFileReader.ReadUInt32() + 8;

                        uint IPUStreamBlockCount = (IPUStreamSize / 32768) + 1;

                        IPSFile.Seek(0, SeekOrigin.Begin);

                        for (int i = 0; i < IPUStreamBlockCount; i++)
                        {
                            IPUFileWriter.Write(IPSFileReader.ReadBytes(32768));
                        }

                        long MSVStreamSize = IPSFile.Length - IPSFile.Position;

                        for (int i = 0; i < MSVStreamSize; i++)
                        {
                            MSVFileWriter.Write(IPSFileReader.ReadByte());
                        }

                        IPSFileReader.Close();
                        IPUFileWriter.Close();
                        MSVFileWriter.Close();

                        File.Move(IPSFileName, IPSFileName + ".old");
                    }
                    else
                        File.Copy(IPSFileName, IPSFileName + ".old");

                    Process ps = new Process();
                    ps.StartInfo.FileName = @"IPUDecoder.exe";
                    ps.StartInfo.CreateNoWindow = true;
                    ps.StartInfo.UseShellExecute = false;
                    ps.StartInfo.RedirectStandardOutput = false;

                    if (IPSFileExt.ToLower() == "ips")
                        ps.StartInfo.Arguments = IPSFileName + ".ipu" + " " + IPSFileName + ".m2v" + " 0";
                    else
                        ps.StartInfo.Arguments = IPSFileName + " " + IPSFileName + ".m2v" + " 0";

                    ps.Start();
                    ps.WaitForExit();

                    if (IPSFileExt.ToLower() == "ips")
                        File.Delete(IPSFileName + ".ipu");
                    else
                    {
                        IPSFile.Close();
                        File.Delete(IPSFileName);
                    }

                    ps = new Process();
                    ps.StartInfo.FileName = @"ReJig.exe";
                    ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    ps.StartInfo.UseShellExecute = false;
                    ps.StartInfo.CreateNoWindow = true;
                    ps.StartInfo.RedirectStandardOutput = false;
                    ps.StartInfo.Arguments = "-level 46 -o " + IPSFileName + ".resampled.m2v -i " + IPSFileName + ".m2v -auto -close -quiet";
                    ps.Start();
                    ps.WaitForExit();

                    File.Delete(IPSFileName + ".m2v");
                    File.Move(IPSFileName + ".resampled.m2v", IPSFileName + ".m2v");

                    ps = new Process();
                    ps.StartInfo.FileName = @"ps2str.exe";
                    ps.StartInfo.CreateNoWindow = true;
                    ps.StartInfo.UseShellExecute = false;
                    ps.StartInfo.RedirectStandardOutput = false;

                    if (IPSFileExt.ToLower() == "ips")
                        ps.StartInfo.Arguments = "c " + IPSFileName + ".m2v " + IPSFileName + ".ipu";
                    else
                        ps.StartInfo.Arguments = "c " + IPSFileName + ".m2v " + IPSFileName;
                    ps.Start();
                    ps.WaitForExit();

                    File.Delete(IPSFileName + ".m2v");

                    if (IPSFileExt.ToLower() == "ips")
                    {
                        IPUFile = new FileStream(IPSFileName + ".ipu", FileMode.Open, FileAccess.Write);
                        IPUFileWriter = new BinaryWriter(IPUFile);

                        if (IPUFile.Length % 32768 != 0)
                        {
                            IPUFile.SetLength(((IPUFile.Length / 32768) + 1) * 32768);
                        }

                        IPUFile.Close();


                        IPUFile = new FileStream(IPSFileName + ".ipu", FileMode.Open, FileAccess.Read);
                        BinaryReader IPUFileReader = new BinaryReader(IPUFile);

                        MSVFile = new FileStream(IPSFileName + ".msv", FileMode.Open, FileAccess.Read);
                        BinaryReader MSVFileReader = new BinaryReader(MSVFile);

                        IPSFile = new FileStream(IPSFileName, FileMode.Create, FileAccess.Write);
                        BinaryWriter IPSFileWriter = new BinaryWriter(IPSFile);

                        for (int i = 0; i < IPUFile.Length; i++)
                        {
                            IPSFileWriter.Write(IPUFileReader.ReadByte());
                        }

                        for (int i = 0; i < MSVFile.Length; i++)
                        {
                            IPSFileWriter.Write(MSVFileReader.ReadByte());
                        }

                        IPUFileReader.Close();
                        MSVFileReader.Close();
                        IPSFileWriter.Close();


                        File.Delete(IPSFileName + ".msv");
                        File.Delete(IPSFileName + ".ipu");

                    }
                }
                StatusLabel.Text = "Files resampled successfully.";
            }
            catch
            {
                StatusLabel.Text = "Please make sure you have all the files required in the current directory.";
            }
            IPUStreamsListBox.Items.Clear();
        }

        private void IPUStreamsListBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (!IPUStreamsListBox.Items.Contains(file.Substring(file.LastIndexOf("\\") + 1)) && (file.ToLower().EndsWith(".ips") || file.ToLower().EndsWith(".ipu")))
                {
                    IPUStreamsListBox.Items.Add(file.Substring(file.LastIndexOf("\\") + 1));
                }
            }
        }

        private void IPUStreamsListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                e.Effect = DragDropEffects.All;
        }
    }
}
