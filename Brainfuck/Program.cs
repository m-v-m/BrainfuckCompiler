using System;
using System.Collections.Generic;
using System.Linq;

namespace Brainfuck
{
	class Program
	{
		private static string printHelloWorld = "++++++++[>++++[>++>+++>+++>+<<<<-]>+>+>->>+[<]<-]>>.>---.+++++++..+++.>>.<-.<.+++.------.--------.>>+.>++.";

		private static readonly int NUMBER_OF_CELLS = 20;
		private static int pointer = 0;
		private static byte[] data = new byte[NUMBER_OF_CELLS];
		private static string programInput = "";
		static void Main(string[] args)
		{
			programInput = printHelloWorld;
			Parse(printHelloWorld);
		}

		static void Parse(string line)
		{
			for (int i = 0; i < line.Length; i++)
			{
				char character = line[i];

				switch (character)
				{
					case '>':
						MoveRight();
						break;
					case '<':
						MoveLeft();
						break;
					case '+':
						Add();
						break;
					case '-':
						Subtract();
						break;
					case '.':
						Output();
						break;
					case ',':
						Input();
						break;
					case '[':
						i = LoopStart(i);
						break;
					case ']':
						i = LoopEnd(i);
						break;
					default:
						continue;
				}
			}
		}

		static void MoveRight()
		{
			if (pointer < 0 || pointer >= NUMBER_OF_CELLS)
			{
				throw new InvalidOperationException($"Runtime error: tried to move pointer outside the allowed range of 0 - {NUMBER_OF_CELLS}");
			}
			pointer++;
		}

		static void MoveLeft()
		{
			if (pointer < 0 || pointer >= NUMBER_OF_CELLS)
			{
				throw new InvalidOperationException($"Runtime error: tried to move pointer outside the allowed range of 0 - {NUMBER_OF_CELLS}");
			}
			pointer--;
		}

		static void Add()
		{
			data[pointer]++;
		}

		static void Subtract()
		{
			data[pointer]--;
		}

		static void Input()
		{
			var input = Console.Read();
			try
			{
				var converted = Convert.ToByte(input);
				data[pointer] = converted;
			}
			catch(OverflowException)
			{
				Console.WriteLine($"Please enter a value between 0 and 255");
			}
		}

		static void Output()
		{
			Console.Write((char)data[pointer]);
		}

		static int LoopStart(int position)
		{
			// If data[pointer] == 0, jump to matching ] + 1
			if (data[pointer] != 0)
			{
				return position;
			}

			var loopEndPosition = FindMatchingLoopEnd(programInput, position);

			if (loopEndPosition == -1)
			{
				throw new InvalidOperationException($"Detected loop start without a matching loop end");
			}

			return loopEndPosition;
		}

		static int LoopEnd(int position)
		{
			// If data[pointer] != 0, jump to matching [ + 1
			if (data[pointer] == 0)
			{
				return position;
			}

			var loopEndPosition = FindMatchingLoopStart(programInput, position);

			if (loopEndPosition == -1)
			{
				throw new InvalidOperationException($"Detected loop end without a matching loop start");
			}

			return loopEndPosition;
		}

		static int FindMatchingLoopEnd(string programInput, int fromPosition)
		{
			int openLoops = 0;
			for (int i = fromPosition; i < programInput.Length; i++)
			{
				var currentCommand = programInput[i];
				if (currentCommand == '[')
				{
					openLoops++;
				}

				if (currentCommand == ']')
				{
					openLoops--;
				}

				if (openLoops == 0)
				{
					return i;
				}
			}

			return -1;
		}

		static int FindMatchingLoopStart(string programInput, int fromPosition)
		{
			int closedLoops = 0;
			for (int i = fromPosition; i > 0; i--)
			{
				var currentCommand = programInput[i];
				if (currentCommand == ']')
				{
					closedLoops++;
				}

				if (currentCommand == '[')
				{
					closedLoops--;
				}

				if (closedLoops == 0)
				{
					return i;
				}
			}

			return -1;
		}
	}
}
