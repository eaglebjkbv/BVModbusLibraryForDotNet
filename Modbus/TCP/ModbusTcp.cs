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
        private byte _unitAddress = 0;
        private Byte[] _data=new Byte[512]; 
        private Byte[] _rData = new Byte[512];
        
        /// <summary>
        /// Connects the speicified IP adres with Port numer of Modbus TCP server or specified Unit number with Modbus TCP serial GateWay
        /// </summary>
        /// <param name="ip"> ip Number of Modbus TCP Server</param>
        /// <param name="port">Port number of Modbus TCP server Default value is 512</param>
        /// <param name="unitAddress">Unit Number of Modbus Serial unit behind Modbus TCP Server Gateway. Default value is 1</param>

        public ModbusTcp(string ip,int port=502,byte unitAddress = 1)
        {
            _data[0] = 0x00;_data[1] = 0x01;
            _data[2] = 0x00;_data[3] = 0x00;
            _data[4] = 0x00; _data[5] = 0x06;
            _data[6] = 0x011;
            _data[7] = 0x03;
            _data[8] = 0x00;_data[9] = 0x00;
            _data[10] = 0x00;_data[11] = 0x01;
             
      
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
        public bool WriteMultipleRegisters(uint regNr,List<UInt16> values)
        {
            bool result = false;
            try
            {
                int len = values.Count;
                Console.WriteLine($"uzunluk:{len.ToString()}");
                if (len * 2 > 255) new Exception("Cannot be more than 255 bytes");
                
                

                //byte[] valBytes = BitConverter.GetBytes(value);
                if (_networkStream != null){
                    byte[] lenFrame = BitConverter.GetBytes((len * 2) + 7);
                    byte[] lenByte = BitConverter.GetBytes(len*2);
                    byte[] lenCount = BitConverter.GetBytes(len);
                    byte[] regBytes = BitConverter.GetBytes(regNr);

                    _data[4] = lenFrame[1]; _data[5] = lenFrame[0];
                    _data[6] = _unitAddress;
                    _data[7] = 0x10;
                    _data[8] = regBytes[1];_data[9] = regBytes[0];
                    _data[10] = lenCount[1]; _data[11] = lenCount[0];
                    _data[12] = lenByte[0];
                    

                    int i = 0;
                    foreach (var value in values)
                    {
                        byte[] valBytes = BitConverter.GetBytes(value);
                        _data[13 + i] = valBytes[1];
                        _data[14 + i] = valBytes[0];
                        //Console.Write($"-{_data[13 + i].ToString()}");
                        //Console.Write($"-{_data[14 + i].ToString()}");
                        i = i + 2 ;
                    }
                    _networkStream.Write(_data, 0, 13+len*2);

                    while (true)
                    {
                        int bytes = _networkStream.Read(_rData, 0, 12);
                        if (bytes > 0)
                        {
                            //foreach (byte d in _rData)
                            //    Console.Write($"-{d.ToString("X2")}");
                            if (_rData[7] == 0x10) result = true;
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



        public bool WriteSingleRegister(UInt16 regNr,UInt16 value)
        {
            bool result = false;
            try
            {
                byte[] regBytes = BitConverter.GetBytes(regNr);
                byte[] valBytes = BitConverter.GetBytes(value);
                

                if (_networkStream != null)
                {
                    _data[6] = _unitAddress;
                    _data[7] = 0x06;
                    _data[8] = regBytes[1];
                    _data[9] = regBytes[0];
                    _data[10] = valBytes[1];
                    _data[11] = valBytes[0];
                    //foreach (byte d in _data)
                    //    Console.Write($"-{d.ToString("X2")}");
                    //Console.WriteLine();
                    _networkStream.Write(_data, 0, 12);
                    while (true)
                    {
                        int bytes = _networkStream.Read(_rData, 0, 12);
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