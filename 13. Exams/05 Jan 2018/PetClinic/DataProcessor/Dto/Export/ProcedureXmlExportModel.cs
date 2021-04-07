using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dto.Export
{
    [XmlType("Procedure")]
    public class ProcedureXmlExportModel
    {
        [MaxLength(10)]
        [RegularExpression(@"^[A-z]{7}[0-9]{3}$")]
        [XmlElement("Passport")]
        public string PassportSerialNumber { get; set; }

        [Required]
        [RegularExpression(@"^((\+359)|0)[0-9]{9}$")]
        [XmlElement("OwnerNumber")]
        public string OwnerNumber { get; set; }

        [Required]
        [XmlElement("DateTime")]
        public string DateTime { get; set; }

        [XmlArray("AnimalAids")]
        public AnimalAidXmlExportModel[] AnimalAids { get; set; }

        [XmlElement("TotalPrice")]
        public decimal TotalPrice { get; set; }
    }
}
