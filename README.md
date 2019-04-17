# Mocks Stuffs

Persistence Mocks implement IDbContext<TKey>. Each entity implement IBaseEntity<int> to be handle by the mock context. Also mock 
context implements IDbContextUtility<int> to generate an Id in case we set generateId = true (which is the default value).
To add a entity to your implementation of DbContext insert a property like:\
    <p align="center">
        public ICollection\<Entity\> Entity { get; set; } = new List<Entity>();\
    </p>
This property most have the same name as the class Entity.



Database mock example

    class Program
    {
        static void Main(string[] args)
        {
            CustomDbContext context = new CustomDbContext(new DbContextUtility());

            for (int i = 0; i < 10; ++i)
                context.Add(new Info { Data = i.ToString() + " aa" });

            IEnumerable<Info> infos = context.GetEntity<Info>(x => true);
            foreach (var info in infos)
                Console.WriteLine(info.Id + " " + info.Data);
            Console.WriteLine(context.GetEntity<Info>(x => x.Data == "1 aa").FirstOrDefault().Data);
        }
    }

    internal class Info : Mocks.Entities.BaseEntityContract.IBaseEntity<int>
    {
        public int Id { get; set; }
        public string Data { get; set; }
    }

    internal class CustomDbContext : Mocks.MockPersistence.Implementation.BaseMockDbContext<int>
    {
        public CustomDbContext(IDbContextUtility<int> dBContextUtility) : base(dBContextUtility)
        {
        }

        public ICollection<Info> Info { get; set; } = new List<Info>
        {
            new Info
            {
                Id = 1,
                Data = "IA10"
            }
        };
    }

    internal class DbContextUtility : IDbContextUtility<int>
    {
        private static Random rnd = new Random();
        public int GenerateNewKey()
        {
            return rnd.Next();
        }
    }
