using System.Xml.Serialization;

namespace TranslatorXml.Models
{
    [XmlRoot("Pago")]
    public class Pago
    {
        public Pago()
        {
            Rut = "";
            FormaPago = "";
        }

        [XmlElement("Rut")]
        public string Rut { get; set; }

        [XmlElement("Monto")]
        public decimal Monto { get; set; }

        [XmlElement("FormaPago")]
        public string FormaPago { get; set; }

        [XmlElement("CodigoAutorizacion")]
        public string CodigoAutorizacion { get; set; }

        [XmlElement("Tarjeta")]
        public string Tarjeta { get; set; }

        public override string ToString()
        {
            return string.Format("Pago: {0} - ${1} ({2})", Rut, Monto, FormaPago);
        }
    }
}
