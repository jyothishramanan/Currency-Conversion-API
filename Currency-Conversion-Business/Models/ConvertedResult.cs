using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currency_Conversion_Business.Models
{
    public class ConvertedResult
    {
        public double Value { get; set; }   
        public ConversionStatus Status { get; set; }
    }
    public enum ConversionStatus
    {
        UnableToFind=-2,
        Invalid=-1,
        Success=1
    }
}
