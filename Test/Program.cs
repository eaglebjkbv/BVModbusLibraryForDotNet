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

                bool result = mbTcp.WriteMultipleRegisters(0x0000, new List<ushort> { 0xABCD, 0x01111,0x2222,0x4444 });

                // bool result=mbTcp.WriteSingleRegister(0x0000,0x00FF);
                if (result)
                {
                    Console.WriteLine("Başarılı");
                }
                else
                {
                    Console.WriteLine("Başarısız !!!");
                }
                mbTcp.DisConnect();
            }
            Console.ReadLine();
        }
    }
}
