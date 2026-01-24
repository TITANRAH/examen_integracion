using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AdapterXml.Models
{
    [XmlRoot("Pagos")]
    public class Pagos
    {
        public Pagos()
        {
            Fecha = "";
            ListaPagos = new List<Pago>();
        }

        [XmlAttribute("fecha")]
        public string Fecha { get; set; }

        [XmlElement("Pago")]
        public List<Pago> ListaPagos { get; set; }

        public override string ToString()
        {
            return string.Format("Pagos (Fecha: {0}, Total: {1})", Fecha, ListaPagos.Count);
        }
    }
}
