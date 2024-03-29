﻿using Custom.DAL.Entities;
using System;

namespace Custom.Cmd
{
    public class Parsing
    {
        public static int ParsePrice(string name)
        {
            int value;

            while (true)
            {
                Console.WriteLine($"\nEnter the {name} in EUR: ");

                if (int.TryParse(Console.ReadLine(), out value))
                {
                    if (value > 99 && value < 15_000_000)
                        break;

                    Console.WriteLine($"\nNot the correct {name} format");
                }
            }

            return value;
        }

        public static DateTime ParseDateTime(string value)
        {
            DateTime year;

            while (true)
            {
                Console.Write($"\nEnter the {value} (dd.MM.yyyy): ");

                if (DateTime.TryParse(Console.ReadLine(), out year))
                {
                    if (year > DateTime.Parse("01.01.1900") && year < DateTime.Now)
                        break;
                    
                    Console.WriteLine($"\nNot the correct {value} format");
                }
            }

            return year;
        }

        public static int ParseInt(string name)
        {
            while (true)
            {
                Console.Write($"\nEnter the {name} : ");

                if (int.TryParse(Console.ReadLine(), out int value))
                    return value;
                    
                Console.WriteLine($"\nNot the correct {name} format");
            }
        }

        public static FuelType ParseFuelType()
        {
            while (true)
            {               
                if (Enum.TryParse(Console.ReadLine(), out FuelType value))
                    return value;

                Console.WriteLine($"\nNot the correct fuel type format");
            }
        }
    }
}