using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Data.DTO
{
    public class RegisterInstructorDTO
    {
        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }

        public string? FullName { get; set; }

        public DateTime Birthday { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? TaxNumber { get; set; }

        public string? CardName { get; set; }

        public string? CardProvider { get; set; }

        public string? CardNumber { get; set; }

        public string? SubmitCertificate { get; set; }
    }
}
