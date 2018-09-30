using System;
using Storiveo.IsisPie;

namespace StationServices
{
    class Program
    {      
        static void Main(string[] args)
        {
			var bootstrap = new Bootstrap();
			bootstrap.Start();
        }
    }
}
