using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace MusicHub.DataProcessor.ImportDtos
{
    [XmlType("Song")]
    public class SongPerformerXmlInputModel
    {
        [Required]
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}