using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SDG.Framework.Modules;
using SDG.Unturned;
using Steamworks;

namespace UnConFix
{
    class Class1 : Command, IModuleNexus, ICommandInputOutput
    {
        public event CommandInputHandler inputCommitted;

        /*
event CommandInputHandler ICommandInputOutput.inputCommitted
{
add
{
// throw new NotImplementedException();
}

remove
{
// throw new NotImplementedException();
}
}*/
        //CommandWindow cmdWin;
        bool act = true;
        List<String> cmdQ = new List<String>();
        void IModuleNexus.initialize()
        {
            Console.WriteLine("Custom JH Module initiating");
            Dedicator.commandWindow.addIOHandler(this);
            //inputCommitted += Test;
            Task.Run(() => {
                Console.WriteLine("Custom JH CMD listener activated.");
                String cmd;
                while ((cmd = Console.ReadLine()) != "" && act)
                {
                    if (cmd.Substring(0,3)=="/JH")
                    {
                        if (!Commander.execute(default(CSteamID), cmd.Substring(4))) { CommandWindow.LogErrorFormat("Unable to match \"" + cmd.Substring(4) + "\" with any built-in commands"); };
                    }
                    //Console.WriteLine("[COMMAND] >> " + cmd);
                    //execute(CSteamID.Nil, cmd);
                    //if (!Commander.execute(default(CSteamID), cmd)) { CommandWindow.LogErrorFormat("Unable to match \"" + cmd + "\" with any built-in commands"); };
                    //inputCommitted.Invoke(cmd);
                    cmdQ.Add(cmd);
                }
            });
        }

        void ICommandInputOutput.initialize(CommandWindow commandWindow)
        {
            //cmdWin = commandWindow;
            Console.WriteLine("[2] Custom JH Module loading");
        }

        void ICommandInputOutput.outputError(string error)
        {
            //Console.Error.WriteLine(error);
        }

        void ICommandInputOutput.outputInformation(string information)
        {
            //Console.WriteLine(information);
        }

        void ICommandInputOutput.outputWarning(string warning)
        {
            //Console.WriteLine("[WARN] "+warning);
        }

        void IModuleNexus.shutdown()
        {
            Dedicator.commandWindow.removeIOHandler(this);
            Console.WriteLine("Custom JH Module unloaded");
            act = false;
        }

        void ICommandInputOutput.shutdown(CommandWindow commandWindow)
        {
            Dedicator.commandWindow.removeIOHandler(this);
            Console.WriteLine("[2] Custom JH Module unloaded");
            act = false;
        }

        void ICommandInputOutput.update()
        {
            for (int i = 0; i < cmdQ.Count; i++)
            {
                inputCommitted.Invoke(cmdQ[i]);
            }
            cmdQ.Clear();
        }
    }
}
