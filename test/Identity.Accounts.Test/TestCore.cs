// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using Identity.Accounts.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Xunit;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Common
{
    public class TestCore
    {
        public TestCore()
        {
            Mapper = new AutoMapper.MapperConfiguration(cfg => {
                cfg.AddAccountMaps();
            }).CreateMapper();

            _dbOptions = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

        }

        protected ILoggerFactory _mill = null;
        private DbContextOptions<AccountDbContext> _dbOptions;
        protected IMapper Mapper { get; }

        protected TestContext CreateTestContext()
        {
            return new TestContext(
                CreateContext(),
                _mill,
                Mapper
            );
        }

        protected AccountDbContext CreateContext()
        {
            return new AccountDbContext(_dbOptions);
        }

    }
}
