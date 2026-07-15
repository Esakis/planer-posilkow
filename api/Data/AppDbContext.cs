using Microsoft.EntityFrameworkCore;
using TaniTydzien.Api.Models;

namespace TaniTydzien.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<SavedList> SavedLists => Set<SavedList>();
    public DbSet<SavedListItem> SavedListItems => Set<SavedListItem>();
    public DbSet<FavoriteRecipe> FavoriteRecipes => Set<FavoriteRecipe>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Product>()
            .HasOne(p => p.Ingredient)
            .WithMany(i => i.Products)
            .HasForeignKey(p => p.IngredientId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Promotion>()
            .HasOne(p => p.Product)
            .WithMany(pr => pr.Promotions)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Recipe)
            .WithMany(r => r.Ingredients)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);

        // decimal precision for SQLite is advisory but keep it explicit
        b.Entity<Product>().Property(p => p.BasePrice).HasPrecision(10, 2);
        b.Entity<Promotion>().Property(p => p.PromoPrice).HasPrecision(10, 2);
        b.Entity<Menu>().Property(m => m.EstimatedCost).HasPrecision(10, 2);
        b.Entity<User>().Property(u => u.WeeklyBudget).HasPrecision(10, 2);

        b.Entity<User>().HasIndex(u => u.Email).IsUnique();
        b.Entity<User>().HasIndex(u => u.ActivationToken);

        b.Entity<FavoriteRecipe>().HasIndex(f => new { f.UserId, f.RecipeId }).IsUnique();
        b.Entity<FavoriteRecipe>()
            .HasOne(f => f.Recipe)
            .WithMany()
            .HasForeignKey(f => f.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<SavedList>().HasIndex(l => l.UserId);
        b.Entity<SavedListItem>()
            .HasOne(i => i.SavedList)
            .WithMany(l => l.Items)
            .HasForeignKey(i => i.SavedListId)
            .OnDelete(DeleteBehavior.Cascade);
        b.Entity<SavedListItem>()
            .HasOne(i => i.Ingredient)
            .WithMany()
            .HasForeignKey(i => i.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
