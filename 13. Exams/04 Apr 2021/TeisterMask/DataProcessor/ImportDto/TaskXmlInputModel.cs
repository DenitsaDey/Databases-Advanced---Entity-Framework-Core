using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using TeisterMask.Data.Models.Enums;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Task")]
    public class TaskXmlInputModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [Required]
        [XmlElement("OpenDate")]
        public string OpenDate { get; set; }

        [Required]
        [XmlElement("DueDate")]
        public string DueDate { get; set; }

        [EnumDataType(typeof(ExecutionType))]
        [XmlElement("ExecutionType")]
        public string ExecutionType { get; set; }

        [EnumDataType(typeof(LabelType))]
        [XmlElement("LabelType")]
        public string LabelType { get; set; }
    }
}