using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mocks.Entities.BaseEntityContract;
using Mocks.MockPersistence.Contract;
using Mocks.MockPersistence.Implementation;
using Mocks.Tests.Common;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Mocks.BaseMockDbContext.Tests
{
    [TestClass]
    public class BaseMockDbContextTest
    {
        private IDbContext<int> DbMock;

        public BaseMockDbContextTest()
        {

        }

        [TestMethod]
        [ExtendedException(typeof(Exception), "NotDeclareEntity are not declare in context as IEnumerable")]
        public void GetList_NotIEnumerableExist_Exception()
        {
            DbMock = new MockPersistence(new DbContextUtility());
            (DbMock as MockPersistence)._GetList(typeof(NotDeclareEntity));
        }

        [TestMethod]
        public void GetList_IEnumerableExist_Null()
        {
            DbMock = new MockPersistence(new DbContextUtility());
            IEnumerable<DeclareEntityNull> declareEntityNull = (IEnumerable<DeclareEntityNull>)(DbMock as MockPersistence)._GetList(typeof(DeclareEntityNull));
            Assert.IsNull(declareEntityNull);
        }

        [TestMethod]
        public void GetEntity_GetEntityFuncLambda_Null()
        {
            DbMock = new MockPersistence(new DbContextUtility());
            Person realEntity = DbMock.GetEntity<Person>(x => x.Id == 4).FirstOrDefault();
            Assert.IsNull(realEntity);
        }

        [TestMethod]
        public void GetEntity_GetEntityFuncLambda_RealEntity()
        {
            DbMock = new MockPersistence(new DbContextUtility());
            Person realEntity = DbMock.GetEntity<Person>(x => x.Id == 1).FirstOrDefault();
            Assert.AreEqual(realEntity.Id, 1);
        }

        [TestMethod]
        public void AddEntity_Entity_Success()
        {
            DbMock = new MockPersistence(new DbContextUtility());
            Person person = new Person
            {
                Age = 23,
                Incoming = 120000.00m,
                Name = "John",
                Lastname = "Doe"
            };
            DbMock.Add(person);
            IEnumerable<Person> populatedMyEntitites = DbMock.GetEntity<Person>(x => true);
            Assert.AreEqual(populatedMyEntitites.Count(), 3);
        }

        [TestMethod]
        [ExtendedException(typeof(Exception), "NotDeclareEntity are not declare in context as ICollection<>")]
        public void AddEntity_EntityNotImplementICollection_ThrowException()
        {
            DbMock = new MockPersistence(new DbContextUtility());
            NotDeclareEntity entity = new NotDeclareEntity();
            DbMock.Add(entity);
        }

        [TestMethod]
        public void GetEntity_ConditionalLambdaExpression_Person()
        {
            DbMock = new MockPersistence(new DbContextUtility());
            Person person = DbMock.GetEntity<Person>(x => x.Lastname == "Doe" && x.Id == 1).FirstOrDefault();
            Assert.AreEqual(person.Id, 1);
            Assert.AreEqual(person.Lastname, "Doe");
        }
        
        [TestMethod]
        public void Update_Entity_UpdatedEntity()
        {
            DbMock = new MockPersistence(new DbContextUtility());
            Person person = new Person
            {
                Age = 27,
                Incoming = 120000.00m,
                Name = "John",
                Lastname = "Doe"
            };
            DbMock.Add(person);
            person.Incoming = 170000.00m;
            DbMock.Update(person);
            person = DbMock.GetEntity<Person>(x => x.Incoming == person.Incoming).FirstOrDefault();
            Assert.AreEqual(person.Incoming, 170000.00m);
        }

        [TestMethod]
        public void Remove_Entity_Removed()
        {
            DbMock = new MockPersistence(new DbContextUtility());
            DbMock.Remove(new Person { Id = 2 });
            IEnumerable<Person> people = DbMock.GetEntity<Person>(x => true);
            Assert.AreEqual(people.Count(), 1);
        }
    }

    internal class MockPersistence : BaseMockDbContext<int>
    {
        public MockPersistence(IDbContextUtility<int> contextUtility) : base(contextUtility)
        {

        }

        public IEnumerable<object> _GetList(Type type)
        {
            return GetList(type);
        }

        public ICollection<DeclareEntityNull> DeclareEntityNull { get; set; }
        public ICollection<Person> Person { get; set; } = new List<Person>
        {
            new Person
            {
                Id = 1,
                Age = 23,
                Incoming = 120000.00m,
                Name = "John",
                Lastname = "Doe"
            },
            new Person
            {
                Id = 2,
                Age = 21,
                Incoming = 120000.00m,
                Name = "Jhane",
                Lastname = "Doe"
            }
        };
    }

    public class Person : IBaseEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public int Age { get; set; }
        public decimal Incoming { get; set; }
    }

    public class DeclareEntityNull : IBaseEntity<int>
    {
        public int Id { get; set; }
        public string Data { get; set; }
    }

    public class NotDeclareEntity : IBaseEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Guid { get; set; }
    }

    

    internal class DbContextUtility : IDbContextUtility<int>
    {
        private Random rnd = new Random();
        private static object objLock = new object();

        public int GenerateNewKey()
        {
            int nm = 0;
            lock (objLock)
            {
                nm = rnd.Next();
            }
            return nm;
        }
    }
}
