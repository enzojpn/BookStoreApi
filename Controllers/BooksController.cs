using System.Text;
using System.Text.Json;
using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BooksService _booksService;
    private readonly string connectionString;
    private readonly IConfiguration config;

    public BooksController(IConfiguration configuration, BooksService booksService)
    {
        _booksService = booksService;
        config = configuration;
        connectionString = this.config.GetValue<string>("AzureServiceBus");

    }

    [HttpGet]
    public async Task<List<Book>> Get() =>
        await _booksService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Book>> Get(string id)
    {
        var book = await _booksService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }
        return book;
    }




    [HttpPost]
    public async Task<IActionResult> Post(Book newBook)
    {
        await _booksService.CreateAsync(newBook);
        await SendMessageQueue(newBook);
        return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
    }

    private async Task SendMessageQueue(Book book)
    {
        string queueName = "books";
        var client = new QueueClient(connectionString, queueName, ReceiveMode.PeekLock);
        string messageBody = JsonSerializer.Serialize(book);
        var message = new Message(Encoding.UTF8.GetBytes(messageBody));

        await client.SendAsync(message);
        await client.CloseAsync();
    }

    //TODO PUT
    //TODO DELETE
}