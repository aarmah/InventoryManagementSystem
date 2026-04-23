import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { AuthService } from '../../services/auth.service';


@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrl: './admin-panel.component.css'
})
export class AdminPanelComponent implements OnInit {
  firstName = '';
  totalProducts = 0;
  totalCategories = 0;
  totalValue = 0;

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    const user = this.authService.getUserInfo();
    this.firstName = user?.firstName || 'Admin';
    this.loadStats();
  }

  loadStats() {
    this.productService.getProducts().subscribe({
      next: (products) => {
        this.totalProducts = products.length;
        this.totalValue = products.reduce((sum, p) => sum + (p.price * p.quantity), 0);
      }
    });
    this.categoryService.getCategories().subscribe({
      next: (categories) => this.totalCategories = categories.length
    });
  }
}