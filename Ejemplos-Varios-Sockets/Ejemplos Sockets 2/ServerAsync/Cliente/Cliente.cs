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

namespace Cliente
{
    public partial class Cliente : Form
    {
        private Socket socketCliente;
        private string nombre;
        private EndPoint epServidor;
        private byte[] buferRx = new byte[1024];
        private delegate void DelegadoMensajeActualizacion(string mensaje);
        private DelegadoMensajeActualizacion delegadoActualizacion = null;

        public Cliente()
        {
            InitializeComponent();
        }

        //Activando el delegado creado para la actualizacion
        private void Cliente_Load(object sender, EventArgs e)
        {
            delegadoActualizacion = new DelegadoMensajeActualizacion(DesplegarMensaje);
        }

        //Cierra el formulario
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        //Tomando los datos del textbox y enviandolo al servidor, los mismo que seran enviados a los demas usuarios que
        //esten conectados al servidor
        private void btnEnviar_Click(object sender, EventArgs e)
        {
            try
            {
                //Se identifiara el nombre del cliente que esta envindo los datos
                Paquete paqueteParaEnviar = new Paquete();
                paqueteParaEnviar.NombreChat = nombre;
                paqueteParaEnviar.MensajeChat = txtMensajeParaEnviar.Text.Trim();
                paqueteParaEnviar.IdentificadorChat = IdentificadorDato.Mensaje;
                byte[] arregloBytes = paqueteParaEnviar.ObtenerArregloBytes();
                socketCliente.BeginSendTo(arregloBytes, 0, arregloBytes.Length, SocketFlags.None, epServidor, new AsyncCallback(ProcesarEnviar), null);
                txtMensajeParaEnviar.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar: " + ex.Message, "Cliente UDP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Nos permitira enlazarnos al servidor, siempre y cuando se hayan ingresado los datos de la direccion del
        //servidor correctamente y tambien simpre y cuando se haya ingresado un nombre de usuario
        private void btnConectar_Click(object sender, EventArgs e)
        {
            try
            {
                //Si los datos ingresados son correcto se intentara hacer una conexion con el servidor
                //mediante la direccion IP local o una direccion IP de red y el puerto 30000
                nombre = txtNombre.Text.Trim();
                Paquete paqueteInicio = new Paquete();
                paqueteInicio.NombreChat = nombre;
                paqueteInicio.MensajeChat = null;
                paqueteInicio.IdentificadorChat = IdentificadorDato.Conectado;
                //asignando el tipo de protocolo a usarse y el tipo de puerto a usarse
                socketCliente = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPAddress servidorIP = IPAddress.Parse(txtServidor.Text.Trim());
                IPEndPoint puntoRemoto = new IPEndPoint(servidorIP, 30000);
                epServidor = (EndPoint)puntoRemoto;
                byte[] buferTx = paqueteInicio.ObtenerArregloBytes();
                socketCliente.BeginSendTo(buferTx, 0, buferTx.Length, SocketFlags.None, epServidor, new AsyncCallback(ProcesarEnviar), null);
                //Asignando el tamaño del bufer maximo de lectura en recepcion
                buferRx = new byte[1024];
                socketCliente.BeginReceiveFrom(buferRx, 0, buferRx.Length, SocketFlags.None, ref epServidor, new AsyncCallback(this.ProcesarRecibir), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectarse: " + ex.Message, "Cliente UDP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcesarEnviar(IAsyncResult res)
        {
            //Envio del mensaje atraves del socket
            try
            {
                socketCliente.EndSend(res);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enviar Datos: " + ex.Message, "Cliente UDP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcesarRecibir(IAsyncResult res)
        {
            try
            {
                //Trermina el proceso de recibir los datos
                socketCliente.EndReceive(res);
                Paquete paqueteRecibido = new Paquete(buferRx);
                if (paqueteRecibido.MensajeChat != null)
                {
                    Invoke(delegadoActualizacion, new object[] { paqueteRecibido.MensajeChat });
                }
                buferRx = new byte[1024];
                //El cliente comiensa a recibir los datos del servidor
                socketCliente.BeginReceiveFrom(buferRx, 0, buferRx.Length, SocketFlags.None, ref epServidor, new AsyncCallback(ProcesarRecibir), null);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show("Datos Recibidos: " + ex.Message, "Cliente UDP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Despliegue de los mensajes en pantalla con el nombre del cliente que los envio
        private void DesplegarMensaje(string mensaje)
        {
            rxtMensajes.Text += mensaje + Environment.NewLine;
        }

        private void Cliente_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Mientras se esta cerrando el formulario tambien se cerrar todo tipo de conexion con el servido
            //Se informara a los clientes conectados que un cliente se ha desconectado
            //Tambien se hara un proceso para cerrar los socket usados para que estos puedan ser usados luego
            try
            {
                if (this.socketCliente != null)
                {
                    Paquete paqueteSalida = new Paquete();
                    paqueteSalida.IdentificadorChat = IdentificadorDato.Desconectado;
                    paqueteSalida.NombreChat = nombre;
                    paqueteSalida.MensajeChat = null;
                    byte[] buferTx = paqueteSalida.ObtenerArregloBytes();
                    socketCliente.SendTo(buferTx, 0, buferTx.Length, SocketFlags.None, epServidor);
                    socketCliente.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al desconectar: " + ex.Message, "Cliente UDP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}