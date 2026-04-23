import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    const token = localStorage.getItem('auth_token');
    
    // Allow ALL authenticated users (both admin and regular)
    if (token) {
      return true;
    }
    
    // Redirect unauthenticated users to home page
    this.router.navigate(['/']);
    return false;
  }
}