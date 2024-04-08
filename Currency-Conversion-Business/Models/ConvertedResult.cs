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
        public int Status { get; set; }

        public Status ConversionStatus { get; set; }
    }
    public enum Status
    {
        UnableToFind,
        Invalid,
        High
    }
}
