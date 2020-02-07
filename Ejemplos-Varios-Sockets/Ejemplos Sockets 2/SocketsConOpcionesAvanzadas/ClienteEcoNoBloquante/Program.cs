// ******************************************************************************************
// Arroyo Auz Christian Xavier.                                                             *
// 01/06/2016.                                                                              *
// ******************************************************************************************


using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading; 
using System.Linq;
using System.Text;
using System.Net;
using System;

namespace ClienteEcoNoBloquante
{
    class Program
    {
        static void Main(string[] args)
        {
            String servidor = "localhost";
            //Estableciendo el tamaño del bufer de Tx y RX
            byte[] buferTx = new byte[512];
            byte[] buferRx = new byte[512];
            int puerto = 8080;
            Socket socketCliente = null;
            
            try
            {
                //Estableciendo el puerto y el tipo de protocolo a usar
                socketCliente = new Socket(AddressFamily.InterNetwork,  SocketType.Stream,  ProtocolType.Tcp);
                socketCliente.Connect(new IPEndPoint(Dns.Resolve(servidor).AddressList[0], puerto));
            }
            catch (Exception e)
            {
                //Mensage de erro de ocurrir una excepcion
                Console.WriteLine(e.Message); Environment.Exit(-1);
            }
            
            int totalBytesEnviados = 0;
            int totalBytesRecibidos = 0;
            //Mensaje enviado al servidor
            buferTx = Encoding.ASCII.GetBytes("ESTOY ENVIANDO TODO ESTOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO!");
            socketCliente.Blocking = false;
            
            while (totalBytesRecibidos < buferTx.Length)
            {
                if (totalBytesEnviados < buferTx.Length)
                {
                    try 
                    { 
                        totalBytesEnviados += socketCliente.Send(buferTx, totalBytesEnviados, buferTx.Length - totalBytesEnviados, SocketFlags.None);
                        Console.WriteLine("Se han enviado un total de {0} bytes al servidor...", totalBytesEnviados);
                    }
                    catch (SocketException se)
                    {
                        if (se.ErrorCode == 10035)
                        {
                            //WSAEWOULDBLOCK: Recurso temproalmente no disponible
                            Console.WriteLine("Temporalmente no es posible enviar, se reintentará despues...");
                        }
                        else
                        { 
                            Console.WriteLine(se.ErrorCode + ": " + se.Message);
                            socketCliente.Close();
                            Environment.Exit(se.ErrorCode);
                        }
                    }
                }

                try
                { 
                    int bytesRecibidos = 0;
                    
                    if ((bytesRecibidos = socketCliente.Receive(buferRx, totalBytesRecibidos, buferRx.Length - totalBytesRecibidos, SocketFlags.None)) == 0)
                    {
                        Console.WriteLine("La conexion se cerro prematuramente...");
                        break;
                    }
                    totalBytesRecibidos += bytesRecibidos;
                }
                catch (SocketException se)
                { 
                    if (se.ErrorCode == 10035)
                    {
                        continue; 
                    }
                    else
                    { 
                        Console.WriteLine(se.ErrorCode + ": " + se.Message);
                        break;
                    }
                } 
                RealizarProcesamiento();
            }
            Console.WriteLine("Se han recibido {0} bytes desde el servidor: {1}", totalBytesRecibidos, Encoding.ASCII.GetString(buferRx, 0, totalBytesRecibidos));
            socketCliente.Close();
        }  
        
        static void RealizarProcesamiento()
        {
            //Tiempo para irse a dormir
            //Si tiempo es mayor o igual a 10 segundos se rompe la conexion
            //Console.WriteLine("."); Thread.Sleep(2000);
            Console.WriteLine("."); Thread.Sleep(11000);
        }
    }
}