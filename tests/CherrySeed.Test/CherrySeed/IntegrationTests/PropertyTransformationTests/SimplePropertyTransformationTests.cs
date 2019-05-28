using System;
using System.Collections.Generic;
using System.Linq;
using CherrySeed.Configuration.Exceptions;
using CherrySeed.EntityDataProvider;
using CherrySeed.ObjectTransformation;
using CherrySeed.Test.Asserts;
using CherrySeed.Test.Base.Repositories;
using CherrySeed.Test.Infrastructure;
using CherrySeed.Test.Mocks;
using CherrySeed.Test.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace CherrySeed.Test.IntegrationTests.PropertyTransformationTests
{
    [TestClass]
    public class SimplePropertyTransformationTests
    {
        private CherrySeedDriver _cherrySeedDriver;

        [TestInitialize]
        public void Setup()
        {
            _cherrySeedDriver = new CherrySeedDriver();
        }

        [TestMethod]
        public void TransformSimplePropertyTypes()
        {
            // Arrange 
            var entityData = new List<EntityData>
            {
                new EntityDataBuilder("CherrySeed.Test.Models.EntityWithSimpleProperties",
                    "MyInteger", "MyString", "MyBool", "MyDateTime", "MyDouble", "MyDecimal")
                    .WithEntity("1", "MyString 1", "true", "2016/05/03", "123,12", "12,33")
                    .Build()
            };

            // Act
            var repository = new InMemoryRepository();
            _cherrySeedDriver.InitAndSeed(entityData.ToDictionaryDataProvider(), repository, cfg =>
            {
                cfg.ForEntity<EntityWithSimpleProperties>();
            });

            // Assert
            Assert.AreEqual(repository.CountSeededObjects(), 1);
            Assert.AreEqual(repository.CountSeededObjects<EntityWithSimpleProperties>(), 1);
            EntityAsserts.AssertEntityWithSimpleProperties(repository.GetEntities<EntityWithSimpleProperties>().First(), new EntityWithSimpleProperties
            {
                MyInteger = 1,
                MyString = "MyString 1",
                MyBool = true,
                MyDateTime = new DateTime(2016, 5, 3),
                MyDouble = 123.12,
                MyDecimal = 12.33m
            });
        }

        [TestMethod]
        public void TransformSimplePropertyTypesWithNullString()
        {
            // Arrange 
            var entityData = new List<EntityData>
            {
                new EntityDataBuilder("CherrySeed.Test.Models.EntityWithSimpleProperties",
                    "MyString")
                    .WithEntity("")
                    .Build()
            };

            // Act
            var repository = new InMemoryRepository();
            _cherrySeedDriver.InitAndSeed(entityData.ToDictionaryDataProvider(), repository, cfg =>
            {
                cfg.ForEntity<EntityWithSimpleProperties>();
            });

            // Assert
            Assert.AreEqual(repository.CountSeededObjects(), 1);
            Assert.AreEqual(repository.CountSeededObjects<EntityWithSimpleProperties>(), 1);
            EntityAsserts.AssertEntityWithSimpleProperties(repository.GetEntities<EntityWithSimpleProperties>().First(), new EntityWithSimpleProperties
            {
                MyString = null
            });
        }

        [TestMethod]
        public void TransformSimplePropertyTypesWithEmptyString()
        {
            // Arrange 
            var entityData = new List<EntityData>
            {
                new EntityDataBuilder("CherrySeed.Test.Models.EntityWithSimpleProperties",
                    "MyString")
                    .WithEntity("$EMPTY$")
                    .Build()
            };

            // Act
            var repository = new InMemoryRepository();
            _cherrySeedDriver.InitAndSeed(entityData.ToDictionaryDataProvider(), repository, cfg =>
            {
                cfg.ForEntity<EntityWithSimpleProperties>();
            });

            // Assert
            Assert.AreEqual(repository.CountSeededObjects(), 1);
            Assert.AreEqual(repository.CountSeededObjects<EntityWithSimpleProperties>(), 1);
            EntityAsserts.AssertEntityWithSimpleProperties(repository.GetEntities<EntityWithSimpleProperties>().First(), new EntityWithSimpleProperties
            {
                MyString = string.Empty
            });
        }

        [TestMethod]
        public void TransformSimplePropertyTypesWithCustomEmptyString()
        {
            // Arrange 
            var entityData = new List<EntityData>
            {
                new EntityDataBuilder("CherrySeed.Test.Models.EntityWithSimpleProperties",
                    "MyString")
                    .WithEntity("%%")
                    .Build()
            };

            // Act
            var repository = new InMemoryRepository();
            _cherrySeedDriver.InitAndSeed(entityData.ToDictionaryDataProvider(), repository, cfg =>
            {
                cfg.WithEmptyStringMarker("%%");
                cfg.ForEntity<EntityWithSimpleProperties>();
            });

            // Assert
            Assert.AreEqual(repository.CountSeededObjects(), 1);
            Assert.AreEqual(repository.CountSeededObjects<EntityWithSimpleProperties>(), 1);
            EntityAsserts.AssertEntityWithSimpleProperties(repository.GetEntities<EntityWithSimpleProperties>().First(), new EntityWithSimpleProperties
            {
                MyString = string.Empty
            });
        }

        [TestMethod]
        public void TransformSimplePropertyTypesWithCustomEmptyStringAfterAddingCustomStringTransformation()
        {

            // Arrange 
            var entityData = new List<EntityData>
                {
                    new EntityDataBuilder("CherrySeed.Test.Models.EntityWithSimpleProperties",
                        "MyString")
                        .WithEntity("Sample text")
                        .Build()
                };

            // Act
            var repository = new InMemoryRepository();

            Action act = () => _cherrySeedDriver.InitAndSeed(entityData.ToDictionaryDataProvider(), repository, cfg =>
            {
                cfg.AddTypeTransformation(typeof(string), new CustomTypeTransformation<string>("Other sample text"));
                cfg.WithEmptyStringMarker("%%");
                cfg.ForEntity<EntityWithSimpleProperties>();
            });
            // Assert
            act.Should().Throw<ConfigurationException>().WithMessage("EmptyString marker can not be set, because the string transformation logic is overriden from you.");
        }

        [TestMethod]
        public void TransformSimplePropertyTypesWithCustomEmptyStringBeforeAddingCustomStringTransformation()
        {

            // Arrange 
            var entityData = new List<EntityData>
                {
                    new EntityDataBuilder("CherrySeed.Test.Models.EntityWithSimpleProperties",
                        "MyString")
                        .WithEntity("Sample text")
                        .Build()
                };

            // Act
            var repository = new InMemoryRepository();


            Action act = () => _cherrySeedDriver.InitAndSeed(entityData.ToDictionaryDataProvider(), repository, cfg =>
            {
                cfg.WithEmptyStringMarker("%%");
                cfg.AddTypeTransformation(typeof(string), new CustomTypeTransformation<string>("Other sample text"));
                cfg.ForEntity<EntityWithSimpleProperties>();
            });

            // Assert
            act.Should().Throw<ConfigurationException>().WithMessage("EmptyString marker can not be set, because the string transformation logic is overriden from you.");

        }

        [TestMethod]
        public void TransformSimplePropertyTypesWithCustomEmptyString_NotEmptyString()
        {
            // Arrange 
            var entityData = new List<EntityData>
            {
                new EntityDataBuilder("CherrySeed.Test.Models.EntityWithSimpleProperties",
                    "MyString")
                    .WithEntity("Das ist kein empty %% string")
                    .Build()
            };

            // Act
            var repository = new InMemoryRepository();
            _cherrySeedDriver.InitAndSeed(entityData.ToDictionaryDataProvider(), repository, cfg =>
            {
                cfg.WithEmptyStringMarker("%%");
                cfg.ForEntity<EntityWithSimpleProperties>();
            });

            // Assert
            Assert.AreEqual(repository.CountSeededObjects(), 1);
            Assert.AreEqual(repository.CountSeededObjects<EntityWithSimpleProperties>(), 1);
            EntityAsserts.AssertEntityWithSimpleProperties(repository.GetEntities<EntityWithSimpleProperties>().First(), new EntityWithSimpleProperties
            {
                MyString = "Das ist kein empty %% string"
            });
        }

        [TestMethod]
        public void IncorrectPropertyName_PropertyMappingExceptionWithMissingProperty()
        {

            // Arrange
            var entityData = new List<EntityData>
                {
                    new EntityData
                    {
                        EntityName = "CherrySeed.Test.Models.EntityWithSimpleProperties",
                        Objects = new List<Dictionary<string, string>>
                        {
                            new Dictionary<string, string>
                            {
                                {"IncorrectFieldName", "1"}
                            }
                        }
                    }
                };

            // Act
            Action act = () => _cherrySeedDriver.InitAndSeed(entityData.ToDictionaryDataProvider(), new EmptyRepository(), cfg =>
            {
                cfg.ForEntity<EntityWithSimpleProperties>();
            });

            // Assert
            act.Should().Throw<PropertyTransformationException>().WithMessage("Transformation of Property 'IncorrectFieldName' of type 'CherrySeed.Test.Models.EntityWithSimpleProperties' to value '1' failed")
                .WithInnerException<NullReferenceException>()
                .WithMessage("Property is missing");


        }

        [TestMethod]
        public void IncorrectPropertyType_PropertyMappingExceptionWithFormatException()
        {
            // Arrange
            var entityData = new List<EntityData>
                {
                    new EntityData
                    {
                        EntityName = "CherrySeed.Test.Models.EntityWithSimpleProperties",
                        Objects = new List<Dictionary<string, string>>
                        {
                            new Dictionary<string, string>
                            {
                                {"MyInteger", "NotANumber"}
                            }
                        }
                    }
                };

            // Act
            Action act = () => _cherrySeedDriver.InitAndSeed(entityData.ToDictionaryDataProvider(), new EmptyRepository(), cfg =>
            {
                cfg.ForEntity<EntityWithSimpleProperties>();
            });

            // Assert
            act.Should().Throw<PropertyTransformationException>().WithMessage("Transformation of Property 'MyInteger' of type 'CherrySeed.Test.Models.EntityWithSimpleProperties' to value 'NotANumber' failed")
                .WithInnerException<FormatException>()
                .WithMessage("Property is missing");

        }

        [TestMethod]
        public void NotSupportedPropertyType_PropertyMappingExceptionWithNotSupportedException()
        {
            // Arrange
            var entityData = new List<EntityData>
                {
                    new EntityData
                    {
                        EntityName = "CherrySeed.Test.Models.EntityWithNotSupportedTypeProperty",
                        Objects = new List<Dictionary<string, string>>
                        {
                            new Dictionary<string, string>
                            {
                                {"UintProperty", "123"}
                            }
                        }
                    }
                };

            // Act
            Action act = () => _cherrySeedDriver.InitAndSeed(entityData.ToDictionaryDataProvider(), new EmptyRepository(), cfg =>
            {
                cfg.ForEntity<EntityWithNotSupportedTypeProperty>();
            });

            // Assert
            act.Should().Throw<PropertyTransformationException>().WithMessage("Transformation of Property 'UintProperty' of type 'CherrySeed.Test.Models.EntityWithNotSupportedTypeProperty' to value '123' failed")
            .WithInnerException<NotSupportedException>()
            .WithMessage("Transformation of type 'System.UInt32' is currently not supported");
        }
    }
}
