using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dto.Import
{
    [XmlType("Procedure")]
    public class ProcedureXmlInputModel
    {
        [XmlElement("Vet")]
        [Required]
        public string VetName { get; set; }

        [XmlElement("Animal")]
        [Required]
        public string Animal { get; set; }

        [XmlElement("DateTime")]
        [Required]
        public string DateTime { get; set; }

        [XmlArray("AnimalAids")]
        public AnimalAidXmlInputModel[] AnimalAids { get; set; }
    }
}
