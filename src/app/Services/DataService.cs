using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DogsVsCats.Contracts;
using DogsVsCats.Models;
using Google.Cloud.Datastore.V1;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;

namespace DogsVsCats.Services
{
    public class DataService : IDataService
    {
        private readonly DatastoreDb _db;
        private readonly StorageClient _storage;
        private readonly KeyFactory _keyFactory;

        private const string Kind = "Fighter";

        private readonly string _bucketName;

        public DataService(IConfiguration configuration)
        {
            _db = DatastoreDb.Create(configuration["ProjectId"]);
            _storage = StorageClient.Create();
            _keyFactory = _db.CreateKeyFactory(Kind);
            _bucketName = configuration["BucketName"];
        }

        public async Task<IEnumerable<string>> GetFightersIdsByTypeAsync(FighterType type)
        {
            var query = new Query(Kind)
            {
                Filter = Filter.Equal("type", type.ToString().ToLower()),
                Projection = { "__key__" }
            };

            var queryResults = await _db.RunQueryAsync(query);

            return queryResults.Entities.Select(entity => entity.Key.Path[0].Name).ToList();
        }

        public async Task<Fighter> GetFighterByIdAsync(string id)
        {
            var entity = await _db.LookupAsync(_keyFactory.CreateKey(id));

            if (entity == null) return null;

            var fighter = new Fighter
            {
                Id = id,
                Name = entity.Properties["name"].StringValue,
                Votes = (int)entity.Properties["vote"].IntegerValue
            };

            var image = await _storage.GetObjectAsync(_bucketName, $"{id}");

            fighter.Image = image.MediaLink;

            return fighter;
        }

        public async Task SetFighterVoteAsync(string id)
        {
            var key = _keyFactory.CreateKey(id);

            var fighter = await _db.LookupAsync(key);

            if (fighter == null) return;

            fighter["vote"] = fighter.Properties["vote"].IntegerValue + 1;

            using (var transaction = await _db.BeginTransactionAsync())
            {
                transaction.Update(fighter);
                await transaction.CommitAsync();
            }
        }

        public async Task SaveFighterAsync(Fighter fighter, Stream image, string contentType, FighterType type)
        {
            var fighterId = Guid.NewGuid().ToString();

            await _storage.UploadObjectAsync(_bucketName,
                    fighterId,
                    contentType,
                    image,
                    new UploadObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead });

            var entity = new Entity
            {
                Key = _keyFactory.CreateKey(fighterId),
                ["name"] = fighter.Name,
                ["type"] = type.ToString().ToLower(),
                ["vote"] = 0
            };

            using (var transaction = await _db.BeginTransactionAsync())
            {
                transaction.Upsert(entity);
                await transaction.CommitAsync();
            }
        }

        public async Task<IEnumerable<Fighter>> GetFightersOrderedByVoteAsync()
        {
            var query = new Query(Kind)
            {
                Order = { { "vote", PropertyOrder.Types.Direction.Descending } }
            };

            var queryResults = await _db.RunQueryAsync(query);

            var fighters = new List<Fighter>();

            foreach (var entity in queryResults.Entities)
            {
                var id = entity.Key.Path[0].Name;
                fighters.Add(new Fighter
                {
                    Id = id,
                    Name = entity.Properties["name"].StringValue,
                    Votes = (int)entity.Properties["vote"].IntegerValue,
                    Image = $"https://storage.googleapis.com/{_bucketName}/{id}"
                });
            }

            return fighters;
        }
    }
}
