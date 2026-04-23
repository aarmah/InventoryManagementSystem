import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CategoryService } from '../../services/category.service';
import { AuthService } from '../../services/auth.service';


@Component({
  selector: 'app-category-list',
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.css'
})

export class CategoryListComponent implements OnInit {
  categories: any[] = [];
  loading = false;
  errorMessage = '';
  isAdmin = false;

  constructor(
    private categoryService: CategoryService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.isAdmin = this.authService.isAdmin();
    this.loadCategories();
  }

  loadCategories() {
    this.loading = true;
    this.categoryService.getCategories().subscribe({
      next: (data) => {
        this.categories = data;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Error loading categories';
        this.loading = false;
      }
    });
  }

  goToAddCategory() {
    this.router.navigate(['/categories/new']);
  }

  editCategory(id: number) {
    this.router.navigate([`/categories/edit/${id}`]);
  }

  deleteCategory(id: number) {
    if (confirm('Are you sure you want to delete this category? All products in this category will be affected.')) {
      this.categoryService.deleteCategory(id).subscribe({
        next: () => {
          this.loadCategories();
        },
        error: () => {
          alert('Error deleting category');
        }
      });
    }
  }
}