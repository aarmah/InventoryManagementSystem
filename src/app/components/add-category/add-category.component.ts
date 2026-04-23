import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CategoryService } from '../../services/category.service';
import { AuthService } from '../../services/auth.service';


@Component({
  selector: 'app-add-category',
  templateUrl: './add-category.component.html',
  styleUrl: './add-category.component.css'
})

export class AddCategoryComponent implements OnInit {
  category = {
    name: '',
    description: ''
  };
  loading = false;
  successMessage = '';
  errorMessage = '';
  isEditMode = false;
  categoryId: number = 0;

  constructor(
    private categoryService: CategoryService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    console.log('AddCategoryComponent initialized');
    // Check if user is admin
    if (!this.authService.isAdmin()) {
      console.log('Not admin, redirecting to categories');
      this.router.navigate(['/categories']);
      return;
    }

    // Check if editing
    this.categoryId = Number(this.route.snapshot.paramMap.get('id'));

    this.route.params.subscribe(params => {
      console.log('Route params:', params);
      if (params['id']) {
        this.categoryId = Number(params['id']);
      }
    });

    console.log('Category ID:', this.categoryId);

    if (this.categoryId && this.categoryId > 0) {
      this.isEditMode = true;
      console.log('Edit mode enabled for category ID:', this.categoryId);
      this.loadCategory();
    }else{
      console.log('Add mode - no category ID found');
    }
  }

  loadCategory() {
    this.categoryService.getCategory(this.categoryId).subscribe({
      next: (data) => {
        this.category = {
          name: data.name,
          description: data.description
        };
      },
      error: (error) => {
        console.error('Error loading category:', error);
        this.errorMessage = 'Error loading category data';
      }
    });
  }

  onSubmit() {
    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.isEditMode) {
      this.categoryService.updateCategory(this.categoryId, this.category).subscribe({
        next: () => {
          this.successMessage = `Category "${this.category.name}" has been updated successfully!`;
          this.loading = false;
          setTimeout(() => {
            this.router.navigate(['/categories']);
          }, 2000);
        },
        error: (error) => {
          console.error('Error updating category:', error);
          this.errorMessage = error.error?.message || 'Failed to update category';
          this.loading = false;
        }
      });
    } else {
      this.categoryService.createCategory(this.category).subscribe({
        next: () => {
          this.successMessage = `Category "${this.category.name}" has been added successfully!`;
          this.resetForm();
          this.loading = false;
          setTimeout(() => {
            this.router.navigate(['/categories']);
          }, 2000);
        },
        error: (error) => {
          console.error('Error creating category:', error);
          this.errorMessage = error.error?.message || 'Failed to create category';
          this.loading = false;
        }
      });
    }
  }

  resetForm() {
    this.category = {
      name: '',
      description: ''
    };
  }
}