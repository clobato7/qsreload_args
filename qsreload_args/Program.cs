using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qlik.Engine;

// Autor: Caio Lobato
// Solução de Reload com um Aplicativo de Console utilizando .NET

namespace qsreload_args
{
    class Program
    {

        static void Main(string[] args)
        {
            string app = "";
            string url = "ws://127.0.0.1:4848"; // Endereço para o Qlik Sense, nesse caso estou utilizando o Desktop (localhost)
            bool partial = false;

            //Argumentos da linha de comando (-a), (-p) e (-u)
            foreach (string arg in args)
            {
                Console.WriteLine(arg);
                if (arg.StartsWith("-a="))
                {
                    app = arg.Substring(3);
                }

                if (arg == "-p")
                {
                    partial = true;
                }

                if (arg.StartsWith("-u="))
                {
                    url = arg.Substring(3);
                }
            }

           
            ILocation location;

            try
            {
                location = Qlik.Engine.Location.FromUri(new Uri(url));
                //location.AsNtlmUserViaProxy(proxyUsesSsl: false); //isso aqui só serve se for para server
                location.AsDirectConnectionToPersonalEdition(); // no caso de desktop utiliza esse aqui

                IEnumerable<IAppIdentifier> apps = location.GetAppIdentifiers();
                bool found = false;
                foreach (var salmon in apps)
                {
                    if (salmon.AppName == app)
                    {
                        Console.WriteLine("Performing " + (partial ? "PARTIAL " : "") + "reload of " + salmon.AppName + " ");
                        IApp foundApp = location.Hub().OpenApp(salmon.AppId);
                        bool ging = foundApp.DoReload(0, partial);
                        Console.WriteLine("Success: " + ging);
                        foundApp.DoSave();
                        found = true;
                    }
                }
                if (!found)
                {
                    Console.WriteLine("App " + app + " could not be found"); //Aplicativo utilizado no argumento (-a) não encontrado, não esquecer de colocar .qvf
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection to Qlik Sense Proxy at " + url + " failed"); // erro na conexão com o Qlik Sense (verificar a url)
                Console.WriteLine(ex.GetBaseException());
            }
        }

    }
}