﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        //[MinLength(3)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        //[RegularExpression(@"^([A-Z][a-z]+) ([A-Z][a-z]+)$")]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        //[Range(3,103)]
        public int Age { get; set; }

        [Required]
        public virtual ICollection<Card> Cards { get; set; } = new HashSet<Card>();
    }
}
//•	Id – integer, Primary Key
//•	Username – text with length [3, 20] (required)
//•	FullName – text, which has two words, consisting of Latin letters. Both start with an upper letter and are followed by lower letters. The two words are separated by a single space (ex. "John Smith") (required)
//•	Email – text(required)
//•	Age – integer in the range[3, 103] (required)
//•	Cards – collection of type Card
