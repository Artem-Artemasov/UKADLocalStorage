using NUnit.Framework;
using UKADLocalStorage.Repository;
using UKAD.Models;
using UKAD.Enums;
using System.Linq;


namespace UKADLocalStorage.Tests
{
    // Не забыть написать такс на изменение коллекции из вне
    public class LinkRepositoryTests
    {
        LinkRepository LinkRepository;
        [SetUp]
        public void SetUp()
        {
            LinkRepository = new LinkRepository();
        }

    /*    [Test]
        public void IsProcessing_EmptyString_ReturnFalse() 
        {
            //Act
            var result = LinkRepository.IsProcessing(new Link("", LocationUrl.NotFound));

            //Assert
            Assert.IsFalse(result);
        }*/

        [Test]
        public void AddAsync_CorrectLink_ShouldAddIt()
        {
            //Arrange
            var link = new Link("https://wwww.ukad-group.com/", LocationUrl.InView, 100);

            //Act
            LinkRepository.AddAsync(link).Wait();

            //Assert
            Assert.IsTrue(LinkRepository.GetLinksAsync().Result.ToList().Count == 1);

        }

        [Test]
        public void AddAsync_NewCorrectLink_ShouldAddAsNew()
        {
            //Arrange
            var link = new Link("https://wwww.ukad-group.com/", LocationUrl.InView);

            //Act
            var result = LinkRepository.AddAsync(link).Result;

            //Assert
            Assert.AreEqual(result, AddState.AddAsNew);
        }

        [Test]
        public void AddAsync_ExitsLinkWithotTime_AddStateWithOutTime()
        {
            //Arrange
            var link = new Link("https://wwww.ukad-group.com/", LocationUrl.InView);

            //Act
            LinkRepository.AddAsync(link).Wait();
            var result = LinkRepository.AddAsync(link).Result;

            //Assert
            Assert.AreEqual(result, AddState.ExistWithoutTime);
        }

        [Test]
        public void AddAsync_LinkExistAndHaveDifferentLocation_AddStateAsAllLocation()
        {
            //Arrange
            var link = new Link("https://wwww.ukad-group.com/", LocationUrl.InView,100);
            LinkRepository.AddAsync(link).Wait();
            link.LocationUrl = LocationUrl.InSiteMap;

            //Act
            var result = LinkRepository.AddAsync(link).Result;

            //Assert
            Assert.AreEqual(result,AddState.AddAsAllLocation);
        }

        [Test]
        public void AddAsync_ExistingLink_AddStateExitNormal()
        {
            //Arrange
            var link = new Link("https://wwww.ukad-group.com/", LocationUrl.All, 100);
            LinkRepository.AddAsync(link).Wait();
            link.LocationUrl = LocationUrl.InSiteMap;

            //Act
            var result = LinkRepository.AddAsync(link).Result;

            //Assert
            Assert.AreEqual(result, AddState.ExistNormal);
        }

        [Test]
        public void GetAllLinksAsync_TryChangeOutside_ShouldNotChange()
        {
            //Arrange
            var link = new Link("https://wwww.ukad-group.com/", LocationUrl.All, 100);
            LinkRepository.AddAsync(link).Wait();
            var beforeChange = LinkRepository.GetLinksAsync().Result.ToList();
            var afterChange  = LinkRepository.GetLinksAsync().Result.ToList();

            //Act
            afterChange.Remove(afterChange.FirstOrDefault());

            //Assert
            Assert.AreNotEqual(afterChange.Count, beforeChange.Count);
        }

        [Test]
        public void GetAllLinksAsync_TryChangeOutsideWithDelegate_ShouldNotChange()
        {
            //Arrange
            var link = new Link("https://wwww.ukad-group.com/", LocationUrl.All, 100);
            LinkRepository.AddAsync(link).Wait();
            var beforeChange = LinkRepository.GetLinksAsync(p => p.LocationUrl == LocationUrl.All).Result.ToList();
            var afterChange  = LinkRepository.GetLinksAsync(p => p.LocationUrl == LocationUrl.All).Result.ToList();

            //Act
            afterChange.Remove(afterChange.FirstOrDefault());

            //Assert
            Assert.AreNotEqual(afterChange.Count, beforeChange.Count);
        }
    }
}