// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using AutoMapper;
using Identity.Clients.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Identity.Clients.Tests
{
    public class CoreTest
    {
        public CoreTest()
        {
            Mapper = new AutoMapper.MapperConfiguration(cfg => {
                cfg.AddClientMaps();
            }).CreateMapper();

            _dbOptions = new DbContextOptionsBuilder<ClientDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

        }

        protected ILoggerFactory _mill = null;
        private DbContextOptions<ClientDbContext> _dbOptions;
        protected IMapper Mapper { get; }

        protected TestContext CreateTestContext()
        {
            return new TestContext(
                CreateContext(),
                _mill,
                Mapper
            );
        }

        protected ClientDbContext CreateContext()
        {
            return new ClientDbContext(_dbOptions);
        }

    }
}
