using Microsoft.AspNetCore.Mvc;
using WebApi.Api.Repositories;
using WebApi.Api.Models;
using WebApi.Api.Dtos;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace WebApi.Api.Controllers;
// declare the class as controller
[ApiController]
// define the route
[Route("items")]
public class ItemsController : ControllerBase
{

	// private static readonly InMemItemsRepository repository = new InMemItemsRepository();
	private readonly IItemsRepository repository;

	private readonly ILogger<ItemsController> logger;
	// injection from services
	public ItemsController(IItemsRepository repository, ILogger<ItemsController> logger)
	{
		this.repository = repository;
		this.logger = logger;
	}

	// GET /items
	[HttpGet]
	public async Task<IEnumerable<ItemDto>> GetItems()
	{
		var items = (await repository.GetItemsAsync()).Select(item => item.AsDto());

		logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: retrieved {items.Count()}.");


		return items;
	}

	// GET /items/{id}
	[HttpGet("{id}")]
	public async Task<ActionResult<ItemDto>> GetItem(Guid id)
	{
		var item = await repository.GetItemAsync(id);

		if (item is null)
		{
			return NotFound();
		}
		return item.AsDto();
	}


	// POST /items
	[HttpPost]
	public async Task<ActionResult<ItemDto>> CreateItem(CreateItemDto itemDto)
	{
		Item item = new()
		{
			Id = Guid.NewGuid(),
			Name = itemDto.Name,
			Price = itemDto.Price,
			createdDate = DateTimeOffset.UtcNow
		};

		await repository.CreateItemAsync(item);

		return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item.AsDto());
	}

	// PUT /items/{id}
	[HttpPut("{id}")]
	public async Task<ActionResult> UpdateItem(Guid id, UpdateItemDto itemDto)
	{
		var existingItem = await repository.GetItemAsync(id);

		if (existingItem is null) return NotFound();

		existingItem.Name = itemDto.Name;
		existingItem.Price = itemDto.Price;




		await repository.UpdateItemAsync(existingItem);

		return NoContent();

	}


	// DELETE /items/{id}
	[HttpDelete("{id}")]
	public async Task<ActionResult> DeleteItem(Guid id)
	{

		var existingItem = await repository.GetItemAsync(id);

		if (existingItem is null) return NotFound();

		await repository.DeleteItemAsync(id);

		return NoContent();
	}


}