using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApi.Api.Models;

public class Item
{

	[BsonId]
	[BsonRepresentation(BsonType.String)]
	public Guid Id { get; set; }
	public string? Name { get; set; }
	public decimal Price { get; set; }

	[BsonRepresentation(BsonType.String)]
	public DateTimeOffset createdDate { get; set; }
}