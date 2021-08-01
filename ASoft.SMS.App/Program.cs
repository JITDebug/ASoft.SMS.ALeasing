using ASoft.SMS.App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.SMS.App
{
    class Program
    {
        static void Main(string[] args)
        {
            string reg_no = "";
            if (args.Length > 0)
                reg_no = args[0];

            MainService ms = new MainService();
            ms.Start();
        }
    }
}
