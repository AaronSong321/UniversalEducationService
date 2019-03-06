using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIT.UES
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new Test();
            test.PersonCreate();
            test.DemonstratePerson();
            Console.ReadLine();
        }
    }
}
