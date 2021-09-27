using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagement.RequestModels
{
    public class CustomerRecordRequestModel
    {
        public int CustomerProfileId { get; set; }
        public string Note { get; set; }
    }
}
