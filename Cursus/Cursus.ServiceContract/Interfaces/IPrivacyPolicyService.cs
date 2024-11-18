using Cursus.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.ServiceContract.Interfaces
{
    public interface IPrivacyPolicyService
    {
        Task<IEnumerable<PrivacyPolicyDTO>> GetPrivacyPolicyAsync();
        Task<PrivacyPolicyDTO> CreatePrivacyPolicyAsync(PrivacyPolicyDTO privacyPolicyDTO);
        Task<PrivacyPolicyDTO> UpdatePrivacyPolicyAsync(int id, PrivacyPolicyDTO privacyPolicyDTO);
        Task DeletePrivacyPolicyAsync(int id);
    }
}
