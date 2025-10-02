import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private token: string | null = null;

  constructor(private router: Router) { }

  // Check for a token in the URL, store it, and navigate to the dashboard.
  handleAuthentication(): void {
    const urlParams = new URLSearchParams(window.location.search);
    const token = urlParams.get('token');

    if (token) {
      this.setToken(token);
      // Navigate to the dashboard, removing the token from the URL.
      this.router.navigate(['/dashboard']);
    }
  }

  // Store the JWT in local storage.
  setToken(token: string): void {
    this.token = token;
    localStorage.setItem('auth_token', token);
  }

  // Retrieve the JWT from local storage.
  getToken(): string | null {
    if (!this.token) {
      this.token = localStorage.getItem('auth_token');
    }
    return this.token;
  }

  // Check if the user is authenticated.
  isAuthenticated(): boolean {
    return this.getToken() !== null;
  }

  // Log the user out by clearing the token and redirecting to the login page.
  logout(): void {
    this.token = null;
    localStorage.removeItem('auth_token');
    this.router.navigate(['/login']);
  }
}