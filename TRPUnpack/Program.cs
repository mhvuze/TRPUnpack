using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace TRPUnpack
{
    class Program
    {
        static void Main(string[] args)
        {
            // Variables
            string input = "";

            // Print header
            Console.WriteLine("TRPUnpack by MHVuze");
            Console.WriteLine("=========================");

            // Handle arguments
            if (args.Length < 1) { Console.WriteLine("ERROR: Supply input file."); return; }
            input = args[0];

            // Check file
            if (!File.Exists(input)) { Console.WriteLine("ERROR: Specified file doesn't exist."); return; }

            // Unpack
            using (BinaryReader reader = new BinaryReader(File.Open(input, FileMode.Open)))
            {
                // Check magic
                if (reader.ReadInt32() != 0x004DA2DC) { Console.WriteLine("ERROR: Specified file is no valid TRP file."); return; }

                // Create output directory
                string folder_path = new FileInfo(input).Directory.FullName + "\\" + Path.GetFileNameWithoutExtension(input);
                if (!Directory.Exists(folder_path)) { Directory.CreateDirectory(folder_path); }

                // Get entry count
                reader.BaseStream.Seek(0x10, SeekOrigin.Begin);
                int count = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                int start = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                reader.BaseStream.Seek(start, SeekOrigin.Begin);

                // Extract files
                long reader_offset = 0;
                for (int i = 0; i < count; i++)
                {
                    // Get entry info
                    string file_name = readNullterminated(reader);
                    reader.BaseStream.Seek(start + (i * 0x40) + 0x20, SeekOrigin.Begin);
                    long offset = IPAddress.NetworkToHostOrder(reader.ReadInt64());
                    long size = IPAddress.NetworkToHostOrder(reader.ReadInt64());
                    reader_offset = reader.BaseStream.Position + 0x10;                    

                    Console.WriteLine("Processing file {0} / {1}: {2} [{3} | {4} bytes]", (i + 1), count, file_name, offset.ToString("X16"), size);

                    // Extract file
                    reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                    byte[] file_data = reader.ReadBytes(Convert.ToInt32(size));
                    File.WriteAllBytes(folder_path + "\\" + file_name, file_data);

                    reader.BaseStream.Seek(reader_offset, SeekOrigin.Begin);
                }

                // End
                Console.WriteLine("=========================");
                Console.WriteLine("INFO: Finished extracting {0} files.", count);
            }
        }

        // Read null terminated string
        static string readNullterminated(BinaryReader reader)
        {
            var char_array = new List<byte>();
            string str = "";
            if (reader.BaseStream.Position == reader.BaseStream.Length)
            {
                byte[] char_bytes2 = char_array.ToArray();
                str = Encoding.UTF8.GetString(char_bytes2);
                return str;
            }
            byte b = reader.ReadByte();
            while ((b != 0x00) && (reader.BaseStream.Position != reader.BaseStream.Length))
            {
                char_array.Add(b);
                b = reader.ReadByte();
            }
            byte[] char_bytes = char_array.ToArray();
            str = Encoding.UTF8.GetString(char_bytes);
            return str;
        }
    }
}
