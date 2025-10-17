using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLHDotNetTrainingBatch3.ConsoleApp
{
    internal class Student
    {
        public int studentID;
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                Console.WriteLine("Before value: " + name);
                name = value;
                Console.WriteLine("After value: " + name);
            }
        }
        

    }
}
