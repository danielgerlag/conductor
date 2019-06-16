using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace ScratchPad
{
    class Program
    {
        static void Main(string[] args)
        {
            Test().Wait();
            //Console.WriteLine(Environment.CurrentDirectory);
            Console.ReadLine();
        }

        static async Task Test()
        {
            var docker = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
            var net = await docker.Networks.CreateNetworkAsync(new NetworksCreateParameters() { Name = "testnet3" } );
            
            var mongoContainerId = await StartMongo(docker, "conductor-tests-mongo");
            await docker.Networks.ConnectNetworkAsync(net.ID, new NetworkConnectParameters() {Container = mongoContainerId});

            var ms = new FileStream(@"D:\dev\Conductor\Conductor.tar", FileMode.Open);
            
            var build = await docker.Images.BuildImageFromDockerfileAsync(ms, new ImageBuildParameters()
            {
                Tags = new List<string>() { "poc" }
            });

            using (StreamReader sr = new StreamReader(build))
            {
                string line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }

            var hostCfg = new HostConfig();
            var pb = new PortBinding
            {
                HostIP = "0.0.0.0",
                HostPort = "8001"
            };

            hostCfg.PortBindings = new Dictionary<string, IList<PortBinding>>();
            hostCfg.PortBindings.Add($"{80}/tcp", new PortBinding[] { pb });

            var container = await docker.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Name = "conductor-tests",
                Image = "poc",
                Env = new List<string>() { $"DBHOST=mongodb://conductor-tests-mongo:27017/" },
                HostConfig = hostCfg,
                NetworkingConfig = new NetworkingConfig()
                {
                    EndpointsConfig = new Dictionary<string, EndpointSettings>()
                    {
                        {"", new EndpointSettings()
                        {
                            Links = new List<string>() { mongoContainerId },
                            NetworkID = net.ID
                        } },
                    }
                }
            });

            await docker.Networks.ConnectNetworkAsync(net.ID, new NetworkConnectParameters() { Container = container.ID });
            var started = await docker.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
            


            Console.WriteLine("YAY!!!!");
            await Task.Delay(0);
            
            //net.
        }

        static async Task<string> StartMongo(DockerClient docker, string name)
        {
            //await docker.Images.CreateImageAsync(new ImagesCreateParameters() { FromImage = "mongo" }, null, new Progress<JSONMessage>());


            var existing = await docker.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
            var old = existing.FirstOrDefault(x => x.Names.Any(y => y == $"/{name}"));

            //if (old != null)
            //{
            //    old.ID
            //}
            //existing[0].

            

            var container = await docker.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Image = $"mongo",
                Name = name
            });
            
            var started = await docker.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
            if (started)
            {
                return container.ID;
            }

            throw new InvalidOperationException();
        }
    }
}
