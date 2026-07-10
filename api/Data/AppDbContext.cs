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
    }
}
