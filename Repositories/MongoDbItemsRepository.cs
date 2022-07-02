using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebApi.Api.configuration;
using WebApi.Api.Models;

namespace WebApi.Api.Repositories;
public class MongoDbItemsRepository : IItemsRepository
{
	private const string databaseName = "catalog";
	private const string collectionName = "items";
	private readonly IMongoCollection<Item> itemsCollection;

	private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

	public MongoDbItemsRepository(IOptions<MongoDbSettings> MongoDbSettings)
	{

		var mongoClient = new MongoClient(connectionString: MongoDbSettings.Value.ConnectionString);
		IMongoDatabase database = mongoClient.GetDatabase(databaseName);
		itemsCollection = database.GetCollection<Item>(collectionName);
	}

	public async Task CreateItemAsync(Item item)
	{
		await itemsCollection.InsertOneAsync(item);
	}


	public async Task DeleteItemAsync(Guid id)
	{
		var filter = filterBuilder.Eq(item => item.Id, id);
		DeleteResult deleteResult = await itemsCollection.DeleteOneAsync(filter);
	}

	public async Task<Item?> GetItemAsync(Guid id)
	{
		var filter = filterBuilder.Eq(item => item.Id, id);
		return await itemsCollection.Find(filter).SingleOrDefaultAsync();
	}

	public async Task<IEnumerable<Item>> GetItemsAsync()
	{
		List<Item> items = await itemsCollection.Find(_ => true).ToListAsync();
		return items;
	}

	public async Task UpdateItemAsync(Item item)
	{
		var filter = filterBuilder.Eq(_item => _item.Id, item.Id);
		ReplaceOneResult replaceOneResult = await itemsCollection.ReplaceOneAsync(filter, item);
	}
}