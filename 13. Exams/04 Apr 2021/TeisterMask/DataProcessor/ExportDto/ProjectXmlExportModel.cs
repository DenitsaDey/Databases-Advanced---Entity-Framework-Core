using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ExportDto
{
    [XmlType("Project")]
    public class ProjectXmlExportModel
    {
        [XmlAttribute("TasksCount")]
        public string TasksCount { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [XmlElement("ProjectName")]
        public string Name { get; set; }

        [XmlElement("HasEndDate")]
        public string HasEndDate { get; set; }

        [XmlArray("Tasks")]
        public TaskXmlExportModel[] Tasks { get; set; }
    }
}
