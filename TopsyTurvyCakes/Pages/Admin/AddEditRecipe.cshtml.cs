using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopsyTurvyCakes.Models;

namespace TopsyTurvyCakes.Pages.Admin
{
    [Authorize]
    public class AddEditRecipeModel : PageModel
    {
        [FromRoute]
        public long? Id { get; set; }

        public bool IsNewRecipe
        {
            get { return Id == null; }
        }

        [BindProperty]
        public IFormFile Image { get; set; }

        [BindProperty]
        public Recipe Recipe { get; set; }
        public IRecipesService RecipesService { get; }

        public AddEditRecipeModel(IRecipesService recipesService)
        {
            RecipesService = recipesService;
        }

        public async Task OnGet()
        {        
            Recipe = await RecipesService.FindAsync(Id.GetValueOrDefault()) 
                ?? new Recipe();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var recipe = await RecipesService.FindAsync(Id.GetValueOrDefault())
                ?? new Recipe();

            recipe.Name = Recipe.Name;
            recipe.Description = Recipe.Description;
            recipe.Ingredients = Recipe.Ingredients;
            recipe.Directions = Recipe.Directions;
            if (Image != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    await Image.CopyToAsync(stream);
                    recipe.Image = stream.ToArray();
                    recipe.ImageContentType = Image.ContentType;
                }
            }

            await RecipesService.SaveAsync(recipe);
            return RedirectToPage("/Recipe",new { id = recipe.Id });
        }

        public async Task<IActionResult> OnPostDelete()
        {
            await RecipesService.DeleteAsync(Id.Value);
            return RedirectToPage("/Index");
        }
    }
}