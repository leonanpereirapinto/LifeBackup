﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Xunit;

namespace LifeBackup.Integration.Tests.Setup
{
    public class TestContext : IAsyncLifetime
    {
        private readonly DockerClient _dockerClient;
        private const string ContainerImageUri = "localstack/localstack";
        private string _containerId;

        public TestContext()
        {
            _dockerClient = new DockerClientConfiguration(new Uri(DockerApiUri())).CreateClient();
        }

        public async Task InitializeAsync()
        {
            await PullImage();

            await StartContainer();
        }

        private async Task PullImage()
        {
            await _dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = ContainerImageUri,
                Tag = "latest",
            }, new AuthConfig(), new Progress<JSONMessage>());
        }

        private async Task StartContainer()
        {
            var response = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = ContainerImageUri,
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    { "4566", default }
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        { "4566", new List<PortBinding> { new PortBinding { HostPort = "4566" }}}
                    }
                },
                Env = new List<string> { "SERVICES=s3:4566"}
            });

            _containerId = response.ID;

            await _dockerClient.Containers.StartContainerAsync(_containerId, null);
        }

        private string DockerApiUri()
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (isWindows)
            {
                return "npipe://./pipe/docker_engine";
            }

            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            if (isLinux)
            {
                return "unix:/var/run/docker.sock";
            }

            throw new Exception("Unable to determine what OS this is running on");
        }

        public async Task DisposeAsync()
        {
            if (_containerId != null)
            {
                await _dockerClient.Containers.KillContainerAsync(_containerId, new ContainerKillParameters());
            }
        }
    }
}
