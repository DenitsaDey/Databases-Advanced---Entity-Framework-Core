﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDtos
{
    [XmlType("Customer")]
    public class CustomerXmlInputModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [XmlElement("FirstName")]
        public string FirstName { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [XmlElement("LastName")]
        public string LastName { get; set; }

        [Range(12, 110)]
        [XmlElement("Age")]
        public int Age { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        [XmlElement("Balance")]
        public decimal Balance { get; set; }

        [XmlArray("Tickets")]
        public TicketXmlInputModel[] Tickets { get; set; }
    }
}
