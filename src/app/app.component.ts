import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  isAuthenticated = false;
  isAdmin = false;
  userName = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.checkAuthStatus();
    
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.checkAuthStatus();
      }
    });
  }

  checkAuthStatus() {
    const token = localStorage.getItem('auth_token');
    this.isAuthenticated = !!token;
    
    if (this.isAuthenticated) {
      const user = this.authService.getUserInfo();
      this.userName = user ? `${user.firstName} ${user.lastName}` : '';
      this.isAdmin = this.authService.isAdmin();
      
      // Don't redirect regular users - let them access pages
      console.log('User:', this.userName, 'Is Admin:', this.isAdmin);
    }
  }

  logout() {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('user_info');
    this.isAuthenticated = false;
    this.isAdmin = false;
    this.userName = '';
    this.router.navigate(['/']);
  }
}