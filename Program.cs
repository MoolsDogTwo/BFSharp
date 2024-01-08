namespace BFSharp;

class Program
{
    private static bool CheckFile(string fileDir)
    {
        if (Path.Exists(fileDir))
        {
            if (Path.GetExtension(fileDir) == ".bf") return true;
            Console.WriteLine("Cannot read file. Make sure the extension ends in \".bf\"");
        }
        else
        {
            Console.WriteLine("Error: File not found.");
        }
        return false;
    }

    static void Main(string[] args)
    {
        string fileDir = Path.Combine(Directory.GetCurrentDirectory(), args[0]);
        
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: BFSharp [file.bf]");
            return;
        }

        if (!CheckFile(fileDir)) { return; }
        
        // Read file's characters into a program array.
        List<char> program = [];
        using (StreamReader file = new(fileDir)) program.AddRange(file.ReadToEnd());
        
        // Store index positions of loops
        Stack<int> loopStack = [];
        Dictionary<int, int> loopTable = [];

        for (int i = 0; i < program.Count; ++i)
        {
            switch (program[i])
            {
                case '[':
                    loopStack.Push(i);
                    break;
                case ']':
                    int beginIndex = loopStack.Pop();
                    loopTable[i] = beginIndex;
                    loopTable[beginIndex] = i;
                    break;
            }
        }
        
        byte[] tape = new byte[30000];
        int memPtr = 0;
        int programPointer = 0;

        while (programPointer < program.Count)
        {
            switch (program[programPointer])
            {
                case '>':
                    ++memPtr;
                    break;
                
                case '<':
                    --memPtr;
                    break;
                
                case '+':
                    ++tape[memPtr];
                    break;
                
                case '-':
                    --tape[memPtr];
                    break;
                
                case '[':
                    if (tape[memPtr] == 0) programPointer = loopTable[programPointer];
                    break;
                
                case ']':
                    if (tape[memPtr] > 0) programPointer = loopTable[programPointer];
                    break;
                
                case '.':
                    Console.Write(Convert.ToChar(tape[memPtr]));
                    break;
                
                case ',':
                    tape[memPtr] = Convert.ToByte(Console.ReadKey().KeyChar);
                    Console.WriteLine();
                    break;
            }

            ++programPointer;
        }
    }
}