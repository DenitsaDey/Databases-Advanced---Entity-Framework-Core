using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("Purchase")]
    public class PurchaseDto
    {
        [XmlElement("Card")]
        [RegularExpression(@"^((\d{4} ){3})(\d{4})$")]
        public string Card { get; set; }

        [XmlElement("Cvc")]
        [RegularExpression(@"^(\d{3})$")]
        public string Cvc { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }

        [XmlElement("Game")]
        public GameDto Game { get; set; }
    }
}