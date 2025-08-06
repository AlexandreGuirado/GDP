using GDP.Domain.Entities;
using GDP.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GDP.Web.Controllers;

public class CustomersController : Controller
{
    private readonly ICustomerRepository _repository;

    public CustomersController(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index(string nameFilter, string emailFilter)
    {
        ViewData["NameFilter"] = nameFilter;
        ViewData["EmailFilter"] = emailFilter;
        var customers = await _repository.SearchAsync(nameFilter, emailFilter);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_CustomerListPartial", customers);
        }

        return View(customers);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var customer = await _repository.GetByIdAsync(id.Value);
        if (customer == null) return NotFound();
        return View(customer);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Email,Phone")] Customer customer)
    {
        if (ModelState.IsValid)
        {
            var existCustomer = await _repository.ExistsExactAsync(customer.Name?.Trim(),
                                                                    customer.Email?.Trim(),
                                                                    customer.Phone?.Trim());
                                                                
            bool duplicate = false;

            foreach(var exist in existCustomer)
            {
                if(exist.Name == customer.Name)
                {
                    ModelState.AddModelError("Name", "Este nome já está cadastrado.");
                    duplicate = true;
                }
                if (exist.Email == customer.Email)
                {
                    ModelState.AddModelError("Email", "Este e-mail já está cadastrado.");
                    duplicate = true;
                }
            }

            if (duplicate)
                return View(customer);

            customer.RegistrationDate = DateTime.Now;
            await _repository.AddAsync(customer);
            TempData["SuccessMessage"] = "Cliente criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var customer = await _repository.GetByIdAsync(id.Value);
        if (customer == null) return NotFound();
        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Phone,RegistrationDate")] Customer customer)
    {
        if (id != customer.Id) return NotFound();

        if (ModelState.IsValid)
        {
            await _repository.UpdateAsync(customer);
            TempData["SuccessMessage"] = "Cliente atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var customer = await _repository.GetByIdAsync(id.Value);
        if (customer == null) return NotFound();
        return View(customer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repository.DeleteAsync(id);
        TempData["SuccessMessage"] = "Cliente deletado com sucesso!";
        return RedirectToAction(nameof(Index));
    }
}