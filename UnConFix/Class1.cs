using SDG.Framework.Modules;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnConFix
{
    public class DynamicTPS : MonoBehaviour, IModuleNexus
    {
        int targetTPS = 10;
        int sleepTPS = 1;
        public void shutdown()
        {
            SDG.Unturned.Provider.onClientConnected -= CheckCount;
            SDG.Unturned.Provider.onClientDisconnected -= CheckCount;
            Console.WriteLine("JH sleeper Module uninitialized.");
        }

        void IModuleNexus.initialize()
        {
            targetTPS = Application.targetFrameRate;
            SDG.Unturned.Provider.onClientConnected += CheckCount; // ()
            SDG.Unturned.Provider.onClientDisconnected += CheckCount; // ()
            Console.WriteLine("Custom JH sleeper Module for putting inactive server to 'sleep' initialized!");
        }

        private void CheckCount()
        {
            if (SDG.Unturned.Provider.clients.Count == 0) // 0 online players
            {
                if (Application.targetFrameRate != sleepTPS) // only modify if needed
                {
                    targetTPS = Application.targetFrameRate; // keep the normal desired TPS for the future when changing back (RocketMod allows configured amount, while vanilla is set to 50.)
                    Application.targetFrameRate = sleepTPS;
                }
            }
            else if (Application.targetFrameRate == sleepTPS) // only modify if server was previously put to sleep
            {
                Application.targetFrameRate = targetTPS;
            }
        }
    }
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
        List<string> cmdQ = new List<string>();
        void IModuleNexus.initialize()
        {
            Console.WriteLine("Custom JH Module initiating");
            Dedicator.commandWindow.addIOHandler(this);
            //inputCommitted += Test;
            Task.Run(() => {
                Console.WriteLine("Custom JH CMD listener activated.");
                string cmd;
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
            Console.WriteLine("Custom JH Module unloading");
            if(act) Dedicator.commandWindow.removeIOHandler(this);
            Console.WriteLine("Custom JH Module unloaded");
            act = false;
        }

        void ICommandInputOutput.shutdown(CommandWindow commandWindow)
        {
            Console.WriteLine("[2] Custom JH Module unloading");
            //if(act) Dedicator.commandWindow.removeIOHandler(this);
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