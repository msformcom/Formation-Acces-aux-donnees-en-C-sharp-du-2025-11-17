using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class ODataResponse<T>
    {
        public T Value { get; set; }
    }
}
