using System;
using SnowFlakeDotnetCore;

namespace SnowFlakeShorterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SnowFlakeShorter snowFlakeShorter = new SnowFlakeShorter(3300,253);
            for(int i =0; i < 20; i++)
            {
                Console.WriteLine(snowFlakeShorter.nextId());
            }
        }
    }
}
