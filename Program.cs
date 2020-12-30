using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNGuard
{
    class Program
    {
        static void Main(string[] args)
        {
            ModuleDefMD module = ModuleDefMD.Load(args[0]);
            foreach (TypeDef type in module.Types)
            {
                if (!type.HasMethods)
                    continue;
                foreach (MethodDef method in type.Methods)
                {
                    if (method.HasBody && method.Body.HasInstructions)
                    {
                        IList<Instruction> IL = method.Body.Instructions;
                        for (int i = 0; i < IL.Count; i++)
                        {
                           if (IL[i].OpCode == OpCodes.Call && IL[i].Operand == null && IL[i + 3].OpCode == OpCodes.Br_S)
                            {
                                Console.WriteLine($"Detected Anti-Tamper @ {method.Name} ({i})");
                                IL[i].OpCode = OpCodes.Nop;
                                IL[i + 3].OpCode = OpCodes.Nop;
                            }
                        }
                    }
                }



            }
            ModuleWriterOptions ee = new ModuleWriterOptions(module);
            ee.MetadataLogger = DummyLogger.NoThrowInstance;
            module.Write($@"{Environment.CurrentDirectory}\Decoded.exe", ee);
        }
    }
}
