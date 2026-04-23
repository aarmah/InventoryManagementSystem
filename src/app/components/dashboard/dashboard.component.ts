import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  firstName = '';
  totalProducts = 0;
  totalCategories = 0;
  totalValue = 0;
  recentProducts: any[] = [];
  loading = false;
  isAdmin = false;

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    const user = this.authService.getUserInfo();
    this.firstName = user?.firstName || 'User';
    this.isAdmin = this.authService.isAdmin();
    this.loadStats();
    this.loadRecentProducts();
  }

  loadStats() {
    this.productService.getProducts().subscribe({
      next: (products) => {
        this.totalProducts = products.length;
        this.totalValue = products.reduce((sum, p) => sum + (p.price * p.quantity), 0);
      }
    });

    this.categoryService.getCategories().subscribe({
      next: (categories) => {
        this.totalCategories = categories.length;
      }
    });
  }

  loadRecentProducts() {
    this.loading = true;
    this.productService.getProducts().subscribe({
      next: (products) => {
        this.recentProducts = products.slice(0, 5);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  goToAddProduct() {
    this.router.navigate(['/products/new']);
  }

  goToAddCategory() {
    this.router.navigate(['/categories/new']);
  }

  goToProducts() {
    this.router.navigate(['/products']);
  }

  goToCategories() {
    this.router.navigate(['/categories']);
  }
}