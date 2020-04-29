// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using OtpNet;

namespace TotpCLient
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: dotnet totpclient.dll key");
                return;
            }
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(args[0]);
            Console.WriteLine(BitConverter.ToString(bytes));

            string b32 = Base32Encoding.ToString(bytes);
            Console.WriteLine(b32);

            bytes = Base32Encoding.ToBytes(b32);
            Console.WriteLine(BitConverter.ToString(bytes));

            var otp = new OtpNet.Totp(bytes);
            Console.WriteLine(otp.ComputeTotp());
            Console.WriteLine(otp.RemainingSeconds() + "s remaining...");

        }

    }
}
