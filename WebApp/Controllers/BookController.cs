using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class BookController(IBookService service) : Controller
{
    // GET
    public async Task<IActionResult> Index() => View(await service.GetPageAsync(1, 10, null));
    
    [HttpGet("Book/Details/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var book = await service.GetByIdAsync(id);
        return book is null ? NotFound(): View(book);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(BookFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        var result = await service.AddAsync(new Book
        {
            Title = model.Title, 
            Isbn = model.Isbn, 
            PublicationDate = model.PublicationDate,
            NumPages = model.NumPages, Price = model.Price, Author = model.Author
        });
        
        TempData["Success"] = $"Dodano książkę „{model.Title}” pod id {result.Id}.";
        return RedirectToAction(nameof(Index));   
    }

    public async Task<IActionResult> ByYear(int year)
    {
        return View(nameof(Index), BookStoreMemory.Books.Where(b => b.PublicationDate.Value.Year == year));
    }

    public async Task<IActionResult> ByIsbn(string isbn)
    {
        var book = BookStoreMemory.Books.FirstOrDefault(b => b.Isbn.Replace("-", string.Empty) == isbn);
        return book is null ?NotFound(): View(nameof(Details), book);
    }

    public IActionResult Docs(string? path)
    {
        return View(path);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await service.GetByIdAsync(id);
        return View(book);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteById(int id, BookDeleteViewModel model)
    {
        var result = await service.DeleteAsync(model.Id);
        if (result)
            TempData["Success"] = $"Dodano książkę o id {model.Id}.";
        else
        {
            TempData["Failed"] = $"Nie usunięto książki";
        }
        return RedirectToAction(nameof(Index)); 
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var book = service.GetByIdAsync(id).GetAwaiter().GetResult();
        if(book is null) return NotFound();
        return View(new BookEditViewModel()
        {
            Id = book.Id,
            Title = book.Title,
            Isbn = book.Isbn,
            PublicationDate = book.PublicationDate,
            NumPages = book.NumPages,
            Price = book.Price,
            Author = book.Author
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(BookEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        var result = await service.UpdateAsync(new Book
        {
            Id = model.Id,
            Title = model.Title, 
            Isbn = model.Isbn, 
            PublicationDate = model.PublicationDate,
            NumPages = model.NumPages, 
            Price = model.Price, 
            Author = model.Author
        });
        
        TempData["Success"] = $"Dodano książkę „{model.Title}” pod id {model.Id}.";
        return RedirectToAction(nameof(Index));
    }
}