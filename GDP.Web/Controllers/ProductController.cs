using GDP.Domain.Entities;
using GDP.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GDP.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductRepository _repository;

    public ProductsController(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index(string nameFilter)
    {
        ViewData["NameFilter"] = nameFilter;
        var products = await _repository.SearchByNameAsync(nameFilter);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_ProductListPartial", products);
        }

        return View(products);
    }
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var product = await _repository.GetByIdAsync(id.Value);
        if (product == null) return NotFound();
        return View(product);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,Price,Stock")] Product product)
    {
        if (ModelState.IsValid)
        {
            await _repository.AddAsync(product);
            TempData["SuccessMessage"] = "Produto criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var product = await _repository.GetByIdAsync(id.Value);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Stock")] Product product)
    {
        if (id != product.Id) return NotFound();

        if (ModelState.IsValid)
        {
            await _repository.UpdateAsync(product);
            TempData["SuccessMessage"] = "Produto atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repository.DeleteAsync(id);
        TempData["SuccessMessage"] = "Produto deletado com sucesso!";
        return RedirectToAction(nameof(Index));
    }
}