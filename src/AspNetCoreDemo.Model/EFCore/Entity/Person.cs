using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.EFCore.Entity
{
    public class Person
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Pwd { get; set; }
        public DateTime? RegDate { get; set; }
        public int? LoginNum { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
