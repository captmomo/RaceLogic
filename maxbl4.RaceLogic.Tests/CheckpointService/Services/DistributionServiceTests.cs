﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Easy.MessageHub;
using maxbl4.RaceLogic.Checkpoints;
using maxbl4.RfidCheckpointService.Hubs;
using maxbl4.RfidCheckpointService.Services;
using maxbl4.RfidDotNet.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace maxbl4.RaceLogic.Tests.CheckpointService.Services
{
    public class DistributionServiceTests : IntegrationTestBase
    {
        public DistributionServiceTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void Should_distribute()
        {
            // One client may throw in OnNext, but observable should not fail and continue sending
            WithStorageService(storageService =>
            {
                var hubContext = Substitute.For<IHubContext<CheckpointsHub>>();
                var cps = new List<Checkpoint>();
                hubContext.Clients.Client("con1")
                            .SendCoreAsync("Checkpoint", Arg.Any<object[]>())
                        .Returns(Task.CompletedTask)
                        .AndDoes((info) => cps.Add(info.ArgAt<object[]>(1).OfType<Checkpoint>().First()));
                
                var ds = new DistributionService(hubContext, MessageHub, storageService, new BufferLogger<DistributionService>());
                ds.StartStream("con1", DateTime.Now);

                
                MessageHub.Publish(new Checkpoint("r1"));
                
                
                new Timing().Logger(Logger).Expect(() => cps.Count >= 1);
                cps[0].RiderId.ShouldBe("r1");
            });
        }
        
        [Fact]
        public void Should_send_to_other_clients_when_one_fails()
        {
            // One client may throw in OnNext, but observable should not fail and continue sending
            WithStorageService(storageService =>
            {
                var hubContext = Substitute.For<IHubContext<CheckpointsHub>>();
                var log = new List<string>();
                
                hubContext.Clients.Client("con1")
                    .SendCoreAsync(Arg.Any<string>(), Arg.Any<object[]>())
                    .ThrowsForAnyArgs(x => new ArgumentOutOfRangeException())
                    .AndDoes((info) => log.Add("thrown"));

                hubContext.Clients.Client("con2")
                    .SendCoreAsync("Checkpoint", Arg.Any<object[]>())
                    .Returns(Task.CompletedTask)
                    .AndDoes((info) => log.Add(info.ArgAt<object[]>(1).OfType<Checkpoint>().First().RiderId));
                                
                var ds = new DistributionService(hubContext, MessageHub, storageService, new BufferLogger<DistributionService>());
                ds.StartStream("con1", DateTime.Now);
                ds.StartStream("con2", DateTime.Now);

                
                MessageHub.Publish(new Checkpoint("r1"));
                
                
                new Timing().Logger(Logger).Expect(() => log.Count >= 2);
                log[0].ShouldBe("thrown");
                log[1].ShouldBe("r1");
            });
        }
    }
}