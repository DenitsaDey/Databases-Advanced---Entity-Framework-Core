using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    [XmlType("Purchase")]
    public class PurchaseInputModel
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        [EnumDataType(typeof(PurchaseType))]
        [XmlElement("Type")]
        public string Type { get; set; }

        [XmlElement("Key")]        
        [RegularExpression(@"^((([A-Z]|\d){4}-){2})(([A-Z]|\d){4})$")]
        public string ProductKey { get; set; }

        [XmlElement("Card")]
        [RegularExpression(@"^((\d{4} ){3})(\d{4})$")]
        public string Card { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }
    }
}
