using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using SevenZip;

using System.Reflection;
namespace unzip_truykick
{
    class  struct_file
     {
        public String FileName;
        public long FileSize;
        public byte[] ByteData;
     }

    class Program
    {

        public static void write_dll()
        {

            if (!File.Exists("SevenZipSharp.dll"))
            {
                File.WriteAllBytes("SevenZipSharp.dll", Properties.Resources.SevenZipSharp);
                FileInfo fi = new FileInfo("SevenZipSharp.dll");
                fi.Attributes = FileAttributes.Hidden;
            }
        }

        public static List<struct_file> TkModelFile = new List<struct_file>();
        static void Main(string[] args)
        {
          
             write_dll();
             BinaryReader2 TkFile;
             Console.ForegroundColor = ConsoleColor.Green;
             Console.Clear();
             Console.WriteLine("\n\n             Decompress Model3D TruyKick - Author: Giay Nhap \n\n");
             if (args.Length < 1)
             {
                 Console.BackgroundColor = ConsoleColor.Blue;
                 Console.ForegroundColor = ConsoleColor.White;
                 Console.Clear();
                 Console.WriteLine(" \n\n            Decompress Model3D TruyKick - Author: Giay Nhap \n\n");
                 Console.WriteLine("   [!] Invalid input data!! \n ");
                 Console.WriteLine("        Press anyKey to exit! ");
                 Console.ReadKey();
                 return;
             }
             for (int j = 0; j < args.Length; j++)
             {
                 TkModelFile = new List<struct_file>();
                 string File_in = args[j];
                 Console.WriteLine("File : " + File_in);
                 FileStream fileStream = new FileStream(File_in, FileMode.Open);
                 TkFile = new BinaryReader2(fileStream);
                 while (TkFile.BaseStream.Position + 64 < TkFile.BaseStream.Length)
                 {
                     TkFile.BaseStream.Position += 2;
                     struct_file Data = gn_get_sub_file(TkFile);
                     TkModelFile.Add(Data);
                 }
                 TkFile.Close();
                 Console.WriteLine("\n  [+] NumFile: " + TkModelFile.Count);
                 for (int i = 0; i < TkModelFile.Count; i++)
                 {
                     Console.WriteLine("   [-] File Name: " + TkModelFile[i].FileName + "  - Size: " + TkModelFile[i].FileSize + "  byte");
                     Do_write_file(File_in + "_ex", i);
                 }
                 Console.WriteLine("\n  [+] Completed !!");
             }
             Console.ReadKey();
        }
        public static struct_file gn_get_sub_file( BinaryReader2 FileIn )
        {
           struct_file Data = new struct_file() ;
           string Filename=  read_string(FileIn);
           FileIn.BaseStream.Position -= 1;
           long curfilesize = FileIn.ReadUInt32();
           byte [] data_byte = new byte[curfilesize];
           data_byte= FileIn.ReadBytes((int)curfilesize);
           Data.FileName = Filename;
           Data.FileSize = curfilesize;
           Data.ByteData = data_byte;
           return Data;
        }
        public static void Do_write_file(String path,int ifile)
        {
            Directory.CreateDirectory(path);
            struct_file data = TkModelFile[ifile];
            make_dir(path + "\\" + data.FileName);
                   FileStream fstream;
                   BinaryWriter fwrite;
                   if ((int)data.ByteData[0] == 93)
                   {
                       Random rnd = new Random();
                       string tmppath = Path.GetTempPath();
                       string filetmp = tmppath + "__tmp___gn" + rnd.Next(1000, 9999) + rnd.Next(1000, 9999);
                       fstream = new FileStream(filetmp, FileMode.OpenOrCreate);
                       fwrite = new BinaryWriter(fstream);
                       fwrite.Write(data.ByteData);
                       fwrite.Close();
                       Decompress_file(filetmp, path + "\\" + data.FileName);
                   }
                   else
                   {
                       fstream = new FileStream(path +"\\"+ data.FileName, FileMode.OpenOrCreate);
                       fwrite = new BinaryWriter(fstream);
                       fwrite.Write(data.ByteData);
                   }
                   fwrite.Close();
        }
        public static void make_dir(string indata)
        {
          int  ipos = indata.LastIndexOf("/");
          if (ipos <= 0) return;
          string path = indata.Substring(0,ipos);
          Directory.CreateDirectory(path);
        }

        public static string read_string(BinaryReader gbStream)
        {
            string returndata = "";
            char gchar;
            do
            {
                gchar =gbStream.ReadChar();
                returndata +=gchar ;
            }
            while ((int)gchar != 0 );
            returndata = returndata.Substring(0, returndata.Length - 1);
            return returndata;
        }
        public static bool Decompress_file(FileStream input, string outFile)
        {

            var decoder = new LzmaDecodeStream(input);

            try
            {
                var output = new FileStream(outFile, FileMode.Create);

                int bufSize = 24576, count;
                byte[] buf = new byte[bufSize];
                while ((count = decoder.Read(buf, 0, bufSize)) > 0)
                {
                    output.Write(buf, 0, count);
                }
                input.Close();
                output.Close();
            }
            catch
            {
                return false;
            }
            return true;


        }

         public static bool Decompress_file(string inFile, string outFile)
        {
            var input = new FileStream(inFile, FileMode.Open);

            var decoder = new LzmaDecodeStream(input);

            try
            {
                var output = new FileStream(outFile, FileMode.Create);

                int bufSize = 24576, count;
                byte[] buf = new byte[bufSize];
                while ((count = decoder.Read(buf, 0, bufSize)) > 0)
                {
                    output.Write(buf, 0, count);
                }
                input.Close();
                output.Close();
            }
            catch
            {
                return false;
            }
            return true;

        
    }
       
    }
    class BinaryReader2 : BinaryReader
    {
        private byte[] a16 = new byte[2];
        private byte[] a32 = new byte[4];
        private byte[] a64 = new byte[8];
        public BinaryReader2(System.IO.Stream stream) : base(stream) { }
        public override int ReadInt32()
        {
            a32 = base.ReadBytes(4);
            Array.Reverse(a32);
            return BitConverter.ToInt32(a32, 0);
        }
        public Int16 ReadInt16()
        {
            a16 = base.ReadBytes(2);
            Array.Reverse(a16);
            return BitConverter.ToInt16(a16, 0);
        }
        public Int64 ReadInt64()
        {
            a64 = base.ReadBytes(8);
            Array.Reverse(a64);
            return BitConverter.ToInt64(a64, 0);
        }
        public UInt32 ReadUInt32()
        {
            a32 = base.ReadBytes(4);
            Array.Reverse(a32);
            return BitConverter.ToUInt32(a32, 0);
        }

    }

}

