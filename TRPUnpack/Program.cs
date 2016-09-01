using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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


        }
    }
}
