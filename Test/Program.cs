using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BV.Modbus.TCP;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ModbusTcp mbTcp = new ModbusTcp("127.0.0.1",502,0x11);
            mbTcp.Connect();
            if (mbTcp.IsReady())
            {
                Console.WriteLine("Modbus Bağlantı hazır....");
                mbTcp.SendCommand(ModbusCmd.writeSingleRegister_FC6, 0x0000, 0x00FF);
                mbTcp.DisConnect();
            }
        }
    }
}
