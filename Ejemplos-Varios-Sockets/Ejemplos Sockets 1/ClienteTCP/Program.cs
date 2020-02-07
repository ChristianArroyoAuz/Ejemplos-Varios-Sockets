using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Net;
using System;

namespace ClienteTCP
{
    class Program
    {
        static void Main(string[] args)
        {
            //Cadena de texto a enviar
            string datos = "##--##--##----***----##--##--##" + "   "+ "ClienteTCP_1";
            string direccion;
            int puerto;
            Console.WriteLine("Cliente TCP 1.");
            Console.WriteLine("\n");
            Console.Write("Ingrese la direccion IP a la que deses enviar datos: ");
            //Leyendo lo que se ingresa por consola, asosiando la direccion IP al servidorIP
            direccion = Console.ReadLine();
            Console.WriteLine("\n");
            Console.Write("Ingrese el número de puerto por el cual desea conectarse: ");
            //Leyendo lo que se ingresa por consola, asosiando el puerto
            puerto = Convert.ToInt16(Console.ReadLine());
            //Asignando la direccion IP y el puerto por el cual enviar la información
            //IPEndPoint remoto = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
            IPEndPoint remoto = new IPEndPoint(IPAddress.Parse(direccion), puerto);
            //Inicializando una nueva instancia TCP
            TcpClient cliente = new TcpClient(); 
            cliente.Connect(remoto);
            if (cliente.Connected) 
            {
                //Si se halla conectado se inicializa una nueva instancia para proporcionar el flujo de datos
                NetworkStream flujo = cliente.GetStream();
                //Realizado un arreglo de bytes y transmitiendolos
                byte[] bufferTx = Encoding.ASCII.GetBytes(datos); 
                flujo.Write(bufferTx, 0, bufferTx.Length); 
                //Cerrando el Cliente
                cliente.Close();
            } 
        }
    }
}