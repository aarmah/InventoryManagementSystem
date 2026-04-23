import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-add-product',
  templateUrl: './add-product.component.html',
  styleUrl: './add-product.component.css'
})

export class AddProductComponent implements OnInit {
  product = {
    name: '',
    description: '',
    price: 0,
    quantity: 0,
    categoryId: 0
  };
  categories: any[] = [];
  loading = false;
  successMessage = '';
  errorMessage = '';
  isEditMode = false;
  productId: number = 0;

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {

     console.log('AddProductComponent initialized');

    // Check if user is admin
    if (!this.authService.isAdmin()) {
      this.router.navigate(['/products']);
      return;
    }

    // Check if editing
    this.productId = Number(this.route.snapshot.paramMap.get('id'));

     // Also check route params
    this.route.params.subscribe(params => {
      console.log('Route params:', params);
      if (params['id']) {
        this.productId = Number(params['id']);
      }
    });
    
    console.log('Product ID:', this.productId);


    if (this.productId  && this.productId > 0) {
      this.isEditMode = true;
       console.log('Edit mode enabled for product ID:', this.productId);
      this.loadProduct();
    }else{
       console.log('Add mode - no product ID found');
    }
    
    this.loadCategories();
  }

  loadProduct() {
    this.productService.getProduct(this.productId).subscribe({
      next: (data) => {
        this.product = {
          name: data.name,
          description: data.description,
          price: data.price,
          quantity: data.quantity,
          categoryId: data.categoryId
        };
      },
      error: (error) => {
        console.error('Error loading product:', error);
        this.errorMessage = 'Error loading product data';
      }
    });
  }

  loadCategories() {
    this.categoryService.getCategories().subscribe({
      next: (data) => {
        this.categories = data;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
        this.errorMessage = 'Failed to load categories';
      }
    });
  }

  onSubmit() {
  this.loading = true;
  this.errorMessage = '';
  this.successMessage = '';

  // Check if token exists
  const token = localStorage.getItem('auth_token');
  console.log('Token exists before update:', !!token);
  
  if (!token) {
    this.errorMessage = 'You are not logged in. Please login again.';
    this.loading = false;
    setTimeout(() => {
      this.router.navigate(['/login']);
    }, 2000);
    return;
  }

  if (this.isEditMode) {

      // Check token before proceeding
    const token = localStorage.getItem('auth_token');
    if (!token) {
      this.errorMessage = 'You are not logged in. Please login again.';
      setTimeout(() => {
        this.router.navigate(['/login']);
      }, 2000);
      return;
    }

    console.log('Updating product:', this.productId, this.product);
    this.productService.updateProduct(this.productId, this.product).subscribe({
      next: (response) => {
        console.log('Update response:', response);
        this.successMessage = `Product "${this.product.name}" has been updated successfully!`;
        this.loading = false;
        // Navigate after 2 seconds
        setTimeout(() => {
          this.router.navigate(['/products']);
        }, 2000);
      },
      error: (error) => {
        console.error('Error updating product - Full error:', error);
        
        if (error.status === 401) {
          this.errorMessage = 'Your session has expired. Please login again.';
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 2000);
        } else if (error.status === 404) {
          this.errorMessage = 'Product not found. It may have been deleted.';
        } else {
          this.errorMessage = error.error?.message || 'Failed to update product. Please try again.';
        }
        this.loading = false;
      }
    });
  } else {
    // Create new product logic
    this.productService.createProduct(this.product).subscribe({
      next: () => {
        this.successMessage = `Product "${this.product.name}" has been added successfully!`;
        this.resetForm();
        this.loading = false;
        setTimeout(() => {
          this.router.navigate(['/products']);
        }, 2000);
      },
      error: (error) => {
        console.error('Error creating product:', error);
        if (error.status === 401) {
          this.errorMessage = 'Your session has expired. Please login again.';
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 2000);
        } else {
          this.errorMessage = error.error?.message || 'Failed to create product';
        }
        this.loading = false;
      }
    });
  }
}
  resetForm() {
    this.product = {
      name: '',
      description: '',
      price: 0,
      quantity: 0,
      categoryId: 0
    };
  }
}