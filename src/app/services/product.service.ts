import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Product, CreateProduct, UpdateProduct } from '../models/product';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = 'http://localhost:5004/api/products';

  constructor(private http: HttpClient) { }

  getProducts(): Observable<Product[]> {
    console.log('ProductService - Getting all products');
    return this.http.get<Product[]>(this.apiUrl).pipe(
      catchError(this.handleError)
    );
  }

  getProduct(id: number): Observable<Product> {
    console.log('ProductService - Getting product with ID:', id);
    return this.http.get<Product>(`${this.apiUrl}/${id}`).pipe(
      catchError(this.handleError)
    );
  }

  createProduct(product: CreateProduct): Observable<Product> {
    console.log('ProductService - Creating product:', product);
    return this.http.post<Product>(this.apiUrl, product).pipe(
      catchError(this.handleError)
    );
  }

  updateProduct(id: number, product: UpdateProduct): Observable<void> {
    console.log('ProductService - Updating product ID:', id, product);

  const token = localStorage.getItem('auth_token');
    console.log('=== Update Product ===');
    console.log('Product ID:', id);
    console.log('Product data:', product);
    console.log('Token exists:', !!token);

    return this.http.put<void>(`${this.apiUrl}/${id}`, product).pipe(
      catchError(this.handleError)
    );
  }

  deleteProduct(id: number): Observable<void> {
    console.log('ProductService - Deleting product ID:', id);
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: any) {
    console.error('ProductService - Error:', error);
    console.error('Error status:', error.status);
    console.error('Error message:', error.message);
    console.error('Error details:', error.error);
    return throwError(() => error);
  }
}