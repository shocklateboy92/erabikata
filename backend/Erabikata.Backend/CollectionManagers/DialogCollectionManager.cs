using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Grpc.Core;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Erabikata.Backend.CollectionManagers
{
    public class DialogCollectionManager : ICollectionManager
    {
        private readonly IMongoCollection<Dialog> _mongoCollection;

        public DialogCollectionManager(IMongoDatabase database)
        {
            _mongoCollection = database.GetCollection<Dialog>(nameof(Dialog));
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case BeginIngestion:
                    await _mongoCollection.DeleteManyAsync(FilterDefinition<Dialog>.Empty);
                    var client = new AnalyzerService.AnalyzerServiceClient(
                        new Channel("127.0.0.1:5001", ChannelCredentials.Insecure)
                    );
                    var resposne = await client.AnalyzeTextAsync(
                        new AnalyzeRequest {Mode = AnalyzerMode.SudachiC, Text = "本当のテクストではありません"}
                    );
                    Console.WriteLine(JsonConvert.SerializeObject(resposne, Formatting.Indented));
                    break;
            }
        }

        public Task InsertMany(IEnumerable<Dialog> dialogs)
        {
            return _mongoCollection.InsertManyAsync(dialogs);
        }
    }
}