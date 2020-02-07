// ******************************************************************************************
// Arroyo Auz Christian Xavier.                                                             *
// 01/06/2016.                                                                              *
// ******************************************************************************************


using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Collections;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Net;
using Protocolo;
using System;

namespace Servidor
{
    public partial class Servidor : Form
    {
        //Definiendo la estructura del cliente
        private struct Cliente
        { 
            public EndPoint puntoExtremo; 
            public string nombre; 
        }

        private ArrayList listaClientes;
        private Socket socketServidor;
        //Tamaño maximo del buffer
        private byte[] buferRx = new byte[1024];
        private delegate void DelegadoActualizarEstado(string estado);
        private DelegadoActualizarEstado delegadoActualizarEstado = null;  

        public Servidor()
        {
            InitializeComponent();
        }

        private void Servidor_Load(object sender, EventArgs e)
        {
            //Definiendo el tipo de protocolo a usarse asi como el tipo de socket que se usara
            //Se define el puerto por el cual se enviara y recibira los datos
            //Finalmente el socke esta habilitado para recibir datos
            try 
            { 
                listaClientes = new ArrayList();
                delegadoActualizarEstado = new DelegadoActualizarEstado(ActualizarEstado);
                socketServidor = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint servidorExtremo = new IPEndPoint(IPAddress.Any, 30000);
                socketServidor.Bind(servidorExtremo);
                IPEndPoint clienteExtremo = new IPEndPoint(IPAddress.Any, 0);  
                EndPoint extremoEP = (EndPoint)clienteExtremo;
                socketServidor.BeginReceiveFrom(buferRx, 0, buferRx.Length, SocketFlags.None, ref extremoEP, 
                                                new AsyncCallback(ProcesarRecibir), extremoEP);
                lblEstado.Text = "Escuchando";
            } 
            //Mensaje de error de producirce una excepcion al no poder recibir los datos enviados
            catch (Exception ex)
            {
                lblEstado.Text = "Error";
                MessageBox.Show("Cargando Error: " + ex.Message, "Servidor UDP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcesarRecibir(IAsyncResult resultadoAsync)
        {
            //Proceso de reensablado de los datos en el destino
            try
            {
                byte[] data;
                Paquete datoRecibido = new Paquete(buferRx);
                Paquete datoParaEnviar = new Paquete();
                IPEndPoint puntoExtremoCliente = new IPEndPoint(IPAddress.Any, 0);
                EndPoint extremoEP = (EndPoint)puntoExtremoCliente;
                socketServidor.EndReceiveFrom(resultadoAsync, ref extremoEP);
                datoParaEnviar.IdentificadorChat = datoRecibido.IdentificadorChat;
                datoParaEnviar.NombreChat = datoRecibido.NombreChat;

                //Diferentes posibilidades de presentacion de mensajes en el destino
                //1) Mensajes recibidos de un cliente
                //2) Mensaje de un cliente que se ha conectado
                //3) Mensaje de cliente desconectado
                //Finalmente se ara un proceso para que los datos se presenten una sola vez
                switch (datoRecibido.IdentificadorChat)
                {
                    case IdentificadorDato.Mensaje:
                        datoParaEnviar.MensajeChat = string.Format("{0}: {1}", datoRecibido.NombreChat, datoRecibido.MensajeChat);
                        break;
                    case IdentificadorDato.Conectado: 
                        Cliente nuevoCliente = new Cliente();
                        nuevoCliente.puntoExtremo = extremoEP; 
                        nuevoCliente.nombre = datoRecibido.NombreChat;
                        listaClientes.Add(nuevoCliente);
                        datoParaEnviar.MensajeChat = string.Format("-- {0} está conectado --", datoRecibido.NombreChat);
                        break;
                    case IdentificadorDato.Desconectado: 
                        foreach (Cliente c in listaClientes)
                        { 
                            if (c.puntoExtremo.Equals(extremoEP))
                            { 
                                listaClientes.Remove(c); 
                                break; 
                            } 
                        }
                        datoParaEnviar.MensajeChat = string.Format("-- {0} se ha desconectado -    ", datoRecibido.NombreChat);
                        break;
                }

                data = datoParaEnviar.ObtenerArregloBytes();
                
                foreach (Cliente clienteEnLista in listaClientes)
                {
                    if (clienteEnLista.puntoExtremo != extremoEP || datoParaEnviar.IdentificadorChat != IdentificadorDato.Conectado)
                    {
                        socketServidor.BeginSendTo(data, 0, data.Length, SocketFlags.None, clienteEnLista.puntoExtremo, new AsyncCallback(ProcesarEnviar), clienteEnLista.puntoExtremo);
                    }
                }
                socketServidor.BeginReceiveFrom(buferRx, 0, buferRx.Length, SocketFlags.None, ref extremoEP, new AsyncCallback(ProcesarRecibir), extremoEP);
                Invoke(delegadoActualizarEstado, new object[] { datoParaEnviar.MensajeChat });
            }
            catch (Exception ex)
            { 
                MessageBox.Show("Error en la recepción: " + ex.Message, "Servidor UDP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ProcesarEnviar(IAsyncResult resultadoAsync)
        { 
            try
            { 
                socketServidor.EndSend(resultadoAsync);
            } 
            catch (Exception ex)
            { 
                MessageBox.Show("Error al enviar datos: " + ex.Message, "Servidor UDP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }

        private void ActualizarEstado(string estado)
        { 
            rxtInformacion.Text += estado + Environment.NewLine;
        }

        //Cierra el formulario y el socket qye se esta usando
        private void btnTerminar_Click(object sender, EventArgs e)
        {
            socketServidor.Close();
            Close(); 
        } 
    }
}