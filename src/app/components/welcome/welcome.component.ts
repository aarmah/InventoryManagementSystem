import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-welcome',
  templateUrl: './welcome.component.html',
  styleUrl: './welcome.component.css'
})

export class WelcomeComponent implements OnInit {
  isAuthenticated = false;
  userName = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.isAuthenticated = !!localStorage.getItem('auth_token');
    if (this.isAuthenticated) {
      const user = this.authService.getUserInfo();
      this.userName = user ? `${user.firstName} ${user.lastName}` : '';
    }
  }

  logout() {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('user_info');
    this.router.navigate(['/']);
    window.location.reload();
  }
}