using WebApi.Api.Dtos;
using WebApi.Api.Models;

namespace WebApi.Api;

public static class Extensions
{

	// "this" in the arguments is to give the class item a method called AsDto()
	public static ItemDto AsDto(this Item item)
	{
		return new ItemDto(item.Id, item.Name, item.Price, item.createdDate);
	}
}