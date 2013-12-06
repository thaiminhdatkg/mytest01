using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Database.Model
{
    [Table("User")]
    class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public String FirstName { get; set; }

        [Required]
        public String LastName { get; set; }

        [NotMapped]
        public String FullName
        {
            get { return LastName + " " + FirstName; }
        }

        public Boolean IsDa { get; set; }
        public Boolean IsKhong { get; set; }
        public Boolean IsNone { get; set; }
    }
}
