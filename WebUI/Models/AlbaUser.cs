using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebUI.Models
{
    public class AlbaUser
    {
        [Key]
        public int Id { get; set; }
        public string User { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastSignIn { get; set; }

        [DataType(DataType.Date)]
        public DateTime Updated { get; set; }
    }
}
