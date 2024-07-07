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

        bool act = true;
        List<String> cmdQ = new List<String>();
        void IModuleNexus.initialize()
        {
            Console.WriteLine("Custom JH Module initiating");
            Dedicator.commandWindow.addIOHandler(this);
            Task.Run(() => {
                Console.WriteLine("Custom JH CMD listener activated.");
                String cmd;
                while ((cmd = Console.ReadLine()) != "" && act)
                {
                    if (cmd.Substring(0,3)=="/JH")
                    {
                        if (!Commander.execute(default(CSteamID), cmd.Substring(4))) { CommandWindow.LogErrorFormat("Unable to match \"" + cmd.Substring(4) + "\" with any built-in commands"); };
                    }
                    cmdQ.Add(cmd);
                }
            });
        }

        void ICommandInputOutput.initialize(CommandWindow commandWindow)
        {
            Console.WriteLine("[2] Custom JH Module loading");
        }

        void ICommandInputOutput.outputError(string error)
        {
        }

        void ICommandInputOutput.outputInformation(string information)
        {
        }

        void ICommandInputOutput.outputWarning(string warning)
        {
        }

        void IModuleNexus.shutdown()
        {
            Console.WriteLine("Custom JH Module unloading");
            if(act) Dedicator.commandWindow.removeIOHandler(this);
            Console.WriteLine("Custom JH Module unloaded");
            act = false;
        }

        void ICommandInputOutput.shutdown(CommandWindow commandWindow)
        {
            Console.WriteLine("[2] Custom JH Module unloading");
            if(act) Dedicator.commandWindow.removeIOHandler(this);
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
