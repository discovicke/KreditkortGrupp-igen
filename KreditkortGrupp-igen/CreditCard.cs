using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace KreditkortGrupp_igen
{
    public class CreditCard
    {


        public static string CreditCardGen(string baseNumber)
        {
            int sum = 0;
            bool doubleDigit = true;

            for (int i = baseNumber.Length - 1; i >= 0; i--)
            {
                int digit = baseNumber[i] - '0';
                if (doubleDigit)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }
                sum += digit;
                doubleDigit = !doubleDigit;
            }
            int checkDigit = (10 - (sum % 10)) % 10;
            return baseNumber + checkDigit;
        }
        public static string GenerateCreditCardNumber(Random rnd)
        {
            string baseNumber = "";
            for (int i = 0; i < 15; i++)
                baseNumber += rnd.Next(0, 10);
            return CreditCardGen(baseNumber);
        }
    }
}
