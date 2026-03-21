using System;

namespace MyCode
{
    public class MyClass
    {
        private readonly IDependency _dependency;

        public MyClass(IDependency dependency)
        {
            _dependency = dependency;
        }

        public void FirstMethod()
        {
            Console.WriteLine("First method");
        }

        public void SecondMethod()
        {
            Console.WriteLine("Second method");
        }

        public void ThirdMethod(int a)
        {
            Console.WriteLine("Third method (int)");
        }

        public void ThirdMethod(double a)
        {
            Console.WriteLine("Third method (double)");
        }

        public int Calculate(int number, string s)
        {
            return number;
        }
    }
}
