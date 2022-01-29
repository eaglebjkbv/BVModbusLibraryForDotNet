using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace BV.Modbus.TCP
{
    public enum ModbusCmd
    {
        writeSingleRegister_FC6,
        readRegisters_FC3,
    }
    class LimitException : System.Exception
    {
        public LimitException()
         : base()
        { }

        public LimitException(String message)
            : base(message)
        { }
    }
    public class ModbusTcp
    {
        
        
        private TcpClient _tcpClient = new TcpClient();
        private NetworkStream _networkStream;
        public string _ip { get; set; }
        public int _port { get; set; }
        private int _unitAddress = 0;
        private Byte[] _data ={
                0x00,0x01,
                0x00,0x00,
                0x00,0x06,
                0x11,
                0x03,
                0x00,0x00,
                0x00,0x01
        };
        private Byte[] _rData = new Byte[512];
        
        /// <summary>
        /// Connects the speicified IP adres with Port numer of Modbus TCP server or specified Unit number with Modbus TCP serial GateWay
        /// </summary>
        /// <param name="ip"> ip Number of Modbus TCP Server</param>
        /// <param name="port">Port number of Modbus TCP server Default value is 512</param>
        /// <param name="unitAddress">Unit Number of Modbus Serial unit behind Modbus TCP Server Gateway. Default value is 1</param>

        public ModbusTcp(string ip,int port=502,int unitAddress = 1)
        {
            _ip = ip;
            _port = port;
            _unitAddress = unitAddress;
        }
        public bool Connect()
        {
            if (!_tcpClient.Connected && _ip != null && _port != 0)
            {
                try
                {
                    _tcpClient.Connect(_ip, _port);
                    if (_tcpClient.Connected)
                    {
                        _networkStream = _tcpClient.GetStream();
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return _networkStream.CanWrite;
        }
        public bool DisConnect()
        {
            if (_networkStream.CanWrite)
            {
                _networkStream.Close();
                _tcpClient.Close();
            }
            return _tcpClient.Connected;
        }
        
        public bool IsReady()
        {
            if (_networkStream.CanWrite)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SendCommand(ModbusCmd cmd,UInt16 regNr,UInt16 value)
        {
            bool result = false;
            try
            {
                byte[] regBytes = BitConverter.GetBytes(regNr);
                byte[] valBytes = BitConverter.GetBytes(value);
                byte[] unitBytes = BitConverter.GetBytes(_unitAddress);

                if (_networkStream != null)
                {
                    _data[6] = unitBytes[0];

                    if(cmd==ModbusCmd.writeSingleRegister_FC6) _data[7] = 0x06; // Write Single Register command
                    if(cmd == ModbusCmd.readRegisters_FC3) _data[7] = 0x03; // Read registers
                    
                    _data[8] = regBytes[1];
                    _data[9] = regBytes[0];
                    _data[10] = valBytes[1];
                    _data[11] = valBytes[0];
                    //foreach (byte d in _data)
                    //    Console.Write($"-{d.ToString("X2")}");
                    //Console.WriteLine();
                    _networkStream.Write(_data, 0, _data.Length);
                    while (true)
                    {
                        int bytes = _networkStream.Read(_rData, 0, _rData.Length);
                        if (bytes > 0)
                        {
                            //foreach (byte d in _rData)
                            //    Console.Write($"-{d.ToString("X2")}");
                            if (_rData[7] == 0x06) result = true;
                            break;

                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            
            return result;
        }
 
            
        
    }
}