
using Currency_Conversion_API.ViewModel;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestPlatform.TestHost;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;

namespace Currency_Conversion_Test
{
    public class CurrencyIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CurrencyIntegrationTest(WebApplicationFactory<Program> factory)
        {

            _factory = factory;
            
        }

        [Fact]
        public async void getBooksTest()
        {
            // Arrange

            using (var scope = _factory.Services.CreateScope())
            {
                var scopService = scope.ServiceProvider;
                //var dbContext = scopService.GetRequiredService<BookCartDBContext>();

                //dbContext.Database.EnsureDeleted();
                //dbContext.Database.EnsureCreated();
                //dbContext.Books.Add(new Models.Entities.Book()
                //{
                //    Title = "Ancient Mariner",
                //    Author = "Coleridge",
                //    Description = "Rime of the Ancient Mariner tells of the misfortunes of a seaman who shoots an albatross, which spells disaster for his ship and fellow sailors.",
                //    CoverImage = "AncientMariner.jpeg",
                //    Price = (decimal)156,
                //    CreatedDate = DateTime.Now,
                //    IsDeleted = false

                //});
                //dbContext.SaveChanges();
            }

            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/Converter");

            //var result = await response.Content.ReadFromJsonAsync<List<GetAllBooksResponse>>();


            //response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            //result.Count.Should().Be(1);

            //result[0].FirstName.Should().Be("name1");
            //result[0].LastName.Should().Be("family1");
            //result[0].Address.Should().Be("address1");
            //result[0].BirthDay.Should().Be(new DateTime(1970, 05, 20));

        }
    }
}
