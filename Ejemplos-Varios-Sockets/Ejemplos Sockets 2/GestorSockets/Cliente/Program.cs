// ******************************************************************************************
// Arroyo Auz Christian Xavier.                                                             *
// 01/06/2016.                                                                              *
// ******************************************************************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Net;
using System;

namespace Cliente
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Cliente());

            //Lanzando el formulario para que pueda trabajar con TCP y HTTP
            ConexionCliente gestor = new ConexionCliente(new Cliente());
            ((Cliente)gestor.ObtenerVista()).EstablecerGestorCliente(gestor);
            gestor.EspecificarLog(((Cliente)gestor.ObtenerVista()).ManejoLog);
            Application.Run(gestor.ObtenerVista());
        }
    }

    public enum estadoConexion
    {
        //Enumeracion de las diferentes estados en los que se puede trabajar
        ok = 0,
        problemaDns = 1,
        problemaConSocket = 2,
        problemaConDns_y_Socket = 3,
        errorDeRedOInalcanzable = 4
    }

    //Inicio de conexion con la direccion DNS ingresada
    public class ConexionCliente
    {
        IPAddress direccionServidor;
        int puerto = 0;
        IPEndPoint sitioRemoto;
        TcpClient cliente;
        Form formularioCliente = null;
        private ManejoLog log;

        public ConexionCliente(Form formularioCliente)
        {
            this.formularioCliente = formularioCliente;
        }

        ~ConexionCliente()
        {
            if (cliente != null)
            {
                if (cliente.Connected) cliente.Close();
            }
        }

        public void EspecificarLog(ManejoLog log)
        {
            this.log = log;
        }

        public void Traza(string msg)
        {
            StackTrace traza = new StackTrace(false);
            string metodoUsado = traza.GetFrame(1).GetMethod().Name;
            log(metodoUsado + " : " + msg + "\r\n");
        }

        public Form ObtenerVista()
        {
            return formularioCliente;
        }

        private estadoConexion ObtenerError(estadoConexion actual, estadoConexion siguiente)
        {
            Traza("Error Detectado");
            if (actual == estadoConexion.ok)
            {
                return siguiente;
            }
            if (actual == estadoConexion.problemaDns)
            {
                if (siguiente == estadoConexion.problemaConSocket)
                {
                    return estadoConexion.problemaConDns_y_Socket;
                }
                else
                {
                    return siguiente;
                }
            }
            return siguiente;
        }

        public String ObtenerResultadoPruebaConexion()
        {
            return PruebaConexion().ToString();
        }

        private estadoConexion PruebaConexion()
        {
            Traza("Prueba de Conexión de Cliente");
            estadoConexion resultado = estadoConexion.ok;
            String testHttp = "GET /index.html HTTP/1.0\n\n";
            String httpDoc = null;
            int cantRecibida = 0;
            Byte[] bytesParaEnviar = Encoding.ASCII.GetBytes(testHttp);
            Byte[] bytesParaRecibir = new Byte[1024];
            string nombre = "";
            IPAddress IPPrueba = null;
            IPEndPoint extremoPrueba = null;

            try
            {
                IPPrueba = Dns.GetHostEntry("www.epn.edu.ec").AddressList[0];
                extremoPrueba = new IPEndPoint(IPPrueba, 80);
            }
            catch (Exception)
            {
                Traza("Parece que el DNS no funciona...");
                resultado = ObtenerError(resultado, estadoConexion.problemaDns);
            }

            try
            {
                nombre = ((IPHostEntry)Dns.GetHostEntry("163.117.139.128")).HostName;
                cliente = new TcpClient();
            }
            catch (Exception)
            {
                Traza("Problemas con los sockets...");
                return ObtenerError(resultado, estadoConexion.problemaConSocket);
            }

            try
            {
                cliente.Connect(extremoPrueba);
                NetworkStream flujo = cliente.GetStream();
                flujo.Write(bytesParaEnviar, 0, bytesParaEnviar.Length);
                cantRecibida = flujo.Read(bytesParaRecibir, 0, bytesParaRecibir.Length);
            }
            catch (Exception)
            {
                Traza("Error en la conexión...");
                return ObtenerError(resultado, estadoConexion.errorDeRedOInalcanzable);
            }
            httpDoc = Encoding.ASCII.GetString(bytesParaRecibir, 0, cantRecibida);
            Traza("Prueba finalizada");
            return resultado;
        }

        public void EspecificarServidor(String direccionIP)
        {
            direccionServidor = Dns.GetHostEntry(direccionIP).AddressList[0];
        }

        public void EspecificarPuertoServidor(String puerto)
        {
            int resultado = 0;

            try
            {
                int.TryParse(puerto, out resultado);
                this.puerto = resultado;
            }
            catch (Exception)
            {
                Traza("Está correcto el puerto?");
            }
        }

        public IPAddress ObtenerDireccionIP(String nombreEquipo)
        {
            return Dns.GetHostEntry(nombreEquipo).AddressList[0];
        }

        public void Conectar()
        {
            try
            {
                if (cliente != null) if (cliente.Connected)
                    {
                        Traza("Cerrando conexiones...");
                        cliente.Client.Disconnect(true);
                    }
                Traza("Creando un endpoint...");
                sitioRemoto = new IPEndPoint(direccionServidor, puerto);
                Traza("Creando el socket...");
                cliente = new TcpClient(); Traza("Conectando...");
                cliente.Connect(sitioRemoto);
            }
            catch (Exception ex)
            {
                Traza("Error en la conexión" + ex.Message);
            }
        }

        public int EnviarRecibir(byte[] buferTx, ref byte[] buferRx)
        {
            try
            {
                int bytes_obtenidos = 0;
                NetworkStream flujo = cliente.GetStream();
                flujo.Write(buferTx, 0, buferTx.Length);
                bytes_obtenidos = flujo.Read(buferRx, 0, buferRx.Length);
                return bytes_obtenidos;
            }
            catch (SocketException sExec)
            {
                Traza("Error: " + sExec.Message);
            }
            return 0;
        }
    }
}