using System.ComponentModel.DataAnnotations.Schema;

namespace MusicHub.Data.Models
{
    public class SongPerformer
    {
        [ForeignKey(nameof(Song))]
        public int SongId { get; set; }
        public virtual Song Song { get; set; }


        [ForeignKey(nameof(Performer))]
        public int PerformerId { get; set; }
        public virtual Performer Performer { get; set; }
    }
}
//•	SongId – integer, Primary Key
//•	Song – the performer’s song (required)
//•	PerformerId – integer, Primary Key
//•	Performer – the song’s performer (required)
