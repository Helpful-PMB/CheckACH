// THIS SOFTWARE IS PROVIDED ``AS IS'' AND WITHOUT ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.

using System;
using System.Collections.Generic;
using System.IO;

namespace CheckACH
{
	internal static class Program
	{
		private static readonly List<string> Errors = new List<string>();
		private static int Row = 0;

		private static void AddError(string error)
		{
			Errors.Add("Row " + Row + ": " + error);
		}

		private static int Main(string[] args)
		{
			try
			{
				Console.WriteLine("Usage: CheckACH.exe <Source File>");
				Console.WriteLine("  THIS SOFTWARE IS PROVIDED ``AS IS'' AND WITHOUT ANY EXPRESS OR");
				Console.WriteLine("  IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED");
				Console.WriteLine("  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.");

				var source = args[0];
				bool fileControl = false;

				foreach (var line in File.ReadAllLines(source))
				{
					if (line.Length != 94 && !fileControl)
					{
						AddError("Row length is not 94 characters");
					}

					switch (line[0])
					{
						case '9': fileControl = true; break;
						case '6': CheckDetailRecord(line); break;
						default: break;
					}

					Row++;
				}

				Console.WriteLine();

				if (Errors.Count == 0)
				{
					Console.WriteLine("**** FILE IS OK");
					return 0;
				}

				Console.WriteLine("**** ERRORS *****");

				foreach (var error in Errors)
				{
					Console.WriteLine(error);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.WriteLine("**** An error occurred! *****");
			}

			return -1;
		}

		public static void CheckDetailRecord(string line)
		{
			string aba = line.Substring(3, 9);

			CheckABA(aba);
		}

		public static bool CheckABA(string digits)
		{
			for (int i = 0; i < 8; i++)
			{
				char c = digits[i];

				if (!Char.IsDigit(c))
				{
					AddError("Invalid Non-digit character in DFI Number");
					return false;
				}
			}

			string aba = digits.Substring(0, 8);
			int checkdigit = digits[8] - '0';

			int caclCheckDigit = GetCheckDigit(aba);

			if (checkdigit != caclCheckDigit)
			{
				AddError("Invalid check digit in DFI Number");
				return false;
			}

			return true;
		}

		public static int GetCheckDigit(string digits)
		{
			if (digits.Length != 8)
				return -1;

			int[] mod = { 3, 7, 1, 3, 7, 1, 3, 7 };

			int sum = 0;

			for (int i = 0; i < 8; i++)
			{
				int c = digits[i] - '0';

				sum += (c * mod[i]);
			}

			sum = (sum % 10);

			if (sum == 0)
				return 0;

			return 10 - sum;
		}
	}
}
