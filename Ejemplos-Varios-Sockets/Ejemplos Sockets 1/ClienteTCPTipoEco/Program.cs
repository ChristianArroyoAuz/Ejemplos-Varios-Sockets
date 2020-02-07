using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Net;
using System;

namespace ClienteTCPTipoEco
{
    class Program
    {
        static void Main(string[] args)
        {
            string datos = "##--##--##----***----##--##--##";
            //Ingresando la direccion Ip y el puerto por el cual se enviara y escuchara los datos
            IPEndPoint remoto = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
            TcpClient cliente = new TcpClient();
            //byte[] bufferRx = new byte[512];
            byte[] bufferRx = new byte[32];
            int indicador;
            //Conectando al cliente
            cliente.Connect(remoto);
            if (cliente.Connected)
            {
                //Inicializando el flujo de bytes
                NetworkStream flujo = cliente.GetStream();
                //Codificando una matrix de bytes de datos
                byte[] bufferTx = Encoding.ASCII.GetBytes(datos);
                flujo.Write(bufferTx, 0, bufferTx.Length);
                do
                {
                    //Revisando que el buffer sea distinto de cero
                    indicador = flujo.Read(bufferRx, 0, bufferRx.Length);
                    if (indicador > 0)
                    {
                        //decodificando el flujo de bytes para ser imprimidos
                        datos = Encoding.ASCII.GetString(bufferRx);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("Mensaje Recibido.");
                        Console.Write("Se recibio: ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(datos);
                    }
                }
                while (indicador > 0);
                //Cerrando la conexion con el cliente
                cliente.Close();
            }
        }
    }
}