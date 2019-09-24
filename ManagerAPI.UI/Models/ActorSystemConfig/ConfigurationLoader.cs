using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Configuration;

namespace ManagerAPI.UI.Models.ActorSystemConfig
{
    public class ConfigurationLoader
    {
        public static Config Load() => LoadConfig("akka.conf");

        private static Config LoadConfig(string configFile)
        {

            if (File.Exists(configFile))
            {
                string config = File.ReadAllText(configFile);
                return ConfigurationFactory.ParseString(config);
            }

            return Config.Empty;
        }

        //        public static Config Load()
        //        {
        //            var config = @"    
        //akka {  
        //        #log-config-on-start = on
        //        #stdout-loglevel = DEBUG
        //        #loglevel = DEBUG
        //        actor {
        //            provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""

        //            #debug {  
        //            #  receive = on 
        //            #  autoreceive = on
        //            #  lifecycle = on
        //            #  event-stream = on
        //            #  unhandled = on
        //            #}
        //            #deployment {
        //            #    /nodeActor {
        //            #        router = round-robin-pool
        //            #        #nr-of-instances = 5
        //            #        remote = ""akka.tcp://ClientActorSystem@localhost:8080""
        //            #    }
        //            #}
        //        }
        //        serialization-bindings {
        //            ""System.Object"" = wire
        //        }
        //        remote {
        //            dot-netty.tcp {
        //		        port = 8090
        //		        hostname = localhost
        //            }
        //        }
        //    }";



        //            return ConfigurationFactory.ParseString(config);
        //        }
    }
}
