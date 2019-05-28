using System;
using System.Collections.Generic;
using CherrySeed.Configuration;
using CherrySeed.Configuration.Exceptions;
using CherrySeed.EntityDataProvider;
using CherrySeed.Test.Base.Repositories;
using CherrySeed.Test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace CherrySeed.Test.IntegrationTests
{
    [TestClass]
    public class ConfigurationValidationTests
    {
        [TestMethod]
        public void DataProviderNotSet_MissingConfigurationException()
        {

            var config = new CherrySeedConfiguration(cfg =>
            {
                cfg.WithRepository(new EmptyRepository());
            });

            Action act = () => config.CreateSeeder();

            act.Should().Throw<MissingConfigurationException>().WithMessage("DataProvider");
        }

        [TestMethod]
        public void RepositoryNotSet_MissingConfigurationException()
        {

            var config = new CherrySeedConfiguration(cfg =>
            {
                cfg.WithDataProvider(new DictionaryDataProvider(new List<EntityData>()));
            });

            Action act = () => config.CreateSeeder();

            act.Should().Throw<MissingConfigurationException>().WithMessage("Repository");
        }
    }
}
