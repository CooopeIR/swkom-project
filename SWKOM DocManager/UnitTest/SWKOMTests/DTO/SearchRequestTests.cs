using DocumentDAL.Entities;
using SWKOM.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.SWKOMTests.DTO
{
    public class SearchRequestTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void DocumentMetadataDTO_Filled()
        {
            //Arrange & Act
            SearchRequest item = new()
            {
                SearchTerm = "GlassHouse",
                IncludeOcr = true,
            };

            //Expected
            string expected_SearchTerm = "GlassHouse";
            bool expected_IncludeOcr = true;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(expected_SearchTerm, Is.EqualTo(item.SearchTerm));
                Assert.That(expected_IncludeOcr, Is.EqualTo(item.IncludeOcr));
            });
        }
    }
}
