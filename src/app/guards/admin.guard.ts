import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    const token = localStorage.getItem('auth_token');
    
    if (token && this.authService.isAdmin()) {
      return true;
    }
    
    // Redirect to welcome page for non-admin users
    this.router.navigate(['/']);
    return false;
  }
}