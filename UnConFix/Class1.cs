using SDG.Framework.Modules;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnConFix
{
    public class DynamicTPS : IModuleNexus
    {
        int targetTPS = 20;
        static int sleepTPS = 10;
        public static int Sleep_TPS
        { 
            get => sleepTPS; 
            set {
                if (Application.targetFrameRate == sleepTPS) // if server is sleeping, update the current framerate
                {
                    Application.targetFrameRate = value;
                }
                sleepTPS = value;
            }
        }
        public void shutdown()
        {
            SDG.Unturned.Provider.onServerConnected -= CheckCountJoin;
            SDG.Unturned.Provider.onServerDisconnected -= CheckCountLeave;
            Console.WriteLine("JH sleeper Module uninitialized.");
        }

        void IModuleNexus.initialize()
        {
            targetTPS = Application.targetFrameRate;
            if(Provider.getServerWorkshopFileIDs().Count == 0)
            {
                Sleep_TPS = 1;
            }
            SDG.Unturned.Provider.onServerConnected += CheckCountJoin; // ()
            SDG.Unturned.Provider.onServerDisconnected += CheckCountLeave; // ()
            Console.WriteLine("Custom JH sleeper Module for putting inactive server to 'sleep' initialized!");
        }
        private void CheckCountJoin(CSteamID r) // when a player joins, so never zero players at this point
        {
            if (Application.targetFrameRate == sleepTPS) // only modify if server was previously put to sleep
            {
                Application.targetFrameRate = targetTPS;
            }
        }
        private void CheckCountLeave(CSteamID r)
        {
            if (SDG.Unturned.Provider.clients.Count == 1) // is 1, removed from list after this event has passed
            {
                if (Application.targetFrameRate != sleepTPS) // only modify if needed
                {
                    targetTPS = Application.targetFrameRate; // keep the normal desired TPS for the future when changing back (RocketMod allows configured amount, while vanilla is set to 50.)
                    Application.targetFrameRate = sleepTPS;
                    Console.WriteLine("[JHSleeperModule] Putting server to sleep..");
                }
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
                    switch (cmd.Substring(0, 3))
                    {
                        case "/JH":
                            {
                                if (!Commander.execute(default(CSteamID), cmd.Substring(4))) { CommandWindow.LogErrorFormat("Unable to match \"" + cmd.Substring(4) + "\" with any built-in commands"); }
                                break;
                            }
                        case "/JT":
                            {
                                if(int.TryParse(cmd.Substring(4), out int newtps)) DynamicTPS.Sleep_TPS = newtps;
                                break;
                            }
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