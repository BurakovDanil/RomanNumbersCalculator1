using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanNumbersCalculator.Models
{
    internal class RomanNumberException: Exception
    {
        private const string baseErrorMessage = "#ERROR";
        public RomanNumberException(string message=baseErrorMessage) : base(message) { }
    }
}
