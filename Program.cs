using System;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Diagnostics;
namespace Arduino_System_Information


{
    public class Program
    {
        static void Main(string[] args)
        {
            //Your com port, and baud rate:
            SerialPort port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            port.Open();
            Console.WriteLine("Serial is ready!");
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            while (true)
            {
                MEMORY_INFO ram = new MEMORY_INFO();
                ram.dwLength = (uint)Marshal.SizeOf(ram);
                GlobalMemoryStatusEx(ref ram);

                ulong totalMem = ram.ullTotalPhys / (1024 * 1024),
                memUsage = totalMem - ram.ullAvailPhys / (1024 * 1024);

                int memRatio = (int)(memUsage * 1.0 / totalMem * 100),
                 cpuUsage = (int)cpuCounter.NextValue();

                byte[] MyMessage = System.Text.Encoding.UTF8.GetBytes($"{memUsage}-{memRatio}-{cpuUsage}");
                port.Write(MyMessage, 0, MyMessage.Length);

                //Per 1000 ms
                System.Threading.Thread.Sleep(1000);
            }
        }


        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalMemoryStatusEx(ref MEMORY_INFO ram);

        //Define the information structure of memory
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_INFO
        {
            public uint dwLength; //Current structure size
            public uint dwMemoryLoad; //Current memory utilization
            public ulong ullTotalPhys; //Total physical memory size
            public ulong ullAvailPhys; //Available physical memory size
            public ulong ullTotalPageFile; //Total Exchange File Size
            public ulong ullAvailPageFile; //Total Exchange File Size
            public ulong ullTotalVirtual; //Total virtual memory size
            public ulong ullAvailVirtual; //Available virtual memory size
            public ulong ullAvailExtendedVirtual; //Keep this value always zero
        }

    }
}
