// ******************************************************************************************
// Arroyo Auz Christian Xavier.                                                             *
// 01/06/2016.                                                                              *
// ******************************************************************************************


using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Net;
using System;

namespace FrmMensaje
{
    public partial class FrmMensaje : Form
    {
        public string nombre;
        UdpClient cliente = new UdpClient(1800);
        IPEndPoint sitioRemoto = new IPEndPoint(IPAddress.Any, 0); 

        public FrmMensaje()
        {
            InitializeComponent();
        }
    }
}