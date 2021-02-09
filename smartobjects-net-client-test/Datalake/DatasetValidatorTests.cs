using System;
using System.Collections.Generic;
using FluentAssertions;
using Mnubo.SmartObjects.Client.Datalake;
using NUnit.Framework;

namespace Mnubo.SmartObjects.Client.Test.Datalake
{
    [TestFixture]
    public class DatasetValidatorTests
    {
        private readonly IDatasetValidator _validator;

        public DatasetValidatorTests()
        {
            this._validator = new DatasetValidator();
        }

        [TestCase("")]
        [TestCase(null)]
        public void GivenNullOrEmptyDatasetKey_WhenIValidateIt_ThenIGetArgumentException(string datasetKey)
        {
            Action validateAction = () => this._validator.ValidateDatasetKey(datasetKey);

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("datasetKey cannot be null or empty");
        }

        [Test]
        public void GivenDatasetKeyGreaterThan64Chars_WhenIValidateIt_ThenIGetArgumentException()
        {
            Action validateAction = () => this._validator.ValidateDatasetKey(StringGenerator.RandomString(65));

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("datasetKey cannot be longer than 64 characters*");
        }

        [Test]
        public void GivenDatasetKeyNotMatchingTheRegex_WhenIValidateIt_ThenIGetArgumentException()
        {
            Action validateAction = () => this._validator.ValidateDatasetKey("something not matching a-zA-Z0-9-_, @#$%^");

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("datasetKey can only contain a-z, A-Z, 0-9, _ and -*");
        }

        [Test]
        public void GivenDatasetKeyThatIsValid_WhenIValidateIt_ThenItDoesNotThrow()
        {
            Action validateAction = () => this._validator.ValidateDatasetKey("somethingvalid010101");

            validateAction
                .Should()
                .NotThrow();
        }

        [TestCase("")]
        [TestCase(null)]
        public void GivenNullOrEmptyFieldKey_WhenIValidateIt_ThenIGetArgumentException(string fieldKey)
        {
            Action validateAction = () => this._validator.ValidateFieldKey(fieldKey);

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("fieldKey cannot be null or empty");
        }
        
        [Test]
        public void GivenFieldKeyGreaterThan64Chars_WhenIValidateIt_ThenIGetArgumentException()
        {
            Action validateAction = () => this._validator.ValidateFieldKey(StringGenerator.RandomString(65));

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("fieldKey cannot be longer than 64 characters*");
        }
        
        [Test]
        public void GivenFieldKeyNotMatchingTheRegex_WhenIValidateIt_ThenIGetArgumentException()
        {
            Action validateAction = () => this._validator.ValidateFieldKey("something not matching a-zA-Z0-9-_, @#$%^");

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("fieldKey can only contain a-z, A-Z, 0-9, _ and -*");
        }

        [Test]
        public void GivenFieldKeyStartingWithInvalidValue_WhenIValidateIt_ThenIGetArgumentException()
        {
            Action validateAction = () => this._validator.ValidateFieldKey("x_somekey");

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("fieldKey cannot start with x_*");
        }
        
        [Test]
        public void GivenFieldKeyThatIsValid_WhenIValidateIt_ThenItDoesNotThrow()
        {
            Action validateAction = () => this._validator.ValidateFieldKey("somethingvalid010101");

            validateAction
                .Should()
                .NotThrow();
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("something invalid &*&(*&)")]
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        public void GivenAFieldWithAnInvalidKey_WhenIValidateIt_ThenIGetArgumentException(string fieldKey)
        {
            var field = new DatasetField
            {
                Key = fieldKey
            };
            
            Action validateAction = () => this._validator.ValidateField(field);

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("fieldKey*");;
        }

        [Test]
        public void GivenAFieldDisplayNameGreaterThan255Chars_WhenIValidateIt_ThenIGetArgumentException()
        {
            var field = new DatasetField
            {
                Key = StringGenerator.RandomString(10),
                DisplayName = StringGenerator.RandomString(256)
            };
            
            Action validateAction = () => this._validator.ValidateField(field);

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("displayName cannot be longer than 255 characters*");;
        }
        
        [Test]
        public void GivenAFieldDescriptionGreaterThan1024Chars_WhenIValidateIt_ThenIGetArgumentException()
        {
            var field = new DatasetField
            {
                Key = StringGenerator.RandomString(10),
                DisplayName = StringGenerator.RandomString(30),
                Description = StringGenerator.RandomString(1025)
            };
            
            Action validateAction = () => this._validator.ValidateField(field);

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("description cannot be longer than 1024 characters*");
        }

        [Test]
        public void GivenAFieldAliasesGreaterThan10_WhenIValidateIt_ThenIGetArgumentException()
        {
            var field = new DatasetField
            {
                Key = StringGenerator.RandomString(10),
                DisplayName = StringGenerator.RandomString(30),
                Description = StringGenerator.RandomString(30),
                Aliases = new List<string>
                {
                    StringGenerator.RandomString(30),
                    StringGenerator.RandomString(30),
                    StringGenerator.RandomString(30),
                    StringGenerator.RandomString(30),
                    StringGenerator.RandomString(30),
                    StringGenerator.RandomString(30),
                    StringGenerator.RandomString(30),
                    StringGenerator.RandomString(30),
                    StringGenerator.RandomString(30),
                    StringGenerator.RandomString(30),
                    StringGenerator.RandomString(30)
                }
            };
            
            Action validateAction = () => this._validator.ValidateField(field);

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("aliases count cannot be longer than 10*");
        }

        [Test]
        public void GivenAFieldWithAliasesLengthGreaterThan255Chars_WhenIValidateIt_ThenIGetArgumentException()
        {
            var field = new DatasetField
            {
                Key = StringGenerator.RandomString(10),
                DisplayName = StringGenerator.RandomString(30),
                Description = StringGenerator.RandomString(30),
                Aliases = new List<string>
                {
                    StringGenerator.RandomString(30),
                    StringGenerator.RandomString(256),
                }
            };
            
            Action validateAction = () => this._validator.ValidateField(field);

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("alias cannot be longer than 255 characters*");
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("something invalid &*&(*&)")]
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        public void GivenACreateDatasetWithInvalidKey_WhenIValidateIt_ThenIGetArgumentException(string datasetKey)
        {
            var createDataset = new CreateDatasetRequest
            {
                DatasetKey = datasetKey
            };
            
            Action validateAction = () => this._validator.ValidateCreateDataset(createDataset);

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("datasetKey*");
        }

        [Test]
        public void GivenACreateDatasetDescriptionGreaterThan512Chars_WhenIValidateIt_ThenIGetArgumentException()
        {
            var createDataset = new CreateDatasetRequest
            {
                DatasetKey = StringGenerator.RandomString(10),
                DisplayName = StringGenerator.RandomString(30),
                Description = StringGenerator.RandomString(513)
            };
            
            Action validateAction = () => this._validator.ValidateCreateDataset(createDataset);

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("description cannot be longer than 512 characters*");
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("something invalid &*&(*&)")]
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        public void GivenACreateDatasetWithInvalidFieldKeys_WhenIValidateIt_ThenIGetArgumentException(string fieldKey)
        {
            var createDataset = new CreateDatasetRequest
            {
                DatasetKey = StringGenerator.RandomString(10),
                DisplayName = StringGenerator.RandomString(30),
                Description = StringGenerator.RandomString(30),
                Fields = new List<DatasetField>
                {
                    new ()
                    {
                        Key = fieldKey
                    }
                }
            };
            
            Action validateAction = () => this._validator.ValidateCreateDataset(createDataset);

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("fieldKey*");
        }

        [Test]
        public void GivenACreateDatasetWithFieldDisplayNameGreaterThan255Chars_WhenIValidateIt_ThenIGetArgumentException()
        {
            var createDataset = new CreateDatasetRequest
            {
                DatasetKey = StringGenerator.RandomString(10),
                DisplayName = StringGenerator.RandomString(30),
                Description = StringGenerator.RandomString(30),
                Fields = new List<DatasetField>
                {
                    new ()
                    {
                        Key = StringGenerator.RandomString(10),
                        DisplayName = StringGenerator.RandomString(256)
                    }
                }
            };
            
            Action validateAction = () => this._validator.ValidateCreateDataset(createDataset);

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("displayName cannot be longer than 255 characters*");
        }
        
        [Test]
        public void GivenACreateDatasetWithFieldDescriptionGreaterThan1024Chars_WhenIValidateIt_ThenIGetArgumentException()
        {
            var createDataset = new CreateDatasetRequest
            {
                DatasetKey = StringGenerator.RandomString(10),
                DisplayName = StringGenerator.RandomString(30),
                Description = StringGenerator.RandomString(30),
                Fields = new List<DatasetField>
                {
                    new ()
                    {
                        Key = StringGenerator.RandomString(10),
                        DisplayName = StringGenerator.RandomString(30),
                        Description = StringGenerator.RandomString(1025)
                    }
                }
            };
            
            Action validateAction = () => this._validator.ValidateCreateDataset(createDataset);

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("description cannot be longer than 1024 characters*");
        }
        
        [Test]
        public void GivenACreateDatasetWithFieldAliasesGreaterThan10_WhenIValidateIt_ThenIGetArgumentException()
        {
            var createDataset = new CreateDatasetRequest
            {
                DatasetKey = StringGenerator.RandomString(10),
                DisplayName = StringGenerator.RandomString(30),
                Description = StringGenerator.RandomString(30),
                Fields = new List<DatasetField>
                {
                    new ()
                    {
                        Key = StringGenerator.RandomString(10),
                        DisplayName = StringGenerator.RandomString(30),
                        Description = StringGenerator.RandomString(30),
                        Aliases = new List<string>
                        {
                            StringGenerator.RandomString(10),
                            StringGenerator.RandomString(10),
                            StringGenerator.RandomString(10),
                            StringGenerator.RandomString(10),
                            StringGenerator.RandomString(10),
                            StringGenerator.RandomString(10),
                            StringGenerator.RandomString(10),
                            StringGenerator.RandomString(10),
                            StringGenerator.RandomString(10),
                            StringGenerator.RandomString(10),
                            StringGenerator.RandomString(10)
                        }
                    }
                }
            };
            
            Action validateAction = () => this._validator.ValidateCreateDataset(createDataset);

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("aliases count cannot be longer than 10*");
        }
        
        [Test]
        public void GivenACreateDatasetWithFieldAliasesGreaterThan255Characters_WhenIValidateIt_ThenIGetArgumentException()
        {
            var createDataset = new CreateDatasetRequest
            {
                DatasetKey = StringGenerator.RandomString(10),
                DisplayName = StringGenerator.RandomString(30),
                Description = StringGenerator.RandomString(30),
                Fields = new List<DatasetField>
                {
                    new ()
                    {
                        Key = StringGenerator.RandomString(10),
                        DisplayName = StringGenerator.RandomString(30),
                        Description = StringGenerator.RandomString(30),
                        Aliases = new List<string>
                        {
                            StringGenerator.RandomString(10),
                            StringGenerator.RandomString(256),
                        }
                    }
                }
            };
            
            Action validateAction = () => this._validator.ValidateCreateDataset(createDataset);

            validateAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("alias cannot be longer than 255 characters*");
        }
    }
}